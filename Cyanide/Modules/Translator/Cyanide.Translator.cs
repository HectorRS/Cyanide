using System;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Net;
using Discord.Commands;
using Newtonsoft.Json.Linq;

namespace Cyanide.Modules.Translator
{
    public class CyanTranslate : ModuleBase<SocketCommandContext>
    {
        [Command("autotranslate")]
        public async Task TranslateAsync(string sourceText, string targetLangText)
        {
            //Sets directories
            string langDir = Directory.GetCurrentDirectory() + @"\Modules\Translator\lang.json";
            string rawDir = Directory.GetCurrentDirectory() + @"\Modules\Translator\rawText.txt";
            
            //Initializes variables
            string rawJsonText;
            string resultText;

            //Converts the language text to ISO code
            string targetLangISO;
            JToken langToken = JObject.Parse(File.ReadAllText(langDir));
            targetLangISO = (string)langToken.SelectToken(targetLangText);

            //Checks if any downloaded files exist, and delete them if there were
            if (File.Exists(rawDir))
            {
                File.Delete(rawDir);
            };

            //Downloads the file from Google Translate (Chrome Extension) API link and assigns all texts to a variable
            string url = "https://translate.googleapis.com/translate_a/single?client=gtx&sl="
                         + "auto" + "&tl=" + targetLangISO + "&dt=t&q=" + Uri.EscapeUriString(sourceText);
            WebClient cyanClient = new WebClient();
            cyanClient.DownloadFile(url, rawDir);
            rawJsonText = File.ReadAllText(rawDir);

            //Filters the content of the texts
            Regex regex = new Regex("(\".*?\")");
            Match match = regex.Match(rawJsonText);
            if (match.Success)
            {
                resultText = match.Groups[1].Value;
            }
            else resultText = "An error has occured.";

            //Returns the result to Discord
            await ReplyAsync(   "Target Language: " + targetLangText + "\n" +
                                "Translated text: " + resultText            );

            //Deletes the downloaded file
            File.Delete(rawDir);
        }
    }
}