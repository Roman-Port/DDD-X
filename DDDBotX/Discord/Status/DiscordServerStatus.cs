using DDDBotX.Framework.Config;
using DDDBotX.Framework.HistoryDb.DbEntities;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace DDDBotX.Discord.Status
{
    public static class DiscordServerStatus
    {
        private static DiscordMessage updateMessage;
        private static Timer updateTimer;
        
        public static void Init()
        {
            updateTimer = new Timer(Program.config.discord_status_update_seconds * 1000);
            updateTimer.AutoReset = true;
            updateTimer.Elapsed += UpdateTimer_Elapsed;
            updateTimer.Start();

            UpdateStatus();
        }

        private static void UpdateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            UpdateStatus();
        }

        public static async Task UpdateStatus()
        {
            //Create the embed to use
            DiscordEmbed embed = CreateEmbed().Build();

            //Get the message
            var message = await GetUpdateMessage();

            //Update
            await message.ModifyAsync(content:"", embed: embed);
        }

        public static async Task<DiscordMessage> GetUpdateMessage()
        {
            //Try to get cached update message
            if (updateMessage != null)
                return updateMessage;

            //Get the existing message
            var channel = await DiscordBot.discord.GetChannelAsync(Program.config.discord_status_channel);
            DiscordMessage message = null;
            try
            {
                message = await channel.GetMessageAsync(Program.config.discord_status_message);
            } catch
            {
                //Ignore and leave null
            }
            if(message != null)
            {
                updateMessage = message;
                return message;
            }

            //Create a new message
            message = await channel.SendMessageAsync("Bot is setting up, please wait...");
            Program.config.discord_status_message = message.Id;
            Program.SaveConfig();
            updateMessage = message;
            return message;
        }

        public static DiscordEmbedBuilder CreateEmbed()
        {
            DiscordEmbedBuilder builder = new DiscordEmbedBuilder();

            //Set images and footer
            //builder.ThumbnailUrl = Program.config.game_icon_url;
            builder.Footer = new DiscordEmbedBuilder.EmbedFooter
            {
                Text = "Created by RomanPort - DDD-X"
            };

            //Check if server is ready
            if(Program.conn.ready)
            {
                //Add header info
                builder.Title = Program.conn.name;
                builder.Color = DiscordColor.Green;
                builder.AddField("Current Map", Program.conn.map_name, false);

                //Create leaderboard
                int count = BuildScoreboard(builder);

                //Determine amount to fetch from leaderboard
                int leaderboardMax = 12;
                if (count > 0)
                    leaderboardMax = 8;
                if (count > 6)
                    leaderboardMax = 6;
                if (count > 12)
                    leaderboardMax = 4;

                //Fetch leaderboard users
                List<DbPlayer> top = Program.db.FetchTopPlayers(leaderboardMax);
                string topString = "";
                for(int i = 0; i<top.Count; i+=1)
                {
                    string kd = $"KD {MathF.Round(top[i].kd_ratio, 1)}";
                    if (top[i].kd_ratio == 1000) //Default K/D with no deaths
                        kd = "UNSTOPPABLE";
                    topString += $"{GetMedal(i + 1)} {top[i].name} ``{kd}``\n";
                }

                //Add leaderboard
                builder.AddField("Leaderboard - " + String.Format("{0:n0}", Program.db.GetNumberOfPlayers()) + " total", topString, false);

                //Add misc
                builder.ImageUrl = Program.config.game_map_url.Replace("{MAP}", Program.conn.map_name);
            } else
            {
                //Not ready!
                builder.Title = "Not Connected";
                builder.Description = "Server has not connected to the bot yet.";
                builder.Color = DiscordColor.Yellow;
            }

            return builder;
        }

        private static string GetScoreValue(int number)
        {
            if (number > 99)
                return "99";
            return number.ToString().PadLeft(2, ' ');
        }

        private static string GetMedal(int place)
        {
            if (place == 1)
                return ":first_place:";
            if (place == 2)
                return ":second_place:";
            if (place == 3)
                return ":third_place:";
            return ":trophy:";
        }

        private static int BuildScoreboard(DiscordEmbedBuilder builder)
        {
            string teamA = "";
            string teamB = "";
            int teamACount = 0;
            int teamBCount = 0;
            BotConfig_TeamDefinition teamAData = Program.config.teamA;
            BotConfig_TeamDefinition teamBData = Program.config.teamB;
            Program.conn.players.Sort((a, b) =>
            {
                return b.frags.CompareTo(a.frags);
            });
            foreach (var p in Program.conn.players)
            {
                //Get team definition
                BotConfig_TeamDefinition team;
                if (teamAData.id == p.team)
                    team = teamAData;
                else if (teamBData.id == p.team)
                    team = teamBData;
                else
                    continue;

                //Create string
                string name = p.player_name;
                if (p.is_bot)
                    name = "**[BOT]** " + name;
                string value = $"{team.emoji} [``{GetScoreValue(p.frags)}``/``{GetScoreValue(p.deaths)}``] {name}";

                //Add
                if (teamAData.id == p.team)
                {
                    teamACount++;
                    teamA += value + "\n";
                }
                else if (teamBData.id == p.team)
                {
                    teamBCount++;
                    teamB += value + "\n";
                }
            }

            //Check for empty player list
            if (teamA.Length == 0)
                teamA = "*No Players*";
            if (teamB.Length == 0)
                teamB = "*No Players*";

            //Add player list
            builder.AddField(teamAData.name + " - " + teamACount + " players", teamA, true);
            builder.AddField(teamBData.name + " - " + teamBCount + " players", teamB, true);

            return teamACount + teamBCount;
        }
    }
}
