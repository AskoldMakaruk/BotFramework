using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using BotFramework.Bot;
using Newtonsoft.Json;

namespace BotMama
{
    public static class Moma
    {
        public static char S = Path.DirectorySeparatorChar;
        public static MomaConfig Config { get; set; }
        public static List<IClient> Clients { get; set; }
        private static string ConfigPath { get; set; }

        public static async void Configure(string momaConfigPath)
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

            Clients = new List<IClient>();
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
                if (!Directory.Exists(botdir))
                {
                    Directory.CreateDirectory(botdir);
                    await CloneRepo(botConfig.GitRepo, botdir);
                }

                {
                    var csprojFile = Directory.EnumerateFiles(botdir).FirstOrDefault(f => f.EndsWith(".csproj"));
                    var csprojDoc = new XmlDocument();
                    var botFrameworkNode = csprojDoc.DocumentElement.SelectSingleNode("/Project/ItemGroup/Reference[@Include'BotFramework']");
                    csprojDoc.RemoveChild(botFrameworkNode);
                    csprojDoc.Save(csprojFile);
                    await DotnetAddReference(csprojFile, Config.FrameworkPath);
                }

                await DotnetRestore(botdir);
                await DotnetBuild(botdir);
                var dllfile = Directory.EnumerateFiles(botdir, botConfig.Name + ".dll").FirstOrDefault();

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

        private static async Task CloneRepo(string giturl, string dirname)
        {
            var result = await CliWrap.Cli.Wrap("git")
                .SetArguments($"clone {giturl} {dirname}")
                .EnableExitCodeValidation(false)
                .ExecuteAsync();
            Log(result.StandardOutput);
        }

        private static async Task DotnetAddReference(string project, string reference)
        {
            var result = await CliWrap.Cli.Wrap("dotnet")
                .SetArguments($"add reference {project} {reference}")
                .EnableExitCodeValidation(false)
                .ExecuteAsync();
            Log(result.StandardOutput);
        }

        private static async Task DotnetRestore(string dirname)
        {
            var result = await CliWrap.Cli.Wrap("dotnet")
                .SetArguments($"restore {dirname}")
                .EnableExitCodeValidation(false)
                .ExecuteAsync();
            Log(result.StandardOutput);
        }

        private static async Task DotnetBuild(string dirname)
        {
            var result = await CliWrap.Cli.Wrap("dotnet")
                .SetArguments($"build {dirname}")
                .EnableExitCodeValidation(false)
                .ExecuteAsync();
            Log(result.StandardOutput);
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