using DDDBotX.Framework.MessageDecoder;
using DDDBotX.Framework.MessageDecoder.Payloads;
using DDDBotX.Framework.Steam;
using RomanPort.SourceLogLib;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace DDDBotX.Framework
{
    public class DDDConnection
    {
        public SourceLogConnection listener;
        public List<DDDOnlinePlayer> players;
        public string map_name;
        public string name = "Not Connected";
        public bool ready;

        public const string BOT_STEAM_ID = "STEAM_ID_STOP_IGNORIN";

        public event PlayerConnectArgs OnPlayerConnect;
        public event PlayerDisconnectArgs OnPlayerDisconnect;
        public event PlayerSendGlobalChatArgs OnPlayerSendGlobalChat;
        public event PlayerSendTeamChatArgs OnPlayerSendTeamChat;
        public event PlayerSendCommandArgs OnPlayerSendCommand;
        public event PlayerListModifiedArgs OnPlayerListModified;
        public event PlayerKilledArgs OnPlayerKilled;
        public event PlayerChangeNameArgs OnPlayerChangeName;
        public event ServerChangeMapsArgs OnServerChangeMaps;
        public event ServerChangeReadyStatusArgs OnServerChangeReadyStatus;
        public event PlayerScoreChangedArgs OnPlayerScoreChanged;
        public event ServerMapEndArgs OnServerMapEnd;

        public void Init()
        {
            //Set up players
            players = new List<DDDOnlinePlayer>();
            
            //Set up listener
            listener = new SourceLogConnection(IPAddress.Any, 1433);
            listener.handler += Listener_handler;
            listener.StartListen();
        }

        private void Listener_handler(DDDStringReader reader, ref int state)
        {
            //Called when we get a message from the DDD server

            //Get bytes and fix them into their real value
            byte[] s = reader.sr.ToArray();
            for (int i = 0; i < s.Length; i++)
                s[i]--;

            //Get message
            DDDMessage msg = DDDMessage.DecodeBytes(s, this);
            if (msg.GetType() == typeof(ModAuthRequestPayload))
                HandlePluginConnect((ModAuthRequestPayload)msg);
            else if (msg.GetType() == typeof(PlayerConnectRequestPayload))
                HandlePlayerConnect((PlayerConnectRequestPayload)msg);
            else if (msg.GetType() == typeof(PlayerDisconnectRequestPayload))
                HandlePlayerDisconnect((PlayerDisconnectRequestPayload)msg);
            else if (msg.GetType() == typeof(PlayerExecCommandActionPayload))
                HandlePlayerExecCommand((PlayerExecCommandActionPayload)msg);
            else if (msg.GetType() == typeof(PlayerKilledEventPayload))
                HandlePlayerKilled((PlayerKilledEventPayload)msg);
            else if (msg.GetType() == typeof(PlayerSwitchedTeamsEventPayload))
                HandlePlayerSwitchTeams((PlayerSwitchedTeamsEventPayload)msg);
            else if (msg.GetType() == typeof(PlayerSwitchedClassEventPayload))
                HandlePlayerSwitchClasses((PlayerSwitchedClassEventPayload)msg);
            else if (msg.GetType() == typeof(PlayerChangeNamePayload))
                HandlePlayerChangeName((PlayerChangeNamePayload)msg);
        }

        private DDDOnlinePlayer GetPlayerByGuid(int guid)
        {
            foreach(var p in players)
            {
                if (p.player_guid == guid)
                    return p;
            }
            return null;
        }

        private void UpdatePlayerFrags(DDDOnlinePlayer player, int newScore)
        {
            //Determine how much it has changed by
            int change = newScore - player.frags;

            //Update
            player.frags = newScore;

            //Send events
            OnPlayerScoreChanged?.Invoke(player, change, 0);
        }

        private void UpdatePlayerDeaths(DDDOnlinePlayer player, int newScore)
        {
            //Determine how much it has changed by
            int change = newScore - player.deaths;

            //Update
            player.deaths = newScore;

            //Send events
            OnPlayerScoreChanged?.Invoke(player, 0, change);
        }

        private void HandlePluginConnect(ModAuthRequestPayload m)
        {
            //Send pre events
            if (ready)
                OnServerMapEnd?.Invoke();

            //Update
            name = m.server_name;
            map_name = m.map_name;
            ready = true;

            //Send events
            OnServerChangeMaps?.Invoke(m.map_name);
            OnServerChangeReadyStatus?.Invoke(ready);

            //Clear player list
            players.Clear();
            OnPlayerListModified?.Invoke();
        }

        private void HandlePlayerConnect(PlayerConnectRequestPayload m)
        {
            //Create player
            DDDOnlinePlayer player = new DDDOnlinePlayer
            {
                player_name = m.name,
                player_guid = m.player_guid,
                class_name = "",
                frags = 0,
                deaths = 0,
                team = 0
            };

            //Set steam ID or bot
            if(m.steam_id == BOT_STEAM_ID)
            {
                player.steam_id = 0;
                player.is_bot = true;
            } else
            {
                player.is_bot = false;
                player.steam_id = ulong.Parse(m.steam_id);
                player._steam = SteamTool.FetchSteamUser(m.steam_id);
            }

            //Add
            players.Add(player);

            //Send events
            OnPlayerConnect?.Invoke(player);
            OnPlayerListModified?.Invoke();
        }

        private void HandlePlayerDisconnect(PlayerDisconnectRequestPayload m)
        {
            //Get player
            DDDOnlinePlayer player = GetPlayerByGuid(m.player_guid);
            if (player == null)
                return;

            //Remove
            players.Remove(player);

            //Send events
            OnPlayerDisconnect?.Invoke(player);
            OnPlayerListModified?.Invoke();
        }

        private void HandlePlayerExecCommand(PlayerExecCommandActionPayload m)
        {
            //Get player
            DDDOnlinePlayer player = GetPlayerByGuid(m.player_guid);
            if (player == null)
                return;

            //Send events
            if (m.command == "say")
                OnPlayerSendGlobalChat?.Invoke(player, m.args);
            else if (m.command == "say_team")
                OnPlayerSendTeamChat?.Invoke(player, m.args);
            else
                OnPlayerSendCommand?.Invoke(player, m.command, m.args);
        }

        private void HandlePlayerKilled(PlayerKilledEventPayload m)
        {
            //Get killed player
            DDDOnlinePlayer player = GetPlayerByGuid(m.killed_guid);
            if (player == null)
                return;

            //Get killer (if any)
            DDDOnlinePlayer killer = null;
            if (m.attacker_guid != 0)
                killer = GetPlayerByGuid(m.attacker_guid);

            //JANK: It seems like the game hasn't updated scores yet when this is recorded, so increase them ourself
            m.killed_deaths++;
            m.attacker_kills++;

            //Update killed scores
            UpdatePlayerFrags(player, m.killed_kills);
            UpdatePlayerDeaths(player, m.killed_deaths);

            //Update killer scores
            if(killer != null)
            {
                UpdatePlayerFrags(killer, m.attacker_kills);
                UpdatePlayerDeaths(killer, m.attacker_deaths);
            }

            //Send events
            OnPlayerKilled?.Invoke(player, killer, m.attacker_weapon);
            OnPlayerListModified?.Invoke();
        }

        private void HandlePlayerSwitchTeams(PlayerSwitchedTeamsEventPayload m)
        {
            //Get player
            DDDOnlinePlayer player = GetPlayerByGuid(m.player_guid);
            if (player == null)
                return;

            //Update
            player.team = m.team;

            //Send events
            OnPlayerListModified?.Invoke();
        }

        private void HandlePlayerSwitchClasses(PlayerSwitchedClassEventPayload m)
        {
            //Get player
            DDDOnlinePlayer player = GetPlayerByGuid(m.player_guid);
            if (player == null)
                return;

            //Update
            player.class_name = m.class_name;

            //Send events
            OnPlayerListModified?.Invoke();
        }

        private void HandlePlayerChangeName(PlayerChangeNamePayload m)
        {
            //Get player
            DDDOnlinePlayer player = GetPlayerByGuid(m.player_guid);
            if (player == null)
                return;

            //Update
            string old = player.player_name;
            player.player_name = m.new_name;

            //Send events
            OnPlayerListModified?.Invoke();
            OnPlayerChangeName?.Invoke(player, m.new_name, old);
        }
    }

    public delegate void PlayerConnectArgs(DDDOnlinePlayer player);
    public delegate void PlayerDisconnectArgs(DDDOnlinePlayer player);
    public delegate void PlayerSendGlobalChatArgs(DDDOnlinePlayer player, string message);
    public delegate void PlayerSendTeamChatArgs(DDDOnlinePlayer player, string message);
    public delegate void PlayerSendCommandArgs(DDDOnlinePlayer player, string command, string args);
    public delegate void PlayerKilledArgs(DDDOnlinePlayer killed, DDDOnlinePlayer killer, string killer_weapon);
    public delegate void PlayerChangeNameArgs(DDDOnlinePlayer player, string newName, string oldName);
    public delegate void ServerChangeMapsArgs(string nextMap);
    public delegate void ServerChangeReadyStatusArgs(bool ready);
    public delegate void PlayerScoreChangedArgs(DDDOnlinePlayer player, int fragsAdd, int deathsAdd);
    public delegate void ServerMapEndArgs();
    public delegate void PlayerListModifiedArgs();
}
