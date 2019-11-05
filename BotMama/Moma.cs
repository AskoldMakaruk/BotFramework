using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using BotTypes.cs;
using CliWrap;
using Newtonsoft.Json;

namespace BotMama
{
    public static class Moma
    {
        public static  char          S = Path.DirectorySeparatorChar;
        public static  MomaConfig    Config     { get; set; }
        public static  List<IClient> Clients    { get; set; }
        private static string        ConfigPath { get; set; }

        public static void Configure(string momaConfigPath)
        {
            ConfigPath = momaConfigPath;
            if (File.Exists(ConfigPath)) Config = LoadConfig();
            else
            {
                Config = new MomaConfig
                {
                    BotsDir         = $"data{S}Bots",
                    CertificatesDir = $"data{S}Certificates",
                    NginxConfig     = $"data{S}nginx",
                };
                SaveConfig();
            }

            if (Config.BotConfigs == null)
            {
                Config.BotConfigs = new MomaConfig.BotConfig[] {default};
                SaveConfig();
            }

            if (!Directory.Exists(Config.BotsDir))
                Directory.CreateDirectory(Config.BotsDir);

            if (!Directory.Exists(Config.CertificatesDir))
                Directory.CreateDirectory(Config.CertificatesDir);

            if (!Directory.Exists(Config.NginxConfig))
                Directory.CreateDirectory(Config.NginxConfig);

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

                var dirname = Config.BotsDir + S + botConfig.Name;
                if (!Directory.Exists(dirname))
                {
                    Directory.CreateDirectory(dirname);
                    CloneRepo(botConfig.GitRepo, dirname).RunSynchronously();
                }

                var innerDirs = Directory.GetDirectories(dirname);
                if (innerDirs.FirstOrDefault(d => d == "obj") == null)
                {
                    DotnetRestore(dirname).RunSynchronously();
                }

                if (innerDirs.FirstOrDefault(d => d == "bin") == null)
                {
                    DotnetBuild(dirname).RunSynchronously();
                }
            }

            Clients = LoadAssemblies(Config.BotsDir).ToList();
        }

        public static void Log(string message) => Console.WriteLine(message);

        private static IEnumerable<IClient> LoadAssemblies(string botsDir)
        {
            foreach (var dir in Directory.GetDirectories(botsDir))
            {
                foreach (var file in Directory.GetFiles(dir, "*.dll"))
                {
                    var assembly = Assembly.LoadFrom("dllPath");
                    AppDomain.CurrentDomain.Load(assembly.GetName());
                    var t = assembly.GetType("Client");
                    yield return Activator.CreateInstance<IClient>();
                }
            }
        }

        private static async Task CloneRepo(string giturl, string dirname)
        {
            var result = await Cli.Wrap("cmd.exe").SetArguments($"/c git clone {giturl} {dirname}").ExecuteAsync();
        }

        private static async Task DotnetRestore(string dirname)
        {
            await Cli.Wrap("cmd.exe").SetArguments($"/c cd {dirname} && dotnet restore").ExecuteAsync();
        }

        private static async Task DotnetBuild(string dirname)
        {
            await Cli.Wrap("cmd.exe").SetArguments($"/c cd {dirname} && dotnet build").ExecuteAsync();
        }

        private static MomaConfig LoadConfig() =>
        JsonConvert.DeserializeObject<MomaConfig>(File.ReadAllText(ConfigPath));

        private static void SaveConfig() => File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(Config));
    }

    public class MomaConfig
    {
        public string      BotsDir         { get; set; }
        public BotConfig[] BotConfigs      { get; set; }
        public string      NginxConfig     { get; set; }
        public string      CertificatesDir { get; set; }

        public struct BotConfig
        {
            public string Name    { get; set; }
            public string Token   { get; set; }
            public string GitRepo { get; set; }
        }
    }
}