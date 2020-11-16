using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DDDBotX.Http.Services
{
    public static class HttpCurrentStatus
    {
        public static async Task OnHTTPRequest(HttpContext e)
        {
            ResponseData response;
            if (Program.conn.ready)
            {
                //Create base data
                response = new ResponseData
                {
                    connected = true,
                    status = new ResponseDataStatus
                    {
                        name = Program.conn.name,
                        map = Program.conn.map_name,
                        map_image_url = Program.config.game_map_url.Replace("{MAP}", Program.conn.map_name),
                        players = new List<ResponseDataStatusPlayer>()
                    }
                };

                //Add players
                foreach(var p in Program.conn.players)
                {
                    //Create basic data
                    var netUser = new ResponseDataStatusPlayer
                    {
                        name = p.player_name,
                        bot = p.is_bot,
                        classname = p.class_name,
                        deaths = p.deaths,
                        frags = p.frags,
                        steam_icon_url = null,
                        steam_id = null,
                        team = p.team
                    };

                    //Fetch additional data if this isn't a bot
                    if(!p.is_bot)
                    {
                        var u = await p.GetSteamUser();
                        netUser.steam_id = p.steam_id.ToString();
                        netUser.steam_icon_url = u.avatarfull;
                    }

                    //Add
                    response.status.players.Add(netUser);
                }
            }
            else
            {
                //Not ready!
                response = new ResponseData
                {
                    connected = false,
                    status = null
                };
            }

            await DDDHttpServer.WriteJSONToBody(e, response);
        }

        class ResponseData
        {
            public bool connected;
            public ResponseDataStatus status;
        }

        class ResponseDataStatus
        {
            public string name;
            public string map;
            public string map_image_url;
            public List<ResponseDataStatusPlayer> players;
        }

        class ResponseDataStatusPlayer
        {
            public string name;
            public string steam_icon_url;
            public string steam_id;
            public int team;
            public bool bot;
            public int frags;
            public int deaths;
            public string classname;
        }
    }
}
