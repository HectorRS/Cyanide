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
        private readonly DiscordSocketClient CyanClient;
        private readonly CommandService CyanCommands;
        private readonly IConfigurationRoot CyanConfig;

        public StartupService(  DiscordSocketClient discord,
                                CommandService commands,
                                IConfigurationRoot config   )
        {
            CyanConfig = config;
            CyanClient = discord;
            CyanCommands = commands;
        }

        public async Task StartAsync()
        {
            string Token = CyanConfig["tokens:discord"];
            if (string.IsNullOrWhiteSpace(Token))
                throw new Exception("Token missing.");

            await CyanClient.LoginAsync(TokenType.Bot, Token);
            await CyanClient.StartAsync();

            await CyanCommands.AddModulesAsync(Assembly.GetEntryAssembly());
        }
    }
}