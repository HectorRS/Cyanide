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

        private IConfigurationRoot cyanConfig;
        private DiscordSocketClient cyanClient;
        private CommandService cyanCommands;

        public async Task StartAsync()
        {
            var cyanBuilder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory + @"/Config")
                .AddJsonFile("Configuration.json");

            cyanConfig = cyanBuilder.Build();
           
            cyanClient = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose,
                MessageCacheSize = 100
            });
 
            cyanCommands = new CommandService(new CommandServiceConfig
            {
                DefaultRunMode = RunMode.Async,
                LogLevel = LogSeverity.Verbose
            });

            var cyanServices = new ServiceCollection()
                .AddSingleton(cyanClient)
                .AddSingleton(cyanCommands)
                .AddSingleton<CommandHandler>()
                .AddSingleton<LoggingService>()
                .AddSingleton<StartupService>()
                .AddSingleton<Random>()
                .AddSingleton(cyanConfig);

            var cyanProvider = cyanServices.BuildServiceProvider();

            cyanProvider.GetRequiredService<LoggingService>();
            await cyanProvider.GetRequiredService<StartupService>().StartAsync();
            cyanProvider.GetRequiredService<CommandHandler>();

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