using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Discord;
using Discord.Commands;
using Newtonsoft.Json.Linq;

namespace Cyanide.Modules
{
    [Name("Translate")]
    [Group("Translate")]
    [Summary("Translates messages.")]
    public class CyanTranslate : CyanModuleBase
    {
        readonly string langDir = Directory.GetCurrentDirectory() + @"\Modules\Translator\lang.json";
        readonly string rawDir  = Directory.GetCurrentDirectory() + @"\Modules\Translator\rawText.txt";
        readonly string apiLinkPrefix = "https://translate.googleapis.com/translate_a/single?client=gtx&sl=";
        readonly string disclaimer = "Dev note: If the result isn't what you expected, try specifying source language manually." + "\n"
                          + "Known bug(s): translating non-Latin-based texts is buggy due to unresolved parsing issues.";

        string sourceLangISO;
        string targetLangISO;
        string rawJsonText;
        string resultText;

        string RegexFilter(string filterInput)
        {
            string filter = "";
            Regex regex = new Regex("(\".*?\")");
            Match match = regex.Match(filterInput);
            if (match.Success)
            {
                filter = match.Groups[1].Value;
            }
            return filter;
        }

        [Command("auto")]
        [Summary("Auto-detects the input text's language and translates the text into the specified target language.")]
        public async Task AutoTranslateAsync(string sourceText, string targetLangText)
        {
            var typingState = Context.Channel.EnterTypingState();

            //Converts the language text to ISO code
            JToken langToken = JObject.Parse(File.ReadAllText(langDir));
            targetLangISO = (string)langToken.SelectToken(targetLangText);

            //Checks if any downloaded files exist, and delete them if there were
            if (File.Exists(rawDir))
            {
                File.Delete(rawDir);
            };

            //Downloads the file from Google Translate (Chrome Extension) API link and assigns all texts to a variable
            string url = apiLinkPrefix + "auto" + "&tl=" + targetLangISO + "&dt=t&q=" + Uri.EscapeUriString(sourceText);
            WebClient cyanClient = new WebClient();
            cyanClient.DownloadFile(url, rawDir);
            rawJsonText = File.ReadAllText(rawDir);

            //Refers to RegexFilter function, which filters the content of the texts
            resultText = RegexFilter(rawJsonText);

            //Builds the embed for result
            var builder = new EmbedBuilder()
                .WithColor(0, 255, 255)
                .AddField("Original text:", sourceText)
                .AddField("Target language:", targetLangText)
                .AddField("Translated text:", resultText)
                .WithFooter(disclaimer);

            //Returns the result to Discord
            await ReplyAsync(builder);

            //Deletes the downloaded file
            File.Delete(rawDir);

            typingState.Dispose();
        }

        [Command("manual")]
        [Summary("Translates the text into the specified target language.")]
        public async Task ManualTranslateAsync(string sourceText, string sourceLangText, string targetLangText)
        {
            var typingState = Context.Channel.EnterTypingState();
            
            JToken langToken = JObject.Parse(File.ReadAllText(langDir));
            sourceLangISO = (string)langToken.SelectToken(sourceLangText);
            targetLangISO = (string)langToken.SelectToken(targetLangText);
            
            if (File.Exists(rawDir))
            {
                File.Delete(rawDir);
            };
            
            string url = apiLinkPrefix + sourceLangISO + "&tl=" + targetLangISO + "&dt=t&q=" + Uri.EscapeUriString(sourceText);

            WebClient cyanClient = new WebClient();
            cyanClient.DownloadFile(url, rawDir);
            rawJsonText = File.ReadAllText(rawDir);

            resultText = RegexFilter(rawJsonText);

            var builder = new EmbedBuilder()
                .WithColor(0, 255, 255)
                .AddField("Original text:", sourceText)
                .AddField("Source language:", sourceLangText)
                .AddField("Target language:", targetLangText)
                .AddField("Translated text:", resultText)
                .WithFooter(disclaimer);
            
            await ReplyAsync(builder);
            
            typingState.Dispose();
        }

        [Command]
        [Summary("Auto-detects the previous message's language and translate it to English.")]
        public async Task EnglishPlsAsync()
        {
            var typingState = Context.Channel.EnterTypingState();

            var messages = await Context.Channel.GetMessagesAsync(2).Flatten();

            string sourceText = "";
            foreach (var content in messages)
            {
                if (content.Content.Equals("Cyan translate", StringComparison.OrdinalIgnoreCase))
                    continue;

                sourceText = content.Content;
            }
            
            if (File.Exists(rawDir))
            {
                File.Delete(rawDir);
            };
            
            string url = apiLinkPrefix + "auto" + "&tl=" + "en" + "&dt=t&q=" + Uri.EscapeUriString(sourceText);

            WebClient cyanClient = new WebClient();
            cyanClient.DownloadFile(url, rawDir);
            rawJsonText = File.ReadAllText(rawDir);

            resultText = RegexFilter(rawJsonText);

            await ReplyAsync("They said: " + resultText + "\n" + disclaimer);
            
            File.Delete(rawDir);

            typingState.Dispose();
        }
    }
}