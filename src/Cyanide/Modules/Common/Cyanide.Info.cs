using Discord;
using Discord.Commands;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Cyanide.Modules
{
    [Name("Info"), Group("Info")]
    [Summary("Gets info about Cyanide.")]
    public class CyanGeneralInfo : CyanModuleBase
    {
        [Command]
        public async Task InfoAsync()
        {
            var cyanide = await Context.Client.GetApplicationInfoAsync();
            
            var builder = new EmbedBuilder()
                .WithColor(0,255,255)
                .WithAuthor(x =>
                {
                    x.Name = cyanide.Name.ToString();
                    x.IconUrl = cyanide.IconUrl;
                    x.Url = "https://discord.gg/eddtepc";
                })
                .WithDescription("Developed by HectorRS and Lemonical")
                .AddInlineField("Memory", GetMemoryUsage())
                .AddInlineField("Latency", GetLatency())
                .AddInlineField("Uptime", GetUptime())
                .WithFooter(GetCyanVersion() + ", " + GetLibrary());
            
            await ReplyAsync(builder);
        }

        public string GetUptime()
        {
            var uptime = (DateTime.Now - Process.GetCurrentProcess().StartTime);
            return $"{uptime.Days}d {uptime.Hours}h {uptime.Minutes}m {uptime.Seconds}s";
        }

        public string GetLibrary()
            => $"Discord.Net v{DiscordConfig.Version}";
        public string GetCyanVersion()
            => $"Cyanide v{AppHelper.Version}";
        public string GetMemoryUsage()
            => $"{Math.Round(GC.GetTotalMemory(true) / (1024.0 * 1024.0), 2)}mb";
        public string GetLatency()
            => $"{Context.Client.Latency}ms";
    }
}