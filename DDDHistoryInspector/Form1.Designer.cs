namespace DDDHistoryInspector
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.listGames = new System.Windows.Forms.ListView();
            this.startTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.length = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.mapName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.players = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.frames = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.listFrames = new System.Windows.Forms.ListView();
            this.deltaTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.realTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.playerCount = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.listPlayers = new System.Windows.Forms.ListView();
            this.name = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.team = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.frags = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.deaths = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // listGames
            // 
            this.listGames.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listGames.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.startTime,
            this.length,
            this.mapName,
            this.players,
            this.frames});
            this.listGames.FullRowSelect = true;
            this.listGames.HideSelection = false;
            this.listGames.Location = new System.Drawing.Point(6, 19);
            this.listGames.Name = "listGames";
            this.listGames.Size = new System.Drawing.Size(434, 388);
            this.listGames.TabIndex = 0;
            this.listGames.UseCompatibleStateImageBehavior = false;
            this.listGames.View = System.Windows.Forms.View.Details;
            this.listGames.SelectedIndexChanged += new System.EventHandler(this.listGames_SelectedIndexChanged);
            // 
            // startTime
            // 
            this.startTime.Text = "Start Time";
            this.startTime.Width = 150;
            // 
            // length
            // 
            this.length.Text = "Length";
            this.length.Width = 70;
            // 
            // mapName
            // 
            this.mapName.Text = "Map";
            this.mapName.Width = 80;
            // 
            // players
            // 
            this.players.Text = "Players";
            this.players.Width = 50;
            // 
            // frames
            // 
            this.frames.Text = "Frames";
            this.frames.Width = 50;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.listGames);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(446, 413);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Games";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox2.Controls.Add(this.listFrames);
            this.groupBox2.Location = new System.Drawing.Point(464, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(307, 413);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Frames";
            // 
            // listFrames
            // 
            this.listFrames.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listFrames.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.deltaTime,
            this.realTime,
            this.playerCount});
            this.listFrames.FullRowSelect = true;
            this.listFrames.HideSelection = false;
            this.listFrames.Location = new System.Drawing.Point(6, 19);
            this.listFrames.Name = "listFrames";
            this.listFrames.Size = new System.Drawing.Size(295, 388);
            this.listFrames.TabIndex = 0;
            this.listFrames.UseCompatibleStateImageBehavior = false;
            this.listFrames.View = System.Windows.Forms.View.Details;
            this.listFrames.SelectedIndexChanged += new System.EventHandler(this.listFrames_SelectedIndexChanged);
            // 
            // deltaTime
            // 
            this.deltaTime.Text = "Delta Seconds";
            this.deltaTime.Width = 90;
            // 
            // realTime
            // 
            this.realTime.Text = "Real Time";
            this.realTime.Width = 120;
            // 
            // playerCount
            // 
            this.playerCount.Text = "Players";
            this.playerCount.Width = 50;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox3.Controls.Add(this.listPlayers);
            this.groupBox3.Location = new System.Drawing.Point(777, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(317, 413);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Players";
            // 
            // listPlayers
            // 
            this.listPlayers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listPlayers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.name,
            this.team,
            this.frags,
            this.deaths});
            this.listPlayers.FullRowSelect = true;
            this.listPlayers.HideSelection = false;
            this.listPlayers.Location = new System.Drawing.Point(6, 19);
            this.listPlayers.Name = "listPlayers";
            this.listPlayers.Size = new System.Drawing.Size(305, 388);
            this.listPlayers.TabIndex = 0;
            this.listPlayers.UseCompatibleStateImageBehavior = false;
            this.listPlayers.View = System.Windows.Forms.View.Details;
            // 
            // name
            // 
            this.name.Text = "Name";
            this.name.Width = 120;
            // 
            // team
            // 
            this.team.Text = "Team";
            this.team.Width = 70;
            // 
            // frags
            // 
            this.frags.Text = "Frags";
            this.frags.Width = 40;
            // 
            // deaths
            // 
            this.deaths.Text = "Deaths";
            this.deaths.Width = 46;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1106, 437);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listGames;
        private System.Windows.Forms.ColumnHeader startTime;
        private System.Windows.Forms.ColumnHeader length;
        private System.Windows.Forms.ColumnHeader mapName;
        private System.Windows.Forms.ColumnHeader players;
        private System.Windows.Forms.ColumnHeader frames;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListView listFrames;
        private System.Windows.Forms.ColumnHeader deltaTime;
        private System.Windows.Forms.ColumnHeader realTime;
        private System.Windows.Forms.ColumnHeader playerCount;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ListView listPlayers;
        private System.Windows.Forms.ColumnHeader name;
        private System.Windows.Forms.ColumnHeader team;
        private System.Windows.Forms.ColumnHeader frags;
        private System.Windows.Forms.ColumnHeader deaths;
    }
}

