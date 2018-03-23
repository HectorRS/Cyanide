using Discord;
using Discord.Commands;
using System.IO;
using System.Threading.Tasks;
using System;

namespace Cyanide
{
    public class CyanModuleBase : ModuleBase<CyanCommandContext>
    {
        public async Task<IUserMessage> ReplyAsync(Embed embed = null, RequestOptions options = null)
        {
            return await Context.Channel.SendMessageAsync("", false, embed, options).ConfigureAwait(false);
        }

        public async Task<IUserMessage> ReplyAsync(string message, Embed embed = null, RequestOptions options = null)
        {
            return await Context.Channel.SendMessageAsync(message, false, embed, options).ConfigureAwait(false);
        }

        public async Task<IUserMessage> ReplyEmbedAsync(string title, string text, string url = null, string footer = null)
        {
            var embed = new EmbedBuilder().WithColor(0, 255, 255).WithDescription(text).WithTitle(title);

            if (url != null && Uri.IsWellFormedUriString(url, UriKind.Absolute))
                embed.WithUrl(url);

            if (!string.IsNullOrWhiteSpace(footer))
                embed.WithFooter(efb => efb.WithText(footer));

            return await Context.Channel.SendMessageAsync("", embed: embed.Build());
        }

        public async Task<IUserMessage> ReplyAsync(Stream stream, string fileName, string message = null, RequestOptions options = null)
        {
            return await Context.Channel.SendFileAsync(stream, fileName, message, false, options).ConfigureAwait(false);
        }
    }
}
