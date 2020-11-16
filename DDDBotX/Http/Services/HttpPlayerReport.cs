using DSharpPlus.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DDDBotX.Http.Services
{
    public static class HttpPlayerReport
    {
        public const string REPORT_SITE = "https://romanport.com/p/dinotown/player-report/";
        public const string REPORT_HISTORY_SINCE = "9/13/2020";
        public const ulong CHANNEL_PUBLIC = 754788141075791934;
        public const ulong CHANNEL_PRIVATE = 279679570670518272;
        
        public static async Task OnHTTPRequest(HttpContext e)
        {
            //Decode
            RequestData request = await DDDHttpServer.DecodePOSTBody<RequestData>(e);

            //Create public embed
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder();
            embed.Title = "Player Report";
            embed.Description = $"This player was reported by another player. You can submit your own reports [here]({REPORT_SITE}).";
            
            //Add player details
            embed.ThumbnailUrl = request.target_player.icon;
            embed.AddField("Reported Player Name", request.target_player.name);
            embed.AddField("Reported Player Steam ID", request.target_player.steam_id);

            //Try to find how many times they've been reported before
            var p = Program.db.GetDbPlayerBySteamId(ulong.Parse(request.target_player.steam_id));
            if(p != null)
            {
                embed.AddField("Previous Reports", $"{p.player_reports} (since {REPORT_HISTORY_SINCE})");
                p.player_reports++;
                Program.db.UpdateDbPlayer(p);
            } else
            {
                embed.AddField("Previous Reports", $"0 (since {REPORT_HISTORY_SINCE})");
            }

            //Add type
            ReportTypeData reportType = new ReportTypeData
            {
                color = new DiscordColor("4f5157"),
                display_name = "Other"
            };
            if (reportTypes.ContainsKey(request.type))
                reportType = reportTypes[request.type];
            embed.AddField("Incident Type", reportType.display_name);
            embed.Color = reportType.color;

            //Add media
            string media = "";
            foreach (var m in request.media)
                media += "<" + m + ">\n";
            if(media == "")
                embed.AddField("Media", "(no media provided)");
            else
                embed.AddField("Media", media);

            //Add description
            embed.AddField("Description", request.description);

            //Send to the reports channel
            await SendMessageToChannel(CHANNEL_PUBLIC, embed);

            //Send private info and send to private channel
            embed.AddField("(PRIVATE) Submitter Discord Username", request.discord);
            await SendMessageToChannel(CHANNEL_PRIVATE, embed);

            //Return OK
            await DDDHttpServer.WriteJSONToBody(e, new ResponseData
            {
                ok = true
            });
        }

        private static async Task SendMessageToChannel(ulong id, DiscordEmbedBuilder embed)
        {
            var discord = Discord.DiscordBot.discord;
            var channel = await discord.GetChannelAsync(id);
            await channel.SendMessageAsync(embed: embed);
        }

        public static readonly Dictionary<string, ReportTypeData> reportTypes = new Dictionary<string, ReportTypeData>()
        {
            {"hack", new ReportTypeData
                {
                    color = new DiscordColor("f5432c"),
                    display_name = "Hacking"
                }
            },
            {"exploit", new ReportTypeData
                {
                    color = new DiscordColor("f5b92c"),
                    display_name = "Using Unintended Exploits"
                }
            },
            {"team", new ReportTypeData
                {
                    color = new DiscordColor("2cf5ab"),
                    display_name = "Force Teamswitching"
                }
            },
            {"relog", new ReportTypeData
                {
                    color = new DiscordColor("582cf5"),
                    display_name = "Malicious Relogging"
                }
            }
        };

        public class ReportTypeData
        {
            public DiscordColor color;
            public string display_name;
        }

        class RequestData
        {
            public RequestDataPlayer target_player;
            public string[] media;
            public string discord;
            public string type;
            public string description;
        }

        class RequestDataPlayer
        {
            public string name;
            public string icon;
            public string steam_id;
        }

        class ResponseData
        {
            public bool ok;
        }
    }
}
