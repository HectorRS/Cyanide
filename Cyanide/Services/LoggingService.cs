using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Cyanide.Services
{
    public class LoggingService
    {
        private readonly DiscordSocketClient CyanClient;
        private readonly CommandService CyanCommands;

        private string CyanLogDirectory { get; }
        private string CyanLogFile => Path.Combine(CyanLogDirectory, $"{DateTime.UtcNow.ToString("yyyy-MM-dd")}.txt");

        public LoggingService(  DiscordSocketClient discord,
                                CommandService commands       )
        {
            CyanLogDirectory = Path.Combine(AppContext.BaseDirectory, "Logs" );

            CyanClient = discord;
            CyanCommands = commands;

            CyanClient.Log += CyanLog;
            CyanCommands.Log += CyanLog;
        }

        private Task CyanLog(LogMessage Message)
        {
            if (!Directory.Exists(CyanLogDirectory))
                Directory.CreateDirectory(CyanLogDirectory);

            if (!File.Exists(CyanLogFile))
                File.Create(CyanLogFile).Dispose();

            string logText = $"{DateTime.UtcNow.ToString("hh:mm:ss")} [{Message.Severity}] {Message.Source}: {Message.Exception?.ToString() ?? Message.Message}";
            File.AppendAllText(CyanLogFile, logText + "\n");

            return Console.Out.WriteLineAsync(logText);
        }
    }
}