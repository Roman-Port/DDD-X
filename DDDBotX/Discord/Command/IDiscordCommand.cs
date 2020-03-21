using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DDDBotX.Discord.Command
{
    public abstract class IDiscordCommand
    {
        public abstract string GetHelpTitle();
        public abstract string[] GetCommandNames();
        public abstract string GetHelpText();
        public abstract Task Execute(MessageCreateEventArgs e, string cmd, string args);
    }
}
