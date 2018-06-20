using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Cyanide.Modules
{
    [Name("Config")]
    [Summary("Bot configuration options.")]
    public class CyanGuildConifg : CyanModuleBase
    {
        private readonly ConfigManager cyanManager;
        private readonly IConfigurationRoot cyanConfig;

        public CyanGuildConifg(ConfigManager manager,
                               IConfigurationRoot config)
        {
            cyanManager = manager;
            cyanConfig = config;
        }

        [Command("prefix")]
        [Summary("Check what prefix this server has configured.")]
        public async Task PrefixAsync()
        {
            var config = await cyanManager.GetOrCreateConfigAsync(Context.Guild.Id);

            if (config.Prefix == null)
                await ReplyEmbedAsync(3, "Prefix Configurator", $"Server's current prefix: `{cyanConfig["globalprefix"]}`");
            else
                await ReplyEmbedAsync(3, "Prefix Configurator", $"Server's current prefix: `{config.Prefix}`");
        }

        [Command("setprefix")]
        [Summary("Change or remove this server's string prefix.")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SetPrefixAsync([Remainder]string prefix)
        {
            var config = await cyanManager.GetOrCreateConfigAsync(Context.Guild.Id);
            await cyanManager.SetPrefixAsync(config, prefix);

            await ReplyEmbedAsync(3, "Prefix Configurator", $"Server's prefix has been set to `{prefix}`");
        }

        [Command("gatelog")]
        [Summary("Check what gatelog channel this server has configured.")]
        [RequireUserPermission(GuildPermission.ManageChannels)]
        public async Task GatelogAsync()
        {
            var config = await cyanManager.GetOrCreateConfigAsync(Context.Guild.Id);

            if (config.UserIOLogChannelId == 0)
                await ReplyEmbedAsync(3, "Gatelog Configurator", "Server has not set the channel to log.");
            else
                await ReplyEmbedAsync(3, "Gatelog Configurator", $"Server's current gatelog channel ID: `{config.UserIOLogChannelId}`");
        }

        [Command("setgatelog")]
        [Summary("Change this server's gatelog channel ID. Set value to 0 to disable logging.")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SetGatelogAsync([Remainder]ulong userIOLogChannelId)
        {
            var config = await cyanManager.GetOrCreateConfigAsync(Context.Guild.Id);
            await cyanManager.SetUserIOChannelIdAsync(config, userIOLogChannelId);

            await ReplyEmbedAsync(3, "Gatelog Configurator", $"Server's gatelog channel ID has been changed to `{userIOLogChannelId}`");
        }
    }
}