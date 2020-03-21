using DDDBotX.Framework.HistoryDb.DbEntities;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace DDDBotX.Framework.HistoryDb
{
    public class HistoryDatabase
    {
        public LiteDatabase database;
        public ILiteCollection<DbGame> games;
        public ILiteCollection<DbPlayer> players;

        public DbGame activeGame;
        
        public HistoryDatabase(DDDConnection conn)
        {
            //Set up DB
            database = new LiteDatabase("ddd_history_v2.db");
            games = database.GetCollection<DbGame>("game_history");
            players = database.GetCollection<DbPlayer>("players");

            //Create new game
            CreateNewGame();

            //Automatically add events
            conn.OnServerMapEnd += Conn_OnServerMapEnd;
            conn.OnServerChangeMaps += Conn_OnServerChangeMaps;
            conn.OnPlayerScoreChanged += Conn_OnPlayerScoreChanged;
            conn.OnPlayerListModified += Conn_OnPlayerListModified;
        }

        public List<DbPlayer> FetchTopPlayers(int count)
        {
            return players.Query().OrderByDescending(x => x.kd_ratio).Limit(count).ToList();
        }

        public long GetNumberOfPlayers()
        {
            return players.LongCount();
        }

        private void CreateNewGame()
        {
            activeGame = new DbGame
            {
                start = DateTime.UtcNow,
                frames = new List<DbGame_Frame>(),
                map_name = Program.conn.map_name,
                _id = Guid.NewGuid(),
                participants = new List<ulong>()
            };
        }

        private DbPlayer GetPlayer(DDDOnlinePlayer data)
        {
            //Get from DB
            DbPlayer player = players.FindById(data.steam_id);
            if (player != null)
                return player;

            //Create a new player
            player = new DbPlayer
            {
                kd_ratio = 0,
                name = data.player_name,
                steam_id = data.steam_id,
                total_deaths = 0,
                total_kills = 0
            };
            return player;
        }

        private void Conn_OnPlayerListModified()
        {
            //Write the current playerlist to the active session
            //Create a new frame
            DbGame_Frame frame = new DbGame_Frame
            {
                time = DateTime.UtcNow,
                players = new List<DbGame_Frame_Player>()
            };
            foreach(var p in Program.conn.players)
            {
                if (p.is_bot)
                    continue; //Skip bots
                frame.players.Add(new DbGame_Frame_Player
                {
                    deaths = p.deaths,
                    frags = p.frags,
                    name = p.player_name,
                    steam_id = p.steam_id,
                    team = p.team
                });
            }

            //Add frame
            activeGame.frames.Add(frame);

            //Make sure all players are in the participants array
            foreach(var p in Program.conn.players)
            {
                if (!activeGame.participants.Contains(p.steam_id))
                    activeGame.participants.Add(p.steam_id);
            }
        }

        private void Conn_OnPlayerScoreChanged(DDDOnlinePlayer data, int fragsAdd, int deathsAdd)
        {
            //Make sure adds are >
            if (fragsAdd < 0 || deathsAdd < 0)
                return;

            //Make sure player is not a bot
            if (data.is_bot)
                return;
            
            //Get player info
            DbPlayer player = GetPlayer(data);

            //Update
            player.total_kills += fragsAdd;
            player.total_deaths += deathsAdd;

            //Update cached k/d
            if (player.total_deaths == 0)
                player.kd_ratio = 1000;
            else
                player.kd_ratio = (float)player.total_kills / (float)player.total_deaths;

            //Save
            players.Upsert(player);
        }

        private void Conn_OnServerChangeMaps(string nextMap)
        {
            //Create a new session
            CreateNewGame();
        }

        private void Conn_OnServerMapEnd()
        {
            //Clean up and commit the current history
            activeGame.end = DateTime.UtcNow;
            if (activeGame.frames.Count > 1)
                games.Insert(activeGame);
        }
    }
}
