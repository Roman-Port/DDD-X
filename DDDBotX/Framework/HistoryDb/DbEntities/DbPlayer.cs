using System;
using System.Collections.Generic;
using System.Text;

namespace DDDBotX.Framework.HistoryDb.DbEntities
{
    public class DbPlayer
    {
        public ulong _id { get { return steam_id; } set { steam_id = value; } } //Actually just the Steam ID
        public ulong steam_id { get; set; }
        public string name { get; set; } //The latest display name
        public long total_kills { get; set; } //The total number of kills this player has
        public long total_deaths { get; set; } //The total number of deaths a player has
        public float kd_ratio { get; set; } //Cached K/D ratio
    }
}
