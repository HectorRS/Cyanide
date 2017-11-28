using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace Cyanide
{ 
    public class CyanProgram
    {
        private DiscordSocketClient CyanClient;
        private CommandService CyanCommands;
        private IServiceProvider CyanServices;

        public static void Main(string[] args)
            => new CyanProgram().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            CyanClient = new DiscordSocketClient();
            CyanCommands = new CommandService();
            CyanServices = new ServiceCollection()
                .AddSingleton(CyanClient)
                .AddSingleton(CyanCommands)
                .BuildServiceProvider();

            CyanClient.Log += CyanLog;

            await InstallCommandsAsync();

            await CyanClient.LoginAsync(TokenType.Bot, CyanConfig.Load().Token);
            await CyanClient.StartAsync();
            await Task.Delay(-1);
        }

        public async Task InstallCommandsAsync()
        {
            CyanClient.MessageReceived += HandleCommandAsync;
            await CyanCommands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;

            if (message == null)
                return;

            int argPos = 0;

            if (!(message.HasStringPrefix("Cyan ", ref argPos) || message.HasMentionPrefix(CyanClient.CurrentUser, ref argPos)))
                return;

            var context = new SocketCommandContext(CyanClient, message);

            var result = await CyanCommands.ExecuteAsync(context, argPos, CyanServices);

            if (!result.IsSuccess)
                await context.Channel.SendMessageAsync(result.ErrorReason);
        }

        private Task CyanLog(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }

    public class CyanConfig
    {
        public string Token { get; set; }

        public CyanConfig()
        {
            Token = "";
        }

        public static CyanConfig Load(string dir = "Configuration.json")
        {
            string file = Path.Combine(AppContext.BaseDirectory, dir);
            return JsonConvert.DeserializeObject<CyanConfig>(File.ReadAllText(file));
        }
    }
}