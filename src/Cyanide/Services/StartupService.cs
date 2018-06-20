using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Cyanide
{
    public class StartupService
    {
        private readonly DiscordSocketClient cyanClient;
        private readonly CommandService cyanCommands;
        private readonly IConfigurationRoot cyanConfig;

        public StartupService(  DiscordSocketClient discord ,
                                CommandService commands     ,
                                IConfigurationRoot config   )
        {
            cyanConfig = config;
            cyanClient = discord;
            cyanCommands = commands;
        }

        //Test mode switch for devs only
        internal bool testMode = false;

        public async Task StartAsync()
        {
            if (testMode)
                await cyanClient.LoginAsync(TokenType.Bot, cyanConfig["tokens:discord-test"]);
            else
                await cyanClient.LoginAsync(TokenType.Bot, cyanConfig["tokens:discord"]);

            await cyanClient.StartAsync();

            await cyanCommands.AddModulesAsync(Assembly.GetEntryAssembly());
            await cyanClient.SetGameAsync(cyanConfig["game"]);
        }
    }
}