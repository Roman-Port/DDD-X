using DDDBotX.Framework;
using DDDBotX.Framework.Config;
using DDDBotX.Framework.HistoryDb;
using DDDBotX.Http;
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
            configPath = args[0];
            config = JsonConvert.DeserializeObject<BotConfig>(File.ReadAllText(configPath));
            
            //Set up connection
            conn = new DDDConnection();
            conn.Init();

            //Set up DB
            db = new HistoryDatabase(conn, config.db_path);
            db.database.Rebuild();
            Console.WriteLine("DB data loaded.");

            //Init Discord
            Discord.DiscordBot.InitAsync().GetAwaiter().GetResult();
            Console.WriteLine("Discord init OK.");

            //Start HTTP server
            DDDHttpServer.RunAsync().GetAwaiter().GetResult();
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
