using DDDBotX.Framework.Steam;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DDDBotX.Framework
{
    public class DDDOnlinePlayer
    {
        public int player_guid; //The GUID of the player on the Source server
        public ulong steam_id; //The ulong Steam ID of this player
        public bool is_bot; //Is this a bot? If true, steam_id will be invalid
        public string player_name; //The in-game name of this player
        public byte team; //The current team this player is on. 0 is unknown
        public string class_name; //The current name of the class this player is in
        public int frags; //The number of frags
        public int deaths; //The number of deaths

        internal Task<SteamUser> _steam;

        public async Task<SteamUser> GetSteamUser()
        {
            if (is_bot)
                throw new Exception("Steam info does not exist on bots.");
            return await _steam;
        }

        public bool HasSteamInfo()
        {
            if (is_bot)
                return false;
            return _steam.IsCompletedSuccessfully;
        }
    }
}
