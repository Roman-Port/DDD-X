using LiteDB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DDDHistoryInspector
{
    public partial class Form1 : Form
    {
        public LiteDatabase database;
        public ILiteCollection<DbGame> dbGames;
        public ILiteCollection<DbPlayer> dbPlayers;

        private DbGame selectedGame;
        private DbGame_Frame selectedFrame;

        public Form1()
        {
            InitializeComponent();

            //Open DB
            database = new LiteDatabase(@"C:\Users\Roman\source\repos\DDDBotX\ddd_history_v2.db");
            dbGames = database.GetCollection<DbGame>("game_history");
            dbPlayers = database.GetCollection<DbPlayer>("players");

            //Add all games
            var games = dbGames.Query().OrderBy(x => x.start).ToArray();
            selectedGame = games[0];
            listGames.BeginUpdate();
            foreach (var g in games)
            {
                DateTime startTime = ConvertTimeToLocal(g.start);
                TimeSpan len = g.end - g.start;
                var i = new ListViewItem(new string[]
                {
                    $"{startTime.ToShortDateString()} {startTime.ToLongTimeString()}",
                    $"{((len.Days * 24) + len.Hours).ToString().PadLeft(2, '0')}:{len.Minutes.ToString().PadLeft(2, '0')}:{len.Seconds.ToString().PadLeft(2, '0')}",
                    g.map_name,
                    g.participants.Count.ToString(),
                    g.frames.Count.ToString()
                });
                i.Tag = g;
                listGames.Items.Add(i);
            }
            listGames.EndUpdate();
        }

        private void UpdateFramesList()
        {
            listFrames.BeginUpdate();
            listFrames.Items.Clear();
            foreach(var f in selectedGame.frames)
            {
                TimeSpan delta = f.time - selectedGame.start;
                DateTime startTime = ConvertTimeToLocal(f.time);
                var i = new ListViewItem(new string[]
                {
                    $"{((delta.Days * 24) + delta.Hours).ToString().PadLeft(2, '0')}:{delta.Minutes.ToString().PadLeft(2, '0')}:{delta.Seconds.ToString().PadLeft(2, '0')}",
                    $"{startTime.ToLongTimeString()}",
                    f.players.Count.ToString()
                });
                i.Tag = f;
                listFrames.Items.Add(i);
            }
            listFrames.EndUpdate();
        }

        private void UpdatePlayersList()
        {
            listPlayers.BeginUpdate();
            listPlayers.Items.Clear();
            foreach (var f in selectedFrame.players)
            {
                string teamLabel = f.team.ToString();
                if (f.team == 2)
                    teamLabel = "ALLIES";
                else if (f.team == 3)
                    teamLabel = "AXIS";
                var i = new ListViewItem(new string[]
                {
                    f.name,
                    teamLabel,
                    f.frags.ToString(),
                    f.deaths.ToString()
                });
                i.Tag = f;
                listPlayers.Items.Add(i);
            }
            listPlayers.EndUpdate();
        }

        private DateTime ConvertTimeToLocal(DateTime utc)
        {
            return utc;//.AddHours(-5);
        }

        private void listGames_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(listGames.SelectedItems.Count == 1)
            {
                selectedGame = (DbGame)listGames.SelectedItems[0].Tag;
                UpdateFramesList();
            }
        }

        private void listFrames_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listFrames.SelectedItems.Count == 1)
            {
                selectedFrame = (DbGame_Frame)listFrames.SelectedItems[0].Tag;
                UpdatePlayersList();
            }
        }
    }
}
