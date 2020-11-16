using DDDBotX.Framework.Steam;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DDDBotX.Http.Services
{
    public static class HttpLeaderboard
    {
        public static async Task OnHTTPRequest(HttpContext e)
        {
            //Get filters
            int filterGamesMin = DDDHttpServer.TryGetIntOrDefaultFromQuery(e, "min_games", 0);
            int filterFragsMin = DDDHttpServer.TryGetIntOrDefaultFromQuery(e, "min_frags", 0);

            //Get args
            int skip = DDDHttpServer.TryGetIntOrDefaultFromQuery(e, "skip", 0);
            int limit = DDDHttpServer.TryGetIntOrDefaultFromQuery(e, "limit", 30);
            if(limit > 100)
            {
                await DDDHttpServer.WriteStringToBody(e, "The maximum limit is 100.", code: 400);
                return;
            }

            //Create base response
            ResponseData response = new ResponseData
            {
                players = new List<ResponseDataPlayer>()
            };

            //Fetch results
            List<string> steamIdsToFetch;
            var top = Program.db.GetTopPlayers(out steamIdsToFetch, limit, skip, filterGamesMin, filterFragsMin);

            //Fetch steam data
            var steamUsers = await SteamTool.FetchSteamUsers(steamIdsToFetch);

            //Add players
            int read = 0;
            foreach (var p in top)
            {
                //Add
                var r = new ResponseDataPlayer
                {
                    game_count = p.gameCount,
                    kd = p.kd,
                    last_seen = p.lastSeen,
                    steam_id = p.id.ToString(),
                    total_deaths = p.deaths,
                    total_frags = p.kills,
                    rank = skip + read
                };
                read++;

                //Get Steam data
                if (steamUsers.ContainsKey(r.steam_id))
                {
                    r.name = steamUsers[r.steam_id].personaname;
                    r.steam_icon_url = steamUsers[r.steam_id].avatarfull;
                }

                //Add
                response.players.Add(r);
            }

            //Write
            await DDDHttpServer.WriteJSONToBody(e, response);
        }

        class ResponseData
        {
            public List<ResponseDataPlayer> players;    
        }

        class ResponseDataPlayer
        {
            public int total_frags;
            public int total_deaths;
            public float kd;
            public string steam_id;
            public DateTime last_seen;
            public int game_count;
            public int rank;

            public string name;
            public string steam_icon_url;
        }
    }
}
