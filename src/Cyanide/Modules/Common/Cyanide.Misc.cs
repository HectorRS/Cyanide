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
    }
}
