using DDDBotX.Framework;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DDDBotX.Discord.Command
{
    public class RconDiscordCommand : IDiscordCommand
    {
        public override async Task Execute(MessageCreateEventArgs e, string cmd, string args)
        {
            await RCONTool.RunRCONCommandDiscord(e.Channel, args);
        }

        public override string[] GetCommandNames()
        {
            return new string[]
            {
                "rc",
                "rcon"
            };
        }

        public override string GetHelpText()
        {
            return "Executes a raw rcon command.";
        }

        public override string GetHelpTitle()
        {
            return "RCON Command";
        }
    }
}
