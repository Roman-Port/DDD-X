using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DDDBotX.Framework.Steam
{
    public static class SteamTool
    {
        public static async Task<SteamUser> FetchSteamUser(string id)
        {
            //Make request
            string url = $"https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={Program.config.steam_api_key}&steamids={id}";
            string response;
            try
            {
                using (HttpClient wc = new HttpClient())
                    response = await wc.GetStringAsync(url);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to fetch Steam data: " + ex.Message);
                return null;
            }

            //Deserialize
            SteamUserPlayersWrapper wrapper = JsonConvert.DeserializeObject<SteamUserPlayersWrapper>(response);
            if (wrapper.response == null)
                return null;
            if (wrapper.response.players == null)
                return null;
            if (wrapper.response.players.Length != 1)
                return null;
            return wrapper.response.players[0];
        }

        public static async Task<Dictionary<string, SteamUser>> FetchSteamUsers(List<string> ids)
        {
            //Make request
            string idList = "";
            for(int i = 0; i<ids.Count; i++)
            {
                idList += ids[i];
                if (i != ids.Count - 1)
                    idList += ",";
            }
            string url = $"https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={Program.config.steam_api_key}&steamids={idList}";
            string response;
            try
            {
                using (HttpClient wc = new HttpClient())
                    response = await wc.GetStringAsync(url);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to fetch Steam data: " + ex.Message);
                return new Dictionary<string, SteamUser>();
            }

            //Deserialize
            SteamUserPlayersWrapper wrapper = JsonConvert.DeserializeObject<SteamUserPlayersWrapper>(response);
            if (wrapper.response == null)
                return new Dictionary<string, SteamUser>();
            if (wrapper.response.players == null)
                return new Dictionary<string, SteamUser>();

            //Create dict of IDs
            var output = new Dictionary<string, SteamUser>();
            foreach(var p in wrapper.response.players)
            {
                output.Add(p.steamid, p);
            }
            return output;
        }
    }
}
