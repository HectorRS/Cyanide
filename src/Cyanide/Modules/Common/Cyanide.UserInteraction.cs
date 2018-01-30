using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace Cyanide.Modules
{
    [Name("User Interaction")]
    [Summary("Various commands to interact with users.")]
    public class CyanUserInteraction : CyanModuleBase
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
