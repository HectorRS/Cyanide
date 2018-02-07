using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Cyanide
{
    public class ConfigManager : DbManager<ConfigDatabase>
    {
        public ConfigManager(ConfigDatabase db)
            : base(db) { }

        public Task<GuildConfig> GetConfigAsync(ulong guildId)
            => _db.GuildConfigs.SingleOrDefaultAsync(x => x.GuildId == guildId);

        public async Task<GuildConfig> GetOrCreateConfigAsync(ulong guildId)
        {
            var config = await GetConfigAsync(guildId);
            if (config != null)
                return config;

            await CreateAsync(new GuildConfig
            {
                GuildId = guildId
            });

            return await GetConfigAsync(guildId);
        }

        public async Task<string> GetPrefixAsync(ulong guildId)
            => (await GetOrCreateConfigAsync(guildId)).Prefix;

        public async Task SetPrefixAsync(ulong guildId, string prefix)
        {
            var config = await GetConfigAsync(guildId);
            await SetPrefixAsync(config, prefix);
        }

        public async Task SetPrefixAsync(GuildConfig config, string prefix)
        {
            if (string.IsNullOrWhiteSpace(prefix))
                config.Prefix = null;
            else
                config.Prefix = prefix;

            _db.GuildConfigs.Update(config);
            await _db.SaveChangesAsync();
        }

        public async Task<ulong> GetUserIOChannelIdAsync(ulong guildId)
            => (await GetOrCreateConfigAsync(guildId)).UserIOLogChannelId;

        public async Task SetUserIOChannelIdAsync(ulong guildId, ulong userIOLogChannelId)
        {
            var config = await GetConfigAsync(guildId);
            await SetUserIOLogChannelIdAsync(config, userIOLogChannelId);
        }

        public async Task SetUserIOLogChannelIdAsync(GuildConfig config, ulong userIOLogChannelId)
        {
            config.UserIOLogChannelId = userIOLogChannelId;
            _db.GuildConfigs.Update(config);
            await _db.SaveChangesAsync();
        }

        // Create
        public async Task CreateAsync(GuildConfig guildConfig)
        {
            await _db.GuildConfigs.AddAsync(guildConfig);
            await _db.SaveChangesAsync();
        }
    }
}