using Discord.Commands;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Cyanide.Modules
{
    [Name("Program Control"), Group("prog")]
    [Summary("Tasks for controlling host machine.")]
    [RequireOwner]
    public class CyanProgramControlAsync : CyanModuleBase
    {
        [Command("restart")]
        [Summary("Restarts Cyanide.")]
        public async Task RemoteRestartAsync()
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = "dotnet.exe",
                Arguments = "Cyanide.dll"
            };

            await ReplyAsync("Restarting...");
            
            Console.WriteLine("Restart command detected. Restarting...");
           
            await Context.Client.StopAsync();
            await Context.Client.LogoutAsync();

            process.StartInfo = startInfo;
            process.Start();

            Environment.Exit(0);
        }

        [Command("kill")]
        [Summary("Terminates Cyanide.")]
        public async Task RemoteKillAsync()
        {
            await ReplyAsync("Logging out...");

            await Context.Client.StopAsync();
            await Context.Client.LogoutAsync();
            Environment.Exit(0);
        }
    }
}
