using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using BotTypes.cs;
using Newtonsoft.Json;

namespace BotMama
{
    public static class Moma
    {
        public static char          S = Path.DirectorySeparatorChar;
        public static MomaConfig    Config  { get; set; }
        public static List<IClient> Clients { get; set; }

        public static void Configure(string momaConfigPath)
        {
            if (File.Exists(momaConfigPath))
                Config = JsonConvert.DeserializeObject<MomaConfig>(File.ReadAllText(momaConfigPath));
            else
            {
                Config = new MomaConfig
                {
                    BotsDir         = $"data{S}Bots",
                    CertificatesDir = $"data{S}Certificates",
                    NginxConfig     = $"data{S}nginx",
                };
                File.WriteAllText(momaConfigPath, JsonConvert.SerializeObject(Config));
            }

            Clients = LoadAssemblies(Config.BotsDir).ToList();

        }

        static IEnumerable<IClient> LoadAssemblies(string botsDir)
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