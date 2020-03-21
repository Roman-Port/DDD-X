using DDDBotX.Discord.Command;
using DDDBotX.Discord.Logs;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDDBotX.Discord
{
    public static class DiscordBot
    {
        public static DiscordClient discord;

        public static List<IDiscordCommand> commands = new List<IDiscordCommand>()
        {
            new RconDiscordCommand()
        };

        public static async Task InitAsync()
        {
            //First, set up the config.
            DiscordConfiguration dc = new DiscordConfiguration
            {
                Token = Program.config.discord_token,
                TokenType = TokenType.Bot
            };
            discord = new DiscordClient(dc); //Create client
            discord.MessageCreated += async e =>
            {
                if (!e.Author.IsBot)
                {
                    try
                    {
                        await OnDiscordMessage(e);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message + ex.StackTrace);
                    }
                }
            };

            //Connect
            await discord.ConnectAsync();

            //Init parts
            Status.DiscordServerStatus.Init();
            Program.conn.OnPlayerSendGlobalChat += Conn_OnPlayerSendGlobalChat;
            Program.conn.OnPlayerSendTeamChat += Conn_OnPlayerSendTeamChat;
            Program.conn.OnPlayerSendCommand += Conn_OnPlayerSendCommand;
            //Program.conn.OnPlayerConnect += Conn_OnPlayerConnect;
            //Program.conn.OnPlayerDisconnect += Conn_OnPlayerDisconnect;
            Program.conn.OnPlayerChangeName += Conn_OnPlayerChangeName;
            //Program.conn.OnPlayerKilled += Conn_OnPlayerKilled;
        }

        private static void Conn_OnPlayerKilled(Framework.DDDOnlinePlayer killed, Framework.DDDOnlinePlayer killer, string killer_weapon)
        {
            if (killed.is_bot)
                return;
            if (killer == null)
                return;
            if (killer.is_bot)
                return;
            LoggerToDiscord.CreatePlayerLogEvent(killed, DiscordColor.Orange, $"Player killed by {killer.player_name} ({killer.steam_id})", $"Killed by {killer_weapon}, killer has killed {killer.frags} times");
        }

        private static void Conn_OnPlayerChangeName(Framework.DDDOnlinePlayer player, string newName, string oldName)
        {
            if (player.is_bot)
                return;
            LoggerToDiscord.CreatePlayerLogEvent(player, DiscordColor.HotPink, "Player changed name", $"{oldName} -> __{newName}__");
        }

        private static void Conn_OnPlayerDisconnect(Framework.DDDOnlinePlayer player)
        {
            if (player.is_bot)
                return;
            LoggerToDiscord.CreatePlayerLogEvent(player, DiscordColor.Yellow, "Player disconnected", "");
        }

        private static void Conn_OnPlayerConnect(Framework.DDDOnlinePlayer player)
        {
            if (player.is_bot)
                return;
            LoggerToDiscord.CreatePlayerLogEvent(player, DiscordColor.Green, "Player connected", "");
        }

        private static void Conn_OnPlayerSendCommand(Framework.DDDOnlinePlayer player, string command, string args)
        {
            LoggerToDiscord.CreatePlayerLogEvent(player, DiscordColor.Red, "Attempted to run command \""+command+"\"", args);
        }

        private static void Conn_OnPlayerSendTeamChat(Framework.DDDOnlinePlayer player, string message)
        {
            LoggerToDiscord.CreatePlayerLogEvent(player, new DiscordColor(56, 130, 220), "Sent a chat message to team", message);
        }

        private static void Conn_OnPlayerSendGlobalChat(Framework.DDDOnlinePlayer player, string message)
        {
            LoggerToDiscord.CreatePlayerLogEvent(player, new DiscordColor(56, 130, 220), "Sent a chat message", message);
        }

        static async Task OnDiscordMessage(MessageCreateEventArgs e)
        {
            //Make sure this is a message we care about
            if (!e.Message.Content.StartsWith(Program.config.discord_prefix) || e.Guild == null)
                return;
            string command = e.Message.Content.Substring(Program.config.discord_prefix.Length).Split(' ')[0].ToLower();
            string args = e.Message.Content.Substring(Program.config.discord_prefix.Length + command.Length + 1);

            //Make sure the user is authenticated
            var member = await e.Guild.GetMemberAsync(e.Author.Id);
            if (member.Roles.Where(x => x.Id == Program.config.discord_admin_role_id).Count() == 0)
                return;

            //Determine command
            IDiscordCommand cmd = null;
            foreach(var c in commands)
            {
                string[] prefixes = c.GetCommandNames();
                if (!prefixes.Contains(command))
                    return;
                cmd = c;
            }

            //Check if we found a command
            if (cmd == null)
                return;

            //Run the command
            await cmd.Execute(e, command, args);
        }
    }
}
