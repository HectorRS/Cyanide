using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Discord;
using Discord.Commands;
using SauceNaoSharp;

namespace Cyanide.Modules.Media
{
    public class CyanSauceNAO : ModuleBase<SocketCommandContext>
    {
        private readonly IConfigurationRoot CyanConfig;

        public CyanSauceNAO(IConfigurationRoot config)
        {
            CyanConfig = config;
        }

        [Command("saucenao")]
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

            string Token = CyanConfig["tokens:SauceNAO"];
            if (string.IsNullOrWhiteSpace(Token))
                throw new Exception("SauceNAO API key missing.");

            var sauceNao = new SauceNao(Token);
            
            SauceNaoSettings.ResultCount = 1;
            
            var response = await sauceNao.GetSourceAsync(url);
            if (response == null) return;
            
            foreach (var results in response.Results)
            {
                await ReplyAsync(   results.ResultData.Title + "\n" +
                                    results.ResultData.Urls[0]      );
            }
        }
    }
}
