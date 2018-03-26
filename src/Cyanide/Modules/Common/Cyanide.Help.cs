using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Configuration;

namespace Cyanide.Modules
{
    [Name("Help")]
    [Group("Help")]
    public class CyanHelp : CyanModuleBase
    {
        private readonly CommandService cyanCommands;
        private readonly ConfigManager cyanManager;
        private readonly IServiceProvider cyanProvider;
        private readonly IConfigurationRoot cyanConfig;

        public CyanHelp(    CommandService commands      ,
                            ConfigManager manager       ,
                            IServiceProvider provider   ,
                            IConfigurationRoot config   )
        {
            cyanCommands = commands;
            cyanManager = manager;
            cyanProvider = provider;
            cyanConfig = config;
        }

        private async Task<string> GetPrefixAsync()
            => (await cyanManager.GetPrefixAsync(Context.Guild.Id))
            ?? $"{cyanConfig["globalprefix"]}";
        
        [Command]
        public async Task HelpAsync()
        {
            string prefix = await GetPrefixAsync();
            var modules = cyanCommands.Modules.Where(x => !string.IsNullOrWhiteSpace(x.Summary));

            var builder = new EmbedBuilder()
                .WithDescription($"Global Prefix: `{cyanConfig["globalprefix"]}`\n"
                                +$"Local Prefix: `{prefix}`")
                .WithFooter(x => x.Text = $"Type `{prefix} help <module>` for more information.")
                .WithColor(0, 255, 255);

            foreach (var module in modules)
            {
                bool success = false;
                foreach (var command in module.Commands)
                {
                    var result = await command.CheckPreconditionsAsync(Context, cyanProvider);
                    if (result.IsSuccess)
                    {
                        success = true;
                        break;
                    }
                }
                
                if (!success)
                    continue;

                builder.AddField(module.Name, module.Summary);
            }

            await ReplyDMAsync(builder);
        }

        [Command]
        public async Task HelpAsync(string moduleName)
        {
            string prefix = await GetPrefixAsync();
            var module = cyanCommands.Modules.FirstOrDefault(x => x.Name.ToLower() == moduleName.ToLower());

            if (module == null)
            {
                await ReplyAsync($"The module `{moduleName}` does not exist.");
                return;
            }

            var commands = module.Commands.Where(x => !string.IsNullOrWhiteSpace(x.Summary))
                                 .GroupBy(x => x.Name)
                                 .Select(x => x.First());

            if (commands.Count() == 0)
            {
                await ReplyAsync($"The module `{module.Name}` has no available commands :(");
                return;
            }

            var builder = new EmbedBuilder()
                .WithFooter(x => x.Text = $"Type `{prefix}help <module> <command>` for more information")
                .WithColor(0, 255, 255);

            foreach (var command in commands)
            {
                var result = await command.CheckPreconditionsAsync(Context, cyanProvider);
                if (result.IsSuccess)
                    builder.AddField(prefix + command.Aliases.First(), command.Summary);
            }

            await ReplyAsync(builder);
        }

        [Command]
        public async Task HelpAsync(string moduleName, string commandName)
        {
            string alias = $"{moduleName} {commandName}".ToLower();
            string prefix = await GetPrefixAsync();
            var module = cyanCommands.Modules.FirstOrDefault(x => x.Name.ToLower() == moduleName.ToLower());

            if (module == null)
            {
                await ReplyAsync($"The module `{moduleName}` does not exist.");
                return;
            }

            var commands = module.Commands.Where(x => !string.IsNullOrWhiteSpace(x.Summary));

            if (commands.Count() == 0)
            {
                await ReplyAsync($"The module `{module.Name}` has no available commands :(");
                return;
            }

            var command = commands.Where(x => x.Aliases.Contains(alias));
            var builder = new EmbedBuilder()
                .WithColor(0, 255, 255);

            var aliases = new List<string>();
            foreach (var overload in command)
            {
                var result = await overload.CheckPreconditionsAsync(Context, cyanProvider);
                if (result.IsSuccess)
                {
                    var sbuilder = new StringBuilder()
                        .Append(prefix + overload.Aliases.First());

                    foreach (var parameter in overload.Parameters)
                    {
                        string p = parameter.Name;

                        if (parameter.IsRemainder)
                            p += "...";
                        if (parameter.IsOptional)
                            p = $"[{p}]";
                        else
                            p = $"<{p}>";

                        sbuilder.Append(" " + p);
                    }

                    builder.AddField(sbuilder.ToString(), overload.Remarks ?? overload.Summary);
                }
                aliases.AddRange(overload.Aliases);
            }

            builder.WithFooter(x => x.Text = $"Aliases: {string.Join(", ", aliases)}");

            await ReplyAsync(builder);
        }
    }
}