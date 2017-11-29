using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System;

namespace Cyanide.Services
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient CyanClient;
        private readonly CommandService CyanCommands;
        private readonly IConfigurationRoot CyanConfig;
        private readonly IServiceProvider CyanServices;

        public CommandHandler(  DiscordSocketClient discord,
                                CommandService commands,
                                IConfigurationRoot config,
                                IServiceProvider provider   )
        {
            CyanClient = discord;
            CyanCommands = commands;
            CyanConfig = config;
            CyanServices = provider;

            CyanClient.MessageReceived += HandleCommandAsync;
        }

        private async Task HandleCommandAsync(SocketMessage MsgParam)
        {
            var message = MsgParam as SocketUserMessage;

            if (message == null)
                return;

            if (message.Author.Id == CyanClient.CurrentUser.Id)
                return;

            var context = new SocketCommandContext(CyanClient, message);

            int argPos = 0;
            if (message.HasStringPrefix(CyanConfig["prefix"], ref argPos))
            {
                var result = await CyanCommands.ExecuteAsync(context, argPos, CyanServices);

                if (!result.IsSuccess)
                    await context.Channel.SendMessageAsync(result.ToString());
            }
        }
    }
}