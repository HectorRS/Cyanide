using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cyanide.Modules
{
    [Name("Purge"), Group("Purge")]
    [Summary("Purges messages from a channel.")]
    [RequireContext(ContextType.Guild)]
    [RequireUserPermission(ChannelPermission.ManageMessages)]
    [RequireBotPermission(ChannelPermission.ManageMessages)]
    public class CyanPurge : CyanModuleBase
    {
        [Command("all")]
        [Summary("Purges all recent messages")]
        public async Task AllAsync(int history = 30)
        {
            var messages = await GetMessageAsync(history + 1);
            await DeleteMessagesAsync(messages);
        }

        [Command("user")]
        [Summary("Purges all recent messages from the specified user")]
        public async Task UserAsync(SocketUser user, int history = 30)
        {
            var cmdmsg = await GetCmdMsgAsync();
            var messages = (await GetMessageAsync(history)).Where(x => x.Author.Id == user.Id);
            await DeleteMessagesAsync(cmdmsg);
            await DeleteMessagesAsync(messages);
        }

        [Command("bots")]
        [Summary("Purges all recent messages made by bots")]
        public async Task BotsAsync(int history = 30)
        {
            var cmdmsg = await GetCmdMsgAsync();
            var messages = (await GetMessageAsync(history)).Where(x => x.Author.IsBot);
            await DeleteMessagesAsync(cmdmsg);
            await DeleteMessagesAsync(messages);
        }

        [Command("filter")]
        [Summary("Purges all recent messages that contain a certain phrase")]
        public async Task FilterAsync(string text, int history = 30)
        {
            var cmdmsg = await GetCmdMsgAsync();
            var messages = (await GetMessageAsync(history)).Where(x => x.Content.ToLower().Contains(text.ToLower()));
            await DeleteMessagesAsync(cmdmsg);
            await DeleteMessagesAsync(messages);
        }

        [Command("attachments")]
        [Summary("Purges all recent messages with attachments")]
        public async Task AttachmentsAsync(int history = 30)
        {
            var cmdmsg = await GetCmdMsgAsync();
            var messages = (await GetMessageAsync(history)).Where(x => x.Attachments.Count() != 0);
            await DeleteMessagesAsync(cmdmsg);
            await DeleteMessagesAsync(messages);
        }

        private Task<IEnumerable<IMessage>> GetMessageAsync(int count)
            => Context.Channel.GetMessagesAsync(count).Flatten();

        private Task<IEnumerable<IMessage>> GetCmdMsgAsync()
            => Context.Channel.GetMessagesAsync(1).Flatten();

        private Task DeleteMessagesAsync(IEnumerable<IMessage> messages)
            => Context.Channel.DeleteMessagesAsync(messages);
    }
}
