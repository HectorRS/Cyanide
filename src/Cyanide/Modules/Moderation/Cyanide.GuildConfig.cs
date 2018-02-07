using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace Cyanide.Modules
{
    [Name("Config")]
    [Summary("Bot configuration options.")]
    public class CyanGuildConifg : CyanModuleBase
    {
        private readonly ConfigManager cyanManager;

        public CyanGuildConifg(ConfigManager manager)
        {
            cyanManager = manager;
        }

        [Command("prefix")]
        [Summary("Check what prefix this server has configured.")]
        public async Task PrefixAsync()
        {
            var config = await cyanManager.GetOrCreateConfigAsync(Context.Guild.Id);

            if (config.Prefix == null)
                await ReplyAsync($"This server's prefix is {Context.Client.CurrentUser.Mention}");
            else
                await ReplyAsync($"This server's prefix is `{config.Prefix}`");
        }

        [Command("setprefix")]
        [Summary("Change or remove this server's string prefix.")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SetPrefixAsync([Remainder]string prefix)
        {
            var config = await cyanManager.GetOrCreateConfigAsync(Context.Guild.Id);
            await cyanManager.SetPrefixAsync(config, prefix);

            await ReplyAsync($"This server's prefix is now `{prefix}`");
        }

        [Command("userlogchannel")]
        [Summary("Check what user logging channel this server has configured.")]
        [RequireUserPermission(GuildPermission.ManageChannels)]
        public async Task UserIOLogChannelIdAsync()
        {
            var config = await cyanManager.GetOrCreateConfigAsync(Context.Guild.Id);

            if (config.UserIOLogChannelId == 0)
                await ReplyAsync($"This server has not set the channel to log in.");
            else
                await ReplyAsync($"This server's log channel ID is `{config.UserIOLogChannelId}`");
        }

        [Command("setuserlogchannel")]
        [Summary("Change this server's log channel ID. Set value to 0 to disable logging.")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SetUserIOLogChannelIdAsync([Remainder]ulong userIOLogChannelId)
        {
            var config = await cyanManager.GetOrCreateConfigAsync(Context.Guild.Id);
            await cyanManager.SetUserIOLogChannelIdAsync(config, userIOLogChannelId);

            await ReplyAsync($"This server's log channel ID is now `{userIOLogChannelId}`");
        }
    }
}