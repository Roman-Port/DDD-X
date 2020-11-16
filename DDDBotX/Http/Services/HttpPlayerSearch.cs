using DDDBotX.Framework.Steam;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DDDBotX.Http.Services
{
    public static class HttpPlayerSearch
    {
        public static async Task OnHTTPRequest(HttpContext e)
        {
            //Read data
            RequestData request = await DDDHttpServer.DecodePOSTBody<RequestData>(e);
            string query = request.query;
            int limit = Math.Min(100, request.limit);

            //Query
            var results = Program.db.SearchPlayers(query.ToLower(), limit);

            //Fetch steam data
            List<string> steamIdsToFetch = new List<string>();
            foreach (var r in results)
                steamIdsToFetch.Add(r.steam_id.ToString());
            var steamUsers = await SteamTool.FetchSteamUsers(steamIdsToFetch);

            //Convert
            ResponseData response = new ResponseData
            {
                query = query,
                players = new List<ResponseDataPlayer>()
            };
            foreach(var r in results)
            {
                if(steamUsers.ContainsKey(r.steam_id.ToString()))
                {
                    //Has Steam data
                    var steam = steamUsers[r.steam_id.ToString()];
                    response.players.Add(new ResponseDataPlayer
                    {
                        name = steam.personaname,
                        icon = steam.avatarfull,
                        steam_id = r.steam_id.ToString(),
                        total_deaths = r.total_deaths,
                        total_kills = r.total_kills
                    });
                } else
                {
                    //Does not have Steam data
                    response.players.Add(new ResponseDataPlayer
                    {
                        name = r.name,
                        icon = null,
                        steam_id = r.steam_id.ToString(),
                        total_deaths = r.total_deaths,
                        total_kills = r.total_kills
                    });
                }
            }

            //Write
            await DDDHttpServer.WriteJSONToBody(e, response);
        }

        class RequestData
        {
            public string query = "";
            public int limit = 100;
        }

        class ResponseData
        {
            public string query;
            public List<ResponseDataPlayer> players;
        }

        class ResponseDataPlayer
        {
            public string name;
            public string icon;
            public string steam_id;
            public long total_kills;
            public long total_deaths;
        }
    }
}
