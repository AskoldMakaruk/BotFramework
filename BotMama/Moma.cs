using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using BotFramework.Bot;
using Newtonsoft.Json;


namespace BotMama
{
    public static partial class Moma
    {
        public static  MomaConfig   Config                       { get; set; }
        public static  List<Client> Clients                      { get; set; }
        private static string       ConfigPath                   { get; set; }
        private static string       ToPath(params string[] args) => Path.Combine(args);

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
                    BotsDir = ToPath("data", "Bots")
                };
                SaveConfig();
            }

            if (Config.BotConfigs == null)
            {
                Config.BotConfigs = new[] {new MomaConfig.BotConfig()};
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

                var botDir = ToPath(Config.BotsDir, botConfig.Name);

                if (botConfig.DataDir == null)
                {
                    botConfig.DataDir = ToPath(botDir, "data");
                    Directory.CreateDirectory(botConfig.DataDir);
                }

                if (botConfig.SrcDir == null)
                {
                    botConfig.SrcDir = ToPath(botDir, "src");
                    Directory.CreateDirectory(botConfig.SrcDir);
                }

                if (botConfig.BinDir == null)
                {
                    botConfig.BinDir = ToPath(botDir, "bin");
                    Directory.CreateDirectory(botConfig.BinDir);
                }

                if (botConfig.GitRepo != null)
                {
                    if (!Directory.Exists(botConfig.SrcDir) || !Directory.EnumerateFiles(botConfig.SrcDir).Any())
                    {
                        Directory.CreateDirectory(botConfig.SrcDir);
                        await CloneRepo(botConfig.GitRepo, botConfig.SrcDir, botConfig.Branch);
                    }

                    botConfig.BinDir = ToPath(botConfig.SrcDir, "bin");
                }
                else
                {
                    Log("Problem in config: can't find bot source");
                    continue;
                }

                var csprojFile = Directory.EnumerateFiles(botConfig.SrcDir).FirstOrDefault(f => f.EndsWith(".csproj"));
                var csprojDoc  = new XmlDocument();
                csprojDoc.Load(csprojFile);

                var botFrameworkNode = csprojDoc.DocumentElement.SelectSingleNode("/Project/ItemGroup/Reference[@Include='BotFramework']");
                if (botFrameworkNode != null)
                {
                    botFrameworkNode.ParentNode.RemoveChild(botFrameworkNode);
                    csprojDoc.Save(csprojFile);
                }

                if (csprojDoc.DocumentElement.SelectSingleNode("/Project/ItemGroup/ProjectReference[contains(@Include,'BotFramework')]") == null)
                {
                    if (Config.FrameworkPath == null || !File.Exists(Config.FrameworkPath) || !Config.FrameworkPath.EndsWith(".csproj"))
                    {
                        Config.FrameworkPath = Directory.GetParent(Directory.GetCurrentDirectory()).EnumerateDirectories("BotFramework").FirstOrDefault()?.EnumerateFiles(".csproj").FirstOrDefault()?.Name;
                    }

                    if (Config.FrameworkPath == null)
                    {
                        Log("Error: invalid BotFramework path.");
                        return;
                    }

                    await DotnetAddReference(csprojFile, Config.FrameworkPath);
                }

                botConfig.IsValid = true;
            }
            SaveConfig();
        }

        public static async void StartBots()
        {
            Clients = new List<Client>();
            foreach (var botConfig in Config.BotConfigs.Where(b => b.IsValid))
            {
                await DotnetRestore(botConfig.SrcDir);
                await DotnetPublish(botConfig.SrcDir, botConfig.BinDir);

                var dllfile = Directory.EnumerateFiles(botConfig.BinDir, botConfig.Name + ".dll", SearchOption.AllDirectories)
                                       .FirstOrDefault();
                if (dllfile == null) continue;

                var assembly = Assembly.LoadFrom(dllfile);
                AppDomain.CurrentDomain.Load(assembly.GetName());
                var client = new Client();
                client.OnLog += Log;
                client.Configure(new Configuration
                {
                    Token    = botConfig.Token,
                    Webhook  = botConfig.UseWebHook ?? false,
                    Assembly = assembly,
                    Name     = botConfig.Name,
                    DataDir  = botConfig.DataDir
                });
                Clients.Add(client);
            }
        }

        public static void Log(Client client, string message) => Log($"{client?.Name}: {message}");

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