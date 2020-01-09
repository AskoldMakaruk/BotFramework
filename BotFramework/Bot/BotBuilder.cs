using System.Reflection;
using Serilog.Core;

namespace BotFramework.Bot
{
    public class BotBuilder
    {
        private readonly BotConfiguration configuration;

        public BotBuilder()
        {
            configuration = new BotConfiguration
            {
                Webhook = false
            };
        }

        public Client Build()
        {
            if (configuration.Logger == null)
            {
                configuration.Logger = Logger.None;
            }

            var client = new Client(configuration);
            return client;
        }

        public BotBuilder WithName(string name)
        {
            configuration.Name = name;
            return this;
        }

        public BotBuilder WithToken(string token)
        {
            configuration.Token = token;
            return this;
        }

        public BotBuilder UseLogger(Logger logger)
        {
            configuration.Logger = logger;
            return this;
        }

        public BotBuilder UseWebhook()
        {
            configuration.Webhook = true;
            return this;
        }

        public BotBuilder UseAssembly(Assembly assembly)
        {
            configuration.Assembly = assembly;
            return this;
        }
    }
}