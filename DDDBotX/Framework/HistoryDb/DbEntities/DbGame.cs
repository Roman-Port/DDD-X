using System;
using System.Collections.Generic;
using System.Text;

namespace DDDBotX.Framework.HistoryDb.DbEntities
{
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
