using DDDBotX.Framework;
using DDDBotX.Framework.Config;
using DDDBotX.Framework.HistoryDb;
using Newtonsoft.Json;
using RomanPort.SourceLogLib;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace DDDBotX
{
    class Program
    {
        public static string configPath;
        public static DDDConnection conn;
        public static BotConfig config;
        public static HistoryDatabase db;

        static void Main(string[] args)
        {
            //Load config
            configPath = "config.json";
            config = JsonConvert.DeserializeObject<BotConfig>(File.ReadAllText(configPath));
            
            //Set up connection
            conn = new DDDConnection();
            conn.Init();
            //conn.OnPlayerListModified += Conn_OnPlayerListModified;

            //Set up DB
            db = new HistoryDatabase(conn);

            //Init Discord
            Discord.DiscordBot.InitAsync().GetAwaiter().GetResult();

            //Hang
            Task.Delay(-1).GetAwaiter().GetResult();
        }

        public static void SaveConfig()
        {
            File.WriteAllText(configPath, JsonConvert.SerializeObject(config));
        }

        private static void Conn_OnPlayerListModified()
        {
            Console.Clear();
            foreach(var p in conn.players)
            {
                Console.WriteLine(p.player_name + " | " + p.steam_id + " | " + p.team + " | " + p.frags + " | " + p.deaths);
            }
        }
    }
}
