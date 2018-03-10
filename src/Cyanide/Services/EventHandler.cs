using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Cyanide
{
    public class EventHandler
    {
        private readonly DiscordSocketClient cyanClient;
        private readonly CommandService cyanCommands;
        private readonly ConfigManager cyanConfigManager;
        private readonly IServiceProvider cyanProvider;

        public EventHandler(DiscordSocketClient discord,
                                CommandService commands,
                                ConfigManager manager,
                                IServiceProvider provider)
        {
            cyanClient = discord;
            cyanCommands = commands;
            cyanConfigManager = manager;
            cyanProvider = provider;
            
            cyanClient.UserJoined += OnUserJoinedAsync;
            cyanClient.UserLeft += OnUserLeftAsync;
        }

        public async Task OnUserJoinedAsync(SocketGuildUser user)
        {
            ulong channelId = await cyanConfigManager.GetUserIOChannelIdAsync(user.Guild.Id);
            var channel = cyanClient.GetChannel(channelId) as SocketTextChannel;
            var builder = new EmbedBuilder()
                .WithColor(0, 255, 255)
                .WithDescription($"User **{user.Username}#{user.Discriminator}** has joined the server.");

            if (channelId != 0 || channel != null)
            {
                await channel.SendMessageAsync("", false, builder);
            }
            else return;
        }

        public async Task OnUserLeftAsync(SocketGuildUser user)
        {
            ulong channelId = await cyanConfigManager.GetUserIOChannelIdAsync(user.Guild.Id);
            var channel = cyanClient.GetChannel(channelId) as SocketTextChannel;
            var builder = new EmbedBuilder()
                .WithColor(0, 255, 255)
                .WithDescription($"User **{user.Username}#{user.Discriminator}** has left the server.");

            if (channelId != 0 || channel != null)
            {
                await channel.SendMessageAsync("", false, builder);
            }
            else return;
        }
    }
}