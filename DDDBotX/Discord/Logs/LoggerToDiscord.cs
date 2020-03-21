using DDDBotX.Framework;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DDDBotX.Discord.Logs
{
    public static class LoggerToDiscord
    {
        public static DiscordChannel loggingChannel;
        
        public static async Task CreatePlayerLogEvent(DDDOnlinePlayer player, DiscordColor color, string title, string content)
        {
            //Build
            DiscordEmbedBuilder builder = new DiscordEmbedBuilder();
            builder.Color = color;
            string steamProfile = "https://steamcommunity.com/profiles/" + player.steam_id;
            if (player.HasSteamInfo())
            {
                var info = await player.GetSteamUser();
                builder.Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    IconUrl = info.avatarfull,
                    Url = steamProfile,
                    Name = player.player_name
                };
            } else
            {
                builder.Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    Url = steamProfile,
                    Name = player.player_name
                };
            }
            builder.Description = $"**{title}**\n{content}";
            builder.Footer = new DiscordEmbedBuilder.EmbedFooter
            {
                Text = "STEAM ID: " + player.steam_id
            };
            builder.Timestamp = DateTime.UtcNow;

            //Send
            await SendEmbedMessage(builder);
        }

        private static async Task<DiscordMessage> SendEmbedMessage(DiscordEmbed embed)
        {
            //Get the channel
            if(loggingChannel == null)
            {
                loggingChannel = await DiscordBot.discord.GetChannelAsync(Program.config.discord_logs_channel);
            }

            //Send
            return await loggingChannel.SendMessageAsync(embed: embed);
        }
    }
}
