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
    }
}
