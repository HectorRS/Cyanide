using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Cyanide
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient cyanClient;
        private readonly CommandService cyanCommands;
        private readonly LoggingService cyanLogger;
        private readonly ConfigManager cyanManager;
        private readonly IServiceProvider cyanProvider;
        private readonly IConfigurationRoot cyanConfig;

        public CommandHandler(  DiscordSocketClient discord ,
                                CommandService commands     ,
                                LoggingService logger       ,
                                ConfigManager manager       ,
                                IServiceProvider provider   ,
                                IConfigurationRoot config   )
        {
            cyanClient = discord;
            cyanCommands = commands;
            cyanLogger = logger;
            cyanManager = manager;
            cyanProvider = provider;
            cyanConfig = config;

            cyanClient.MessageReceived += OnMessageReceivedAsync;
        }

        private async Task OnMessageReceivedAsync(SocketMessage MsgParam)
        {
            var msg = MsgParam as SocketUserMessage;
            if (msg == null)
                return;

            var context = new CyanCommandContext(cyanClient, msg);
            string prefix = await cyanManager.GetPrefixAsync(context.Guild.Id);

            int argPos = 0;
            bool hasStringPrefix = prefix == null ? false : msg.HasStringPrefix(prefix, ref argPos);

            if  (  hasStringPrefix
                || msg.HasStringPrefix(cyanConfig["globalprefix"], ref argPos))
                using (context.Channel.EnterTypingState())
                    await ExecuteAsync(context, cyanProvider, argPos);
        }

        public async Task ExecuteAsync(CyanCommandContext context, IServiceProvider provider, int argPos)
        {
            var result = await cyanCommands.ExecuteAsync(context, argPos, provider);
            await ResultAsync(context, result);
        }

        public async Task ExecuteAsync(CyanCommandContext context, IServiceProvider provider, string input)
        {
            var result = await cyanCommands.ExecuteAsync(context, input, provider);
            await ResultAsync(context, result);
        }

        private async Task ResultAsync(CyanCommandContext context, IResult result)
        {
            if (result.IsSuccess)
                return;

            switch (result)
            {
                case ExecuteResult exr:
                    await cyanLogger.LogAsync(LogSeverity.Error, "Commands", exr.Exception?.ToString() ?? exr.ErrorReason);
                    break;
                default:
                    await context.Channel.SendMessageAsync(result.ErrorReason);
                    break;
            }
        }
    }
}

    /*public class CommandHandler
    {
        private readonly DiscordSocketClient cyanClient;
        private readonly CommandService cyanCommands;
        private readonly LoggingService cyanLogger;
        private readonly IServiceProvider cyanServices;

        public CommandHandler
            (
            DiscordSocketClient discord,
            CommandService commands,
            LoggingService logger,
            IServiceProvider provider
            )
        {
            cyanClient = discord;
            cyanCommands = commands;
            CyanConfig = config;
            cyanServices = provider;

            cyanClient.MessageReceived += HandleCommandAsync;
        }

        private async Task HandleCommandAsync(SocketMessage MsgParam)
        {
            var message = MsgParam as SocketUserMessage;

            if (message == null)
                return;

            if (message.Author.Id == cyanClient.CurrentUser.Id)
                return;

            var context = new SocketCommandContext(cyanClient, message);

            int argPos = 0;
            if (message.HasStringPrefix(CyanConfig["prefix"], ref argPos))
            {
                var result = await cyanCommands.ExecuteAsync(context, argPos, cyanServices);

                if (!result.IsSuccess)
                    await context.Channel.SendMessageAsync(result.ToString());
            }
        }
    }*/