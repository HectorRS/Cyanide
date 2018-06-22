using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace Cyanide.Modules
{
    [Name("Miscellaneous")]
    [Summary("Random, practically useless commands.")]
    public class CyanMisc : CyanModuleBase
    {
        [Command("echo")]
        [Summary("Echoes a message.")]
        public async Task EchoAsync([Remainder] string echo)
        {
            var message = await Context.Channel.GetMessagesAsync(1).Flatten();
            await Context.Channel.DeleteMessagesAsync(message);

            await ReplyAsync(echo);
        }

        [Command("github")]
        [Summary("Replies with the link to Cyanide's Github.")]
        public async Task GithubAsync()
        {
            await ReplyEmbedAsync(0, null, "https://github.com/HectorRS/Cyanide", "https://github.com/HectorRS/Cyanide");
        }
    }
}
