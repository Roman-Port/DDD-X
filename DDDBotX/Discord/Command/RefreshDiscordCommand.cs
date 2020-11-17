using DDDBotX.Discord.Status;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DDDBotX.Discord.Command
{
    public class RefreshDiscordCommand : IDiscordCommand
    {
        public override async Task Execute(MessageCreateEventArgs e, string cmd, string args)
        {
            //Send message displaying status
            var msg = await e.Message.RespondAsync(embed: new DiscordEmbedBuilder().WithTitle("Updating Message...").WithColor(DiscordColor.Grayple).WithDescription("Please wait..."));

            //Update
            try
            {
                await DiscordServerStatus.UpdateStatus();
                await msg.ModifyAsync(embed: new DiscordEmbedBuilder().WithTitle("Successfully Updated!").WithColor(DiscordColor.Green).WithDescription("Updated the message in server status."));
            } catch (Exception ex)
            {
                await msg.ModifyAsync(embed: new DiscordEmbedBuilder().WithTitle("Error Updating").WithColor(DiscordColor.Green).WithDescription(ex.Message).AddField("Stack Trace", ex.StackTrace));
            }
        }

        public override string[] GetCommandNames()
        {
            return new string[]
            {
                "forceupdate"
            };
        }

        public override string GetHelpText()
        {
            return "Forces the server status message to be updated.";
        }

        public override string GetHelpTitle()
        {
            return "Force Update";
        }
    }
}
