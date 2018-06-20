using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
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

        private bool IsOversized(string url)
        {
            WebClient client = new WebClient();
            MemoryStream ms = new MemoryStream(client.DownloadData(url));

            long size = ms.Length;

            client.Dispose();
            ms.Dispose();

            if (size >= 15728640) return true;
            else return false;
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
                await ReplyEmbedAsync(2, "Error: No images/attachments found", "Try reposting the image.");
                return;
            }

            if (IsOversized(url))
            {
                await ReplyEmbedAsync(2, "Error: Oversized image", "Image cannot be 15MB or higher.");
                return;
            }

            string Token = cyanConfig["tokens:SauceNAO"];
            if (string.IsNullOrWhiteSpace(Token))
                throw new Exception("SauceNAO API key missing.");

            var sauceNao = new SauceNao(Token);
            
            SauceNaoSettings.ResultCount = 1;

            try
            {
                var response = await sauceNao.GetSourceAsync(url);
                if (response == null) return;

                foreach (var results in response.Results)
                {
                    if (double.Parse(results.ResultInfo.Similarity) >= 65.00)
                    {
                        if (!string.IsNullOrWhiteSpace(results.ResultData.PixivId.ToString())
                            && !string.IsNullOrWhiteSpace(results.ResultData.PixivMemberName))
                        {
                            await ReplyEmbedAsync(1,
                            "Result found:",
                            "Pixiv: \"" + results.ResultData.Title + "\"\n" +
                            "Artist: " + results.ResultData.PixivMemberName + "\n" +
                            results.ResultData.Urls[0]);
                        }
                        else await ReplyEmbedAsync(1, 
                            "Result found:",
                            "```prolog\n"   +
                            "Similarity: '" + results.ResultInfo.Similarity     + "%'\n" +
                            "Title: '"      + results.ResultData.Title          + "'\n" +
                            "Creator: '"    + results.ResultData.ImageCreator   + "'\n" +
                            "Source: '"     + results.ResultData.ImageSource    + "'\n" +
                            "Url: ['"       + results.ResultData.Urls[0]        + "']\n```");
                    }
                    else await ReplyEmbedAsync(2, "Error: Similarity too low", "No results found.");
                }
            }
            catch
            {
                await ReplyEmbedAsync(2, "Error: Unhandled Exception", "Try going directly to SauceNAO:\n" + "http://saucenao.com/search.php?db=999&url=" + url);
            }
        }
    }
}
