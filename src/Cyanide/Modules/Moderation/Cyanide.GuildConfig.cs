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
        [Summary("Check what prefix this guild has configured.")]
        public async Task PrefixAsync()
        {
            var config = await cyanManager.GetOrCreateConfigAsync(Context.Guild.Id);

            if (config.Prefix == null)
                await ReplyAsync($"This guild's prefix is {Context.Client.CurrentUser.Mention}");
            else
                await ReplyAsync($"This guild's prefix is `{config.Prefix}`");
        }

        [Command("setprefix")]
        [Summary("Change or remove this guild's string prefix.")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SetPrefixAsync([Remainder]string prefix)
        {
            var config = await cyanManager.GetOrCreateConfigAsync(Context.Guild.Id);
            await cyanManager.SetPrefixAsync(config, prefix);

            await ReplyAsync($"This guild's prefix is now `{prefix}`");
        }
    }
}