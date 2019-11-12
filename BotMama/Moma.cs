using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Xml;
using BotFramework.Bot;
using CliWrap.Models;
using Newtonsoft.Json;

namespace BotMama
{
    public static partial class Moma
    {
        public static char S = Path.DirectorySeparatorChar;
        public static MomaConfig Config { get; set; }
        public static List<IClient> Clients { get; set; }
        private static string ConfigPath { get; set; }

        public static void LoadConfiguration(string momaConfigPath)
        {
            ConfigPath = momaConfigPath;
            if (File.Exists(ConfigPath))
            {
                Config = LoadConfig();
            }
            else
            {
                Config = new MomaConfig
                {
                    BotsDir = $"data{S}Bots",
                };
                SaveConfig();
            }

            if (Config.BotConfigs == null)
            {
                Config.BotConfigs = new MomaConfig.BotConfig[] { default };
                SaveConfig();
            }

            if (!Directory.Exists(Config.BotsDir))
                Directory.CreateDirectory(Config.BotsDir);

        }

        public static async void ValidateBots()
        {
            foreach (var botConfig in Config.BotConfigs)
            {
                if (botConfig.Name == null)
                {
                    Log("Error in config: botname is null");
                    continue;
                }

                if (botConfig.GitRepo == null)
                {
                    Log("Error in config: gitrepo is null");
                    continue;
                }

                var botdir = Config.BotsDir + S + botConfig.Name;
                if (!Directory.Exists(botdir) || Directory.EnumerateFiles(botdir).Count() == 0)
                {
                    Directory.CreateDirectory(botdir);
                    await CloneRepo(botConfig.GitRepo, botdir);
                }

                var csprojFile = Directory.EnumerateFiles(botdir).FirstOrDefault(f => f.EndsWith(".csproj"));
                var csprojDoc = new XmlDocument();
                csprojDoc.Load(csprojFile);

                var botFrameworkNode = csprojDoc.DocumentElement.SelectSingleNode("/Project/ItemGroup/Reference[@Include='BotFramework']");
                if (botFrameworkNode != null)
                {
                    csprojDoc.RemoveChild(botFrameworkNode);
                    csprojDoc.Save(csprojFile);
                }
                if (csprojDoc.DocumentElement.SelectSingleNode("/Project/ItemGroup/ProjectReference[contains(@Include,'BotFramework')]") == null)
                    //todo this
                    await DotnetAddReference(csprojFile, Config.FrameworkPath);
            }
        }

        public static async void StartBots()
        {
            Clients = new List<IClient>();
            foreach (var botConfig in Config.BotConfigs)
            {
                var botdir = Config.BotsDir + S + botConfig.Name;
                await DotnetRestore(botdir);
                await DotnetBuild(botdir);

                var dllfile = Directory.EnumerateFiles(botdir, botConfig.Name + ".dll", SearchOption.AllDirectories).FirstOrDefault();

                var assembly = Assembly.LoadFrom(dllfile);
                AppDomain.CurrentDomain.Load(assembly.GetName());
                var client = new Client();
                client.OnLog += Log;
                client.Configure(new Configuration
                {
                    Token = botConfig.Token,
                        Webhook = botConfig.UseWebHook??false,
                        Assembly = assembly,
                        Name = botConfig.Name
                });
                Clients.Add(client);
            }
        }

        public static void Log(IClient client, string message) => Log($"{client?.Name}: {message}");

        public static void Log(string message)
        {
            Console.WriteLine(message);
        }

        private static MomaConfig LoadConfig()
        {
            return JsonConvert.DeserializeObject<MomaConfig>(File.ReadAllText(ConfigPath));
        }

        private static void SaveConfig()
        {
            File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(Config));
        }
    }
}