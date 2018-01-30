using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace Cyanide
{
    public static class ConsoleCyanifier
    {
        /// <summary> Write a string to the console on an existing line. </summary>
        /// <param name="text">String written to the console.</param>
        /// <param name="foreground">The text color in the console.</param>
        /// <param name="background">The background color in the console.</param>
        public static void Append(string text, ConsoleColor? foreground = null, ConsoleColor? background = null)
        {
            if (foreground == null)
                foreground = ConsoleColor.Cyan;
            if (background == null)
                background = ConsoleColor.Black;

            Console.ForegroundColor = (ConsoleColor)foreground;
            Console.BackgroundColor = (ConsoleColor)background;
            Console.Write(text);
        }

        /// <summary> Write a string to the console on an new line. </summary>
        /// <param name="text">String written to the console.</param>
        /// <param name="foreground">The text color in the console.</param>
        /// <param name="background">The background color in the console.</param>
        public static void NewLine(string text = "", ConsoleColor? foreground = null, ConsoleColor? background = null)
        {
            if (foreground == null)
                foreground = ConsoleColor.Cyan;
            if (background == null)
                background = ConsoleColor.Black;

            Console.ForegroundColor = (ConsoleColor)foreground;
            Console.BackgroundColor = (ConsoleColor)background;
            Console.Write(Environment.NewLine + text);
        }

        public static void Log(object severity, string source, string message)
        {
            ConsoleCyanifier.NewLine($"{DateTime.Now.ToString("hh:mm:ss")} ", ConsoleColor.DarkGray);
            ConsoleCyanifier.Append($"[{severity}] ", ConsoleColor.Red);
            ConsoleCyanifier.Append($"{source}: ", ConsoleColor.DarkGreen);
            ConsoleCyanifier.Append(message, ConsoleColor.Cyan);
        }

        public static Task LogAsync(object severity, string source, string message)
        {
            ConsoleCyanifier.NewLine($"{DateTime.Now.ToString("hh:mm:ss")} ", ConsoleColor.DarkGray);
            ConsoleCyanifier.Append($"[{severity}] ", ConsoleColor.Red);
            ConsoleCyanifier.Append($"{source}: ", ConsoleColor.DarkGreen);
            ConsoleCyanifier.Append(message, ConsoleColor.Cyan);
            return Task.CompletedTask;
        }

        public static void Log(IUserMessage msg)
        {
            var channel = (msg.Channel as IGuildChannel);
            ConsoleCyanifier.NewLine($"{DateTime.Now.ToString("hh:mm:ss")} ", ConsoleColor.Gray);

            if (channel?.Guild == null)
                ConsoleCyanifier.Append($"[PM] ", ConsoleColor.Magenta);
            else
                ConsoleCyanifier.Append($"[{channel.Guild.Name} #{channel.Name}] ", ConsoleColor.DarkGreen);

            ConsoleCyanifier.Append($"{msg.Author}: ", ConsoleColor.Green);
            ConsoleCyanifier.Append(msg.Content, ConsoleColor.DarkCyan);
        }

        public static void Log(CyanCommandContext c)
        {
            var channel = (c.Channel as SocketGuildChannel);
            ConsoleCyanifier.NewLine($"{DateTime.Now.ToString("hh:mm:ss")} ", ConsoleColor.Gray);

            if (channel == null)
                ConsoleCyanifier.Append($"[PM] ", ConsoleColor.Magenta);
            else
                ConsoleCyanifier.Append($"[{c.Guild.Name} #{channel.Name}] ", ConsoleColor.DarkGreen);

            ConsoleCyanifier.Append($"{c.User}: ", ConsoleColor.Green);
            ConsoleCyanifier.Append(c.Message.Content, ConsoleColor.DarkCyan);
        }
    }
}