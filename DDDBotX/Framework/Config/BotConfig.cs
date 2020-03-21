using System;
using System.Collections.Generic;
using System.Text;

namespace DDDBotX.Framework.Config
{
    public class BotConfig
    {
        public BotConfig_TeamDefinition teamA;
        public BotConfig_TeamDefinition teamB;

        public string game_icon_url;
        public string game_map_url; //Replaces "{MAP}" with a map name

        public string steam_api_key;

        public string game_ip;
        public int game_port;
        public string rcon_password;

        public string discord_token;
        public string discord_prefix;
        public ulong discord_admin_role_id;
        public int discord_status_update_seconds;
        public ulong discord_logs_channel;
        public ulong discord_status_channel;
        public ulong discord_status_message;
    }

    public class BotConfig_TeamDefinition
    {
        public int id;
        public string name;
        public string emoji;
    }
}
