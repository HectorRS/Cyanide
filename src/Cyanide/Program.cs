using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Cyanide
{
    public class CyanProgram
    {
        public static void Main(string[] args)
            => new CyanProgram().StartAsync().GetAwaiter().GetResult();

        private IConfigurationRoot cyanConfig;

        public async Task StartAsync()
        {
            ConsoleCyanifier.NewLine($"Cyanide v.{AppHelper.Version}");
            ConsoleCyanifier.NewLine();

            var cyanBuilder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory + @"/Config")
                .AddJsonFile("Configuration.json");
            cyanConfig = cyanBuilder.Build();

            var cyanServices = new ServiceCollection()
                .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
                {
                    LogLevel = LogSeverity.Verbose,
                    MessageCacheSize = 100
                }))
                .AddSingleton(new CommandService(new CommandServiceConfig
                {
                    DefaultRunMode = RunMode.Async,
                    LogLevel = LogSeverity.Verbose
                }))
                .AddDbContext<ConfigDatabase>(ServiceLifetime.Transient)
                .AddTransient<ConfigManager>()
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