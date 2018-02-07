using Discord.Commands;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Cyanide.Modules
{
    [Name("Restart"), Group("Restart")]
    [Summary("Restarts Cyanide.")]
    [RequireOwner]
    public class CyanRemoteRestartAsync : CyanModuleBase
    {
        Process process = new Process();
        ProcessStartInfo startInfo = new ProcessStartInfo()
        {
            FileName = "dotnet.exe",
            Arguments = "run Cyanide.dll"
        };

        [Command]
        public async Task RemoteRestartAsync()
        {
            await ReplyAsync("Restarting...");
            
            ConsoleCyanifier.NewLine("Restart command detected. Restarting...");
            ConsoleCyanifier.NewLine();

            Context.Channel.EnterTypingState().Dispose();
            await Context.Client.StopAsync();
            await Context.Client.LogoutAsync();

            process.StartInfo = startInfo;
            process.Start();

            Environment.Exit(0);
        }
    }
}
