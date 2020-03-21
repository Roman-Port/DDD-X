using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DDDBotX.Framework
{
    public static class RCONTool
    {
        public static async Task<RCONResponseData> RunRCONCommand(string cmd)
        {
            try
            {
                //Create an RCON connection
                DDDRconClient.RconClient rc = new DDDRconClient.RconClient();

                //Connect
                bool connected = await rc.ConnectAsync(new IPEndPoint(IPAddress.Parse(Program.config.game_ip), Program.config.game_port));
                if (!connected)
                    return new RCONResponseData("Couldn't connect to server", false);

                //Authenticate
                bool authenticated = await rc.AuthenticateAsync(Program.config.rcon_password);
                if (!authenticated)
                    return new RCONResponseData("Authentication failed", false);

                //Send
                string response = await rc.ExecuteCommandAsync(cmd);

                //Close
                rc.sock.Close();

                //Finish
                return new RCONResponseData(response, true);
            } catch (Exception ex)
            {
                return new RCONResponseData("Unhandled exception!", false);
            }
        }

        public static async Task<RCONResponseData> RunRCONCommandDiscord(DiscordChannel channel, string cmd)
        {
            //Create message embed
            DiscordEmbedBuilder workingEmbed = new DiscordEmbedBuilder();
            workingEmbed.Color = new DiscordColor(56, 130, 220);
            workingEmbed.Title = "Working...";
            workingEmbed.Description = "Connecting to server, please wait...";
            workingEmbed.Footer = new DiscordEmbedBuilder.EmbedFooter
            {
                IconUrl = "https://romanport.com/static/SourceBot/rcon_loader.gif",
                Text = $"{Program.config.game_ip}:{Program.config.game_port}"
            };

            //Send message
            DiscordMessage m = await channel.SendMessageAsync(embed: workingEmbed);

            //Send RCON
            DateTime start = DateTime.UtcNow;
            var response = await RunRCONCommand(cmd);
            DateTime end = DateTime.UtcNow;

            //Create response
            workingEmbed = new DiscordEmbedBuilder();
            workingEmbed.Footer = new DiscordEmbedBuilder.EmbedFooter
            {
                Text = $"{Program.config.game_ip}:{Program.config.game_port} - in {Math.Round((end - start).TotalMilliseconds)} ms"
            };
            if(response.ok)
            {
                workingEmbed.Title = "RCON Command Result";
                workingEmbed.Color = DiscordColor.Green;
                workingEmbed.Description = response.text;
            } else
            {
                workingEmbed.Title = "RCON Error";
                workingEmbed.Color = DiscordColor.Red;
                workingEmbed.Description = response.text;
            }

            //Update
            await m.ModifyAsync(embed: workingEmbed);
            return response;
        }
    }

    public struct RCONResponseData
    {
        public string text;
        public bool ok;

        public RCONResponseData(string text, bool ok)
        {
            this.text = text;
            this.ok = ok;
        }
    }
}
