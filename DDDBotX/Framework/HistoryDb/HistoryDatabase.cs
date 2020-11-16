using DDDBotX.Framework.HistoryDb.DbEntities;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DDDBotX.Framework.HistoryDb
{
    public class HistoryDatabase
    {
        public LiteDatabase database;
        public ILiteCollection<DbGame> games;
        public ILiteCollection<DbPlayer> players;
        public List<ComputedPlayerData> playerData;

        public DbGame activeGame;
        
        public HistoryDatabase(DDDConnection conn, string path)
        {
            //Set up DB
            Console.WriteLine("Loading database...");
            database = new LiteDatabase(path);
            games = database.GetCollection<DbGame>("game_history");
            players = database.GetCollection<DbPlayer>("players");

            //Clean up
            //Console.WriteLine("Cleaning up DB... (DO NOT SHUT DOWN)");
            //database.Rebuild();

            //Compute player data
            Console.WriteLine("Computing leaderboard data...");
            RecomputePlayerData();

            //Create new game
            CreateNewGame();

            //Automatically add events
            conn.OnServerMapEnd += Conn_OnServerMapEnd;
            conn.OnServerChangeMaps += Conn_OnServerChangeMaps;
            conn.OnPlayerScoreChanged += Conn_OnPlayerScoreChanged;
            conn.OnPlayerListModified += Conn_OnPlayerListModified;
        }

        /// <summary>
        /// Searches for players by their name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public List<DbPlayer> SearchPlayers(string name, int limit = int.MaxValue)
        {
            var f = players.Find(x => x.name.ToLower().Contains(name), 0, limit);
            return f.ToList();
        }

        public DbPlayer GetDbPlayerBySteamId(ulong steamId)
        {
            return players.FindOne(x => x.steam_id == steamId);
        }

        public void UpdateDbPlayer(DbPlayer p)
        {
            players.Update(p);
        }

        /// <summary>
        /// Recomputes the playerData from the database
        /// </summary>
        public void RecomputePlayerData()
        {
            var g = games.FindAll();
            Console.WriteLine("Data loaded.");
            Dictionary<ulong, ComputedPlayerData> playerScores = new Dictionary<ulong, ComputedPlayerData>();
            foreach (var game in g)
            {
                //Find the last frame with each player
                Dictionary<ulong, DbGame_Frame_Player> lastFramePlayer = new Dictionary<ulong, DbGame_Frame_Player>();
                foreach (var frame in game.frames)
                {
                    foreach (var player in frame.players)
                    {
                        if (lastFramePlayer.ContainsKey(player.steam_id))
                            lastFramePlayer[player.steam_id] = player;
                        else
                            lastFramePlayer.Add(player.steam_id, player);
                    }
                }

                //Add these last frames to the total scores
                foreach (var player in lastFramePlayer)
                {
                    if (!playerScores.ContainsKey(player.Key))
                        playerScores.Add(player.Key, new ComputedPlayerData
                        {
                            kills = player.Value.frags,
                            deaths = player.Value.deaths,
                            id = player.Value.steam_id,
                            gameCount = 1,
                            lastSeen = game.end
                        });
                    else
                    {
                        playerScores[player.Key].kills += player.Value.frags;
                        playerScores[player.Key].deaths += player.Value.deaths;
                        playerScores[player.Key].lastSeen = game.end;
                        playerScores[player.Key].gameCount += 1;
                    }
                }
            }

            //Flatten
            List<ComputedPlayerData> finalPlayerScores = new List<ComputedPlayerData>();
            foreach (var player in playerScores)
            {
                if (player.Value.deaths == 0)
                    player.Value.kd = 0;
                else
                    player.Value.kd = (float)player.Value.kills / (float)player.Value.deaths;
                finalPlayerScores.Add(player.Value);
            }

            //Sort
            finalPlayerScores.Sort((ComputedPlayerData a, ComputedPlayerData b) =>
            {
                return b.kd.CompareTo(a.kd);
            });

            playerData = finalPlayerScores;
        }

        /// <summary>
        /// Returns computed data entry for player by their Steam ID
        /// </summary>
        /// <param name="steamId"></param>
        /// <returns></returns>
        public ComputedPlayerData GetPlayerDataBySteamId(ulong steamId)
        {
            foreach(var p in playerData)
            {
                if (p.id == steamId)
                    return p;
            }
            return null;
        }

        public List<ComputedPlayerData> GetTopPlayers(out List<string> steamIdsToFetch, int limit, int skip, int filterGamesMin = 0, int filterFragsMin = 0)
        {
            int read = 0;
            int skipped = 0;
            steamIdsToFetch = new List<string>();
            List<ComputedPlayerData> output = new List<ComputedPlayerData>();
            foreach (var p in Program.db.playerData)
            {
                //Make sure it fits filters
                if (p.gameCount < filterGamesMin)
                    continue;
                if (p.kills < filterFragsMin)
                    continue;

                //Check if we should skip this
                if (skipped < skip)
                {
                    skipped++;
                    continue;
                }

                //Add
                output.Add(p);
                steamIdsToFetch.Add(p.id.ToString());

                //Check if that's the end
                read++;
                if (read == limit)
                    break;
            }
            return output;
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
