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

        public CyanHelp(    CommandService commands     ,
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
        {
            if (Context.Guild != null) return await cyanManager.GetPrefixAsync(Context.Guild.Id);
            else return $"{cyanConfig["globalprefix"]}";
        }
        
        [Command]
        public async Task HelpAsync()
        {
            string prefix = await GetPrefixAsync();
            bool isDM = (Context.Guild != null) ? false : true;
            var modules = cyanCommands.Modules.Where(x => !string.IsNullOrWhiteSpace(x.Summary));
            var cyanide = await Context.Client.GetApplicationInfoAsync();
            
            var embed = new EmbedBuilder()
                .WithColor(0, 255, 255)
                .WithAuthor(x =>
                {
                    x.Name = cyanide.Name;
                    x.IconUrl = cyanide.IconUrl;
                });
                
            if (isDM)
            {
                embed.WithDescription($"Global Prefix: `{prefix}`\n\n" +
                                      $"List of available commands:")
                     .WithFooter(x => x.Text = $"Type `{prefix} help <module>` for more information.");
            }
            else
            {
                prefix = await GetPrefixAsync();

                embed.WithDescription($"Global Prefix: `{cyanConfig["globalprefix"]}`\n" +
                                      $"Local Prefix: `{prefix}`\n\n" +
                                      $"List of available commands:")
                     .WithFooter(x => x.Text = $"Type `{prefix} help <module>` for more information.");
            }
            
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

                embed.AddField(module.Name, module.Summary);
            }

            embed.Build();
            await ReplyAsync(embed, true);
        }

        [Command]
        public async Task HelpAsync(string moduleName)
        {
            string prefix = await GetPrefixAsync();

            var module = cyanCommands.Modules.FirstOrDefault(x => x.Name.ToLower() == moduleName.ToLower());
            var cyanide = await Context.Client.GetApplicationInfoAsync();

            if (module == null)
            {
                await ReplyEmbedAsync(2, "Error", $"The module `{moduleName}` does not exist.");
                return;
            }

            var commands = module.Commands.Where(x => !string.IsNullOrWhiteSpace(x.Summary))
                                 .GroupBy(x => x.Name)
                                 .Select(x => x.First());

            if (commands.Count() == 0)
            {
                await ReplyEmbedAsync(2, "Error", $"The module `{module.Name}` has no available commands.");
                return;
            }

            var embed = new EmbedBuilder()
                .WithAuthor(x =>
                {
                    x.Name = cyanide.Name;
                    x.IconUrl = cyanide.IconUrl;
                })
                .WithTitle(moduleName)
                .WithFooter(x => x.Text = $"Type `{prefix}help <module> <command>` for more information.")
                .WithColor(0, 255, 255);

            foreach (var command in commands)
            {
                var result = await command.CheckPreconditionsAsync(Context, cyanProvider);
                if (result.IsSuccess)
                    embed.AddField(prefix + command.Aliases.First(), command.Summary);
            }

            embed.Build();
            await ReplyAsync(embed, true);
        }

        [Command]
        public async Task HelpAsync(string moduleName, string commandName)
        {
            string alias = $"{moduleName} {commandName}".ToLower();
            string prefix = await GetPrefixAsync();

            var module = cyanCommands.Modules.FirstOrDefault(x => x.Name.ToLower() == moduleName.ToLower());
            var cyanide = await Context.Client.GetApplicationInfoAsync();

            if (module == null)
            {
                await ReplyEmbedAsync(2, "Error", $"The module `{moduleName}` does not exist.");
                return;
            }

            var commands = module.Commands.Where(x => !string.IsNullOrWhiteSpace(x.Summary));

            if (commands.Count() == 0)
            {
                await ReplyEmbedAsync(2, "Error", $"The module `{module.Name}` has no available commands.");
                return;
            }

            var command = commands.Where(x => x.Aliases.Contains(alias));
            var embed = new EmbedBuilder()
                .WithAuthor(x =>
                {
                    x.Name = cyanide.Name;
                    x.IconUrl = cyanide.IconUrl;
                })
                .WithColor(0, 255, 255);

            var aliases = new List<string>();
            foreach (var overload in command)
            {
                var result = await overload.CheckPreconditionsAsync(Context, cyanProvider);
                if (result.IsSuccess)
                {
                    var strBuilder = new StringBuilder()
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

                        strBuilder.Append(" " + p);
                    }

                    embed.AddField(strBuilder.ToString(), overload.Remarks ?? overload.Summary);
                }
                aliases.AddRange(overload.Aliases);
            }

            embed.WithFooter(x => x.Text = $"Aliases: {string.Join(", ", aliases)}");

            embed.Build();
            await ReplyAsync(embed, true);
        }
    }
}