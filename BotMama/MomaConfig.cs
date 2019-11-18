using Newtonsoft.Json;

namespace BotMama
{
    public class MomaConfig
    {
        public string      FrameworkPath { get; set; }
        public string      BotsDir       { get; set; }
        public BotConfig[] BotConfigs    { get; set; }

        public class BotConfig
        {
            public string Name       { get; set; }
            public string Token      { get; set; }
            public string GitRepo    { get; set; }
            public string SrcDir     { get; set; }
            public string BinDir     { get; set; }
            public string DataDir    { get; set; }
            public string Branch     { get; set; }
            public bool?  UseWebHook { get; set; }

            [JsonIgnore] public bool IsValid { get; set; } = false;
        }
    }
}