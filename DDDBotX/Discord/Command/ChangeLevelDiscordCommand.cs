using DDDBotX.Framework;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDDBotX.Discord.Command
{
    public class ChangeLevelDiscordCommand : IDiscordCommand
    {
        public override async Task Execute(MessageCreateEventArgs e, string cmd, string args)
        {
            //Get map and gamemode
            if(args.Split(' ').Length != 2)
            {
                await e.Message.RespondAsync("**INVALID USAGE**\nValid usage is ``changelevel {map} {gamemode}``");
                return;
            }
            string map = args.Split(' ')[0];
            string mode = args.Split(' ')[1];

            //Make sure map is valid
            if (!Program.config.valid_maps.Contains(map))
            {
                await e.Message.RespondAsync($"**INVALID MAP**\n\"{map}\" is not a valid map.");
                return;
            }

            //Make sure mode is valid
            if (!Program.config.valid_modes.Contains(mode))
            {
                await e.Message.RespondAsync($"**INVALID GAMEMODE**\n\"{mode}\" is not a valid gamemode.");
                return;
            }

            //Run
            await RCONTool.RunRCONCommandDiscord(e.Channel, $"changelevel {map} {mode}");
        }

        public override string[] GetCommandNames()
        {
            return new string[]
            {
                "changelevel",
                "changemap"
            };
        }

        public override string GetHelpText()
        {
            return "Changes the map to specified gamemode and map. USAGE: {map} {gamemode}";
        }

        public override string GetHelpTitle()
        {
            return "Change Level";
        }
    }
}
