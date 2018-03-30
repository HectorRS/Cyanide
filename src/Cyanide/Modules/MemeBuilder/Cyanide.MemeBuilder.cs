using System.Threading.Tasks;
using Discord.Commands;

namespace Cyanide.Modules
{
    [Name("Meme Builder")]
    [Summary("Creates memes with user-inputted texts.")]
    public class CyanMemeBuilder : CyanModuleBase
    {
        private readonly MemeBuilderService cyanMB;

        public CyanMemeBuilder(MemeBuilderService mBService)
        {
            cyanMB = mBService;
        }

        [Command("honest")]
        [Summary("\"You're not being honest, [text].\"")]
        public async Task MbHonestAsync([Remainder] string str)
        {
            await ReplyAsync(cyanMB.BuildMemeHonest(str), "c-r-m-Honest.png");
        }

        [Command("aslongas"), Alias("ala")]
        [Summary("\"No matter if you're a boy or a girl, it's all okay as long as [text]!\"")]
        public async Task MbAsLongAsAsync([Remainder] string str)
        {
            await ReplyAsync(cyanMB.BuildMemeAsLongAs(str), "c-r-m-ALA.png");
        }
    }
}
