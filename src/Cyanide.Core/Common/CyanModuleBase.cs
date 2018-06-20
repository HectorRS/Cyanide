using Discord;
using Discord.Commands;
using System.IO;
using System.Threading.Tasks;
using System;

namespace Cyanide
{
    public class CyanModuleBase : ModuleBase<CyanCommandContext>
    {
        public async Task<IUserMessage> ReplyAsync(string message, Embed embed = null, bool isDM = false, RequestOptions options = null)
        {
            if (isDM)       //Reply in DM
                return await Context.User.SendMessageAsync(message, false, embed, options).ConfigureAwait(false);
            else            //Reply in Channel
                return await Context.Channel.SendMessageAsync(message, false, embed, options).ConfigureAwait(false);
        }

        public async Task<IUserMessage> ReplyAsync(Embed embed = null, bool isDM = false, RequestOptions options = null)
        {
            if (isDM)       //Reply in DM
                return await Context.User.SendMessageAsync("", false, embed, options).ConfigureAwait(false);
            else            //Reply in Channel
                return await Context.Channel.SendMessageAsync("", false, embed, options).ConfigureAwait(false);
        }

        public async Task<IUserMessage> ReplyAsync(Stream stream, string fileName, string message = null, RequestOptions options = null)
        {
            return await Context.Channel.SendFileAsync(stream, fileName, message, false, options).ConfigureAwait(false);
        }

        public async Task<IUserMessage> ReplyEmbedAsync(int opstat, string title, string description, bool isDM, RequestOptions options = null)
            => await ReplyEmbedAsync(opstat, title, description, null, null, isDM, options);

        public async Task<IUserMessage> ReplyEmbedAsync(int opstat, string title, string description, string url = null, string footer = null, bool isDM = false, RequestOptions options = null)
        {
            /* -------------- OPSTAT codes -------------- *
             *                                            *
             * OPSTAT = 0 -> Default format               *
             * OPSTAT = 1 -> Operation successful format  *
             * OPSTAT = 2 -> Operation failed format      *
             * OPSTAT = 3 -> Configuration format         *
             *                                            *
             * - Other opstat codes return blank format - */
             
            var cyanide = await Context.Client.GetApplicationInfoAsync();
            var embed = new EmbedBuilder();

            switch (opstat)
            {
                case 0:     // Default format: default embed color, using author, title and description
                    embed.WithColor(0, 255, 255)
                         .WithAuthor(x =>
                         {
                             x.Name = cyanide.Name;
                             x.IconUrl = cyanide.IconUrl;
                         })
                         .WithTitle(title)
                         .WithDescription(description);
                    break;
                case 1:     // Operation successful format: green embed color, using title as author, and description
                    embed.WithColor(107, 153, 61)
                         .WithAuthor(x =>
                         {
                             x.Name = title;
                             x.IconUrl = "https://i.imgur.com/ldIcE1r.png";
                         })
                         .WithDescription(description);
                    break;
                case 2:     // Operation failed format: red embed color, using title as author, and description
                    embed.WithColor(153, 61, 61)
                         .WithAuthor(x =>
                         {
                             x.Name = title;
                             x.IconUrl = "https://i.imgur.com/NZdGcja.png";
                         })
                        .WithDescription(description);
                    break;
                case 3:     // Configuration format: yellow embed color, using title as author, and description
                    embed.WithColor(204, 171, 41)
                         .WithAuthor(x =>
                         {
                             x.Name = title;
                             x.IconUrl = "https://i.imgur.com/7E05otS.png";
                         })
                         .WithDescription(description);
                break;
                default:    // Blank format: default embed color, using title and description
                    embed.WithColor(0, 255, 255)
                         .WithTitle(title)
                         .WithDescription(description);
                    break;
            }
                
            if (url != null && Uri.IsWellFormedUriString(url, UriKind.Absolute))
                embed.WithUrl(url);

            if (!string.IsNullOrWhiteSpace(footer))
                embed.WithFooter(efb => efb.WithText(footer));
            
            embed.Build();

            if (isDM)
                return await Context.User.SendMessageAsync("", false, embed, options).ConfigureAwait(false);
            else
                return await Context.Channel.SendMessageAsync("", false, embed, options).ConfigureAwait(false);
        }
    }
}