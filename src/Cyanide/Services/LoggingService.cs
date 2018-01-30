using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Cyanide
{
    public class LoggingService
    {
        private readonly DiscordSocketClient cyanClient;
        private readonly CommandService cyanCommands;

        private string CyanLogDirectory { get; }
        private string CyanLogFile => Path.Combine(CyanLogDirectory, $"{DateTime.UtcNow.ToString("yyyy-MM-dd")}.txt");

        public LoggingService(DiscordSocketClient discord, CommandService commands)
        {
            CyanLogDirectory = Path.Combine(AppContext.BaseDirectory, "Logs" );

            cyanClient = discord;
            cyanCommands = commands;

            cyanClient.Log += OnLogAsync;
            cyanCommands.Log += OnLogAsync;
        }

        public Task LogAsync(object severity, string source, string message)
        {
            if (!Directory.Exists(CyanLogDirectory))
                Directory.CreateDirectory(CyanLogDirectory);
            if (!File.Exists(CyanLogFile))
                File.Create(CyanLogFile).Dispose();

            string logText = $"{DateTime.UtcNow.ToString("hh:mm:ss")} [{severity}] {source}: {message}";
            File.AppendAllText(CyanLogFile, logText + "\n");

            return Console.Out.WriteLineAsync(logText);
        }
        private Task OnLogAsync(LogMessage msg)
            => LogAsync(msg.Severity, msg.Source, msg.Exception?.ToString() ?? msg.Message);
    }
}