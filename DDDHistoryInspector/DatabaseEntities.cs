using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDDHistoryInspector
{
    public class DbPlayer
    {
        public ulong _id { get { return steam_id; } set { steam_id = value; } } //Actually just the Steam ID
        public ulong steam_id { get; set; }
        public string name { get; set; } //The latest display name
        public long total_kills { get; set; } //The total number of kills this player has
        public long total_deaths { get; set; } //The total number of deaths a player has
        public float kd_ratio { get; set; } //Cached K/D ratio
        public int player_reports { get; set; } //Number of times this player has been reported
    }

    public class DbGame
    {
        public Guid _id { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public string map_name { get; set; }
        public List<DbGame_Frame> frames { get; set; }
        public List<ulong> participants { get; set; }
    }

    public class DbGame_Frame
    {
        //A game frame is a capture of the scoreboard at a current time

        public DateTime time { get; set; }
        public List<DbGame_Frame_Player> players { get; set; }
    }

    public class DbGame_Frame_Player
    {
        public ulong steam_id { get; set; }
        public int frags { get; set; }
        public int deaths { get; set; }
        public byte team { get; set; }
        public string name { get; set; }
    }
}
