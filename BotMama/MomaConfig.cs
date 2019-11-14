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
            public string Name          { get; set; }
            public string Token         { get; set; }
            public string GitRepo       { get; set; }
            public string ProjectFolder { get; set; }
            public string Branch        { get; set; }
            public bool?  UseWebHook    { get; set; }

            [JsonIgnore] public string BinDir { get; set; }
        }
    }
}