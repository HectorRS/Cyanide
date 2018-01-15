using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Cyanide.Services
{
    public class StartupService
    {
        private readonly DiscordSocketClient cyanClient;
        private readonly CommandService cyanCommands;
        private readonly IConfigurationRoot cyanConfig;

        public StartupService(  DiscordSocketClient discord,
                                CommandService commands,
                                IConfigurationRoot config   )
        {
            cyanConfig = config;
            cyanClient = discord;
            cyanCommands = commands;
        }

        public async Task StartAsync()
        {
            string Token = cyanConfig["tokens:discord"];
            if (string.IsNullOrWhiteSpace(Token))
                throw new Exception("Token missing.");

            await cyanClient.LoginAsync(TokenType.Bot, Token);
            await cyanClient.StartAsync();

            await cyanCommands.AddModulesAsync(Assembly.GetEntryAssembly());
        }
    }
}