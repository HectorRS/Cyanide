using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Cyanide.Services;

namespace Cyanide
{
    public class CyanProgram
    {
        public static void Main(string[] args)
            => new CyanProgram().StartAsync().GetAwaiter().GetResult();

        private IConfigurationRoot CyanConfig;
        private DiscordSocketClient CyanClient;
        private CommandService CyanCommands;

        public async Task StartAsync()
        {
            var CyanBuilder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("Configuration.json");

            CyanConfig = CyanBuilder.Build();
           
            CyanClient = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose,
                MessageCacheSize = 100
            });
 
            CyanCommands = new CommandService(new CommandServiceConfig
            {
                DefaultRunMode = RunMode.Async,
                LogLevel = LogSeverity.Verbose
            });

            var services = new ServiceCollection()
                .AddSingleton(CyanClient)
                .AddSingleton(CyanCommands)
                .AddSingleton<CommandHandler>()
                .AddSingleton<LoggingService>()
                .AddSingleton<StartupService>()
                .AddSingleton<Random>()
                .AddSingleton(CyanConfig);

            var CyanProvider = services.BuildServiceProvider();

            CyanProvider.GetRequiredService<LoggingService>();
            await CyanProvider.GetRequiredService<StartupService>().StartAsync();
            CyanProvider.GetRequiredService<CommandHandler>();

            await Task.Delay(-1);
        }
    }
}
/*
    public class CyanConfig
    {
        public string Token { get; set; }
        public string OwnerIDs { get; set; }

        public CyanConfig()
        {
            Token = "";
            OwnerIDs = "";
        }

        public static CyanConfig Load(string dir = "Configuration.json")
        {
            string file = Path.Combine(AppContext.BaseDirectory, dir);
            return JsonConvert.DeserializeObject<CyanConfig>(File.ReadAllText(file));
        }
    }
}
*/