using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Discord;
using Discord.Commands;


namespace Cyanide.Modules
{
    [Name("Info"), Group("Info")]
    [Summary("Gets info about Cyanide.")]
    public class CyanGeneralInfo : CyanModuleBase
    {
        private readonly IConfigurationRoot cyanConfig;

        public CyanGeneralInfo(IConfigurationRoot config)
        {
            cyanConfig = config;
        }

        [Command]
        public async Task InfoAsync()
        {
            bool isInline = true;

            var embed = new EmbedBuilder()
                .WithColor(0,255,255)
                .WithThumbnailUrl(Context.Client.CurrentUser.GetAvatarUrl())
                .WithTitle("Cyanide")
                .AddField("Version", GetCyanVersion(), isInline)
                .AddField("Latency", GetLatency(), isInline)
                .AddField("Developed by", "HectorRS\nLemonical")
                .WithFooter("Cyan is the new Black!");

            embed.Build();
            await ReplyAsync(embed);
        }

        [Command("verbose"), Alias("vb")]
        public async Task InfoVerboseAsync()
        {
            bool isInline = true;

            var embed = new EmbedBuilder()
                .WithColor(0, 255, 255)
                .WithThumbnailUrl(Context.Client.CurrentUser.GetAvatarUrl())
                .WithTitle("Cyanide")
                .AddField("Version", GetCyanVersion(), isInline)
                .AddField("Latency", GetLatency(), isInline)
                .AddField("Library", GetLibrary())
                .AddField("Uptime", GetUptime(), isInline)
                .AddField("Memory Usage", GetMemoryUsage(), isInline)
                .AddField("Developed by", "HectorRS\nLemonical")
                .WithFooter(GetGame());

            embed.Build();
            await ReplyAsync(embed);
        }

        public string GetUptime()
        {
            var uptime = (DateTime.Now - Process.GetCurrentProcess().StartTime);
            return $"{uptime.Days}d {uptime.Hours}h {uptime.Minutes}m {uptime.Seconds}s";
        }

        public string GetLibrary()
            => $"Discord.Net v{DiscordConfig.Version}";
        public string GetCyanVersion()
            => $"v{AppHelper.Version}";
        public string GetMemoryUsage()
            => $"{Math.Round(GC.GetTotalMemory(true) / (1024.0 * 1024.0), 2)}mb";
        public string GetLatency()
            => $"{Context.Client.Latency}ms";
        public string GetGame()
            => $"{cyanConfig["game"]}";
    }
}