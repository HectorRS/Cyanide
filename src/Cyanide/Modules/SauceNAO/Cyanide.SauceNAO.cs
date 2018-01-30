using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Discord;
using Discord.Commands;
using SauceNaoSharp;

namespace Cyanide.Modules
{
    [Name("SauceNAO")]
    [Summary("Reverse image search module.")]
    public class CyanSauceNAO : CyanModuleBase
    {
        private readonly IConfigurationRoot cyanConfig;

        public CyanSauceNAO(IConfigurationRoot config)
        {
            cyanConfig = config;
        }

        [Command("saucenao")]
        [Summary("Find the source of the last image attachment/embed posted within the same channel")]
        public async Task SauceNaoAsync()
        {
            var messages = await Context.Channel.GetMessagesAsync(100).Flatten();

            string url = "";
            foreach (var attach in messages)
            {
                if (attach.Attachments.Count > 0)
                {
                    url = attach.Attachments.FirstOrDefault().ProxyUrl ?? attach.Attachments.FirstOrDefault().Url;
                    if (url != null)
                        break;
                }

                if (attach.Embeds.Count > 0)
                {
                    url = attach.Embeds.FirstOrDefault().Thumbnail.HasValue
                        ? attach.Embeds.FirstOrDefault().Thumbnail.Value.Url
                        : attach.Embeds.FirstOrDefault().Url;
                    if (url != null)
                        break;
                }
            }

            if (url == null)
            {
                await ReplyAsync("No link found.");
                return;
            }

            string Token = cyanConfig["tokens:SauceNAO"];
            if (string.IsNullOrWhiteSpace(Token))
                throw new Exception("SauceNAO API key missing.");

            var sauceNao = new SauceNao(Token);
            
            SauceNaoSettings.ResultCount = 1;
            
            var response = await sauceNao.GetSourceAsync(url);
            if (response == null) return;
            
            foreach (var results in response.Results)
            {
                await ReplyAsync( results.ResultData.Title + "\n"
                                + results.ResultData.Urls[0]    );
            }
        }
    }
}
