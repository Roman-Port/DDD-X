using DDDBotX.Framework;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DDDBotX.Discord.Command
{
    public class SetFragLimitCommand : IDiscordCommand
    {
        public override async Task Execute(MessageCreateEventArgs e, string cmd, string args)
        {
            //Get map and gamemode
            if (args.Split(' ').Length != 1)
            {
                await e.Message.RespondAsync("**INVALID USAGE**\nValid usage is ``frags {limit}``");
                return;
            }
            string fragCountNum = args.Split(' ')[0];

            //Parse
            if(!int.TryParse(fragCountNum, out int fragCount))
            {
                await e.Message.RespondAsync("**INVALID USAGE**\nValid usage is ``frags {limit}``");
                return;
            }

            //Validate
            if(fragCount < 1 || fragCount >= short.MaxValue)
            {
                await e.Message.RespondAsync("**INVALID USAGE**\nThe number of frags you specified were out of bounds.");
                return;
            }

            //Run
            await RCONTool.RunRCONCommandDiscord(e.Channel, $"sm_cvar ddd_dm_frags {fragCount}");
        }

        public override string[] GetCommandNames()
        {
            return new string[]
            {
                "frags",
                "fraglimit",
                "limit"
            };
        }

        public override string GetHelpText()
        {
            return "Changes the frag limit. Specify a number of frags.";
        }

        public override string GetHelpTitle()
        {
            return "Change Frag Limit";
        }
    }
}
