using Discord;
using Newtonsoft.Json;
using System;
using System.IO;

namespace Cyanide
{
    public class Configuration
    {
        [JsonIgnore]
        public static string FileName { get; private set; } = "Config/subconfig.json";
        public AuthTokens Token { get; set; } = new AuthTokens();
        public Configuration() : this("Config/subconfig.json") { }
        public Configuration(string fileName)
        {
            FileName = fileName;
        }
        
        public static void EnsureExists()
        {
            string file = Path.Combine(AppContext.BaseDirectory, FileName);
            if (!File.Exists(file))
            {
                string path = Path.GetDirectoryName(file);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                var config = new Configuration();

                ConsoleCyanifier.Log(LogSeverity.Warning, "Cyanide", "Please enter your token: ");
                string token = Console.ReadLine();

                config.Token.Discord = token;
                config.SaveJson();
            }
            ConsoleCyanifier.Log(LogSeverity.Info, "Cyanide", "Configuration Loaded");
        }

        public void SaveJson()
        {
            string file = Path.Combine(AppContext.BaseDirectory, FileName);
            File.WriteAllText(file, ToJson());
        }

        public static Configuration Load()
        {
            string file = Path.Combine(AppContext.BaseDirectory, FileName);
            return JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(file));
        }

        public string ToJson()
            => JsonConvert.SerializeObject(this, Formatting.Indented);
    }

    public class AuthTokens
    {
        public string Discord { get; set; } = "";
        public string SauceNAO { get; set; } = "";
    }
}
