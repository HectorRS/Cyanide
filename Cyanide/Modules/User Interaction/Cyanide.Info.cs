using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Cyanide.Modules.User_Interaction
{
    [Group("info")]
    public class CyanInfo : ModuleBase<SocketCommandContext>
    {
        [Command("owner")]
        [Summary("Gives bot owner's discord tag.")]
        public async Task OwnerInfoAsync()
        {
            await ReplyAsync("My owner/useless developer is Scarlett Azure#9098. Careful where you tread around him, 'cause he's always depressed and will not hesitate to hang himself.");
        }
        [Command("bot")]
        [Summary("Gives Cyanide's info.")]
        public async Task BotInfoAsync()
        {
            await ReplyAsync("I'm Cyanide, a small-scale private bot designed to automate what my owner's too lazy to do.");
        }
    }
}