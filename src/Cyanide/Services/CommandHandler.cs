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
        private readonly CommandService      cyanCommands;
        private readonly LoggingService      cyanLogger;
        private readonly ConfigManager       cyanManager;
        private readonly IServiceProvider    cyanProvider;
        private readonly IConfigurationRoot  cyanConfig;

        public CommandHandler(DiscordSocketClient discord,
                              CommandService      commands,
                              LoggingService      logger,
                              ConfigManager       manager,
                              IServiceProvider    provider,
                              IConfigurationRoot  config)
        {
            cyanClient   = discord;
            cyanCommands = commands;
            cyanLogger   = logger;
            cyanManager  = manager;
            cyanProvider = provider;
            cyanConfig   = config;

            cyanClient.MessageReceived += OnMessageReceivedAsync;
        }

        private async Task OnMessageReceivedAsync(SocketMessage MsgParam)
        {
            var msg = MsgParam as SocketUserMessage;
            if (msg == null) return;
            var context = new CyanCommandContext(cyanClient, msg);

            int argPos = 0;
            string prefix = cyanConfig["globalprefix"];
            
            if (context.Guild != null)
            {
                prefix = await cyanManager.GetPrefixAsync(context.Guild.Id);
            }

            bool hasStringPrefix = prefix == null ? false : msg.HasStringPrefix(prefix, ref argPos);

            if (hasStringPrefix || msg.HasStringPrefix(cyanConfig["globalprefix"], ref argPos))
            {
                var ts = context.Channel.EnterTypingState();
                await ExecuteAsync(context, cyanProvider, argPos);
                ts.Dispose();
                GC.Collect();
            }
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