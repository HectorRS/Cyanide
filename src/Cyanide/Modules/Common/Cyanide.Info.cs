using Discord.Commands;
using System.Threading.Tasks;

namespace Cyanide.Modules
{
    [Name("Info")]
    [Group("Info")]
    [Summary("Gives info.")]
    public class CyanGeneralInfo : CyanModuleBase
    {
        [Command("owner")]
        [Summary("Gives bot owner's discord tag.")]
        public async Task OwnerInfoAsync()
        {
            await ReplyAsync("My owner/useless developer is Scarlett Azure#9098." + "\n" + "Careful where you tread around him, 'cause he's always depressed and will not hesitate to hang himself.");
        }

        [Command("bot")]
        [Summary("Gives Cyanide's info.")]
        public async Task BotInfoAsync()
        {
            await ReplyAsync("I'm Cyanide, a small-scale private bot designed to automate what my owner's too lazy to do.");
        }
    }
}