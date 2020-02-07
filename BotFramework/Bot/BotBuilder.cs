using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using BotFramework.Commands;
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
            CheckConfiguration();
            var client = new Client(configuration);
            return client;
        }

        private void CheckConfiguration()
        {
            if (configuration.Token == null)
            {
                throw new ArgumentNullException(nameof(configuration.Token));
            }

            if (!Regex.IsMatch(configuration.Token, "\\d{9}:[0-9A-Za-z_-]{35}"))
            {
                throw new ArgumentException("Invalid telegram api token.");
            }

            if (configuration.Assembly == null && configuration.Commands == null)
            {
                throw new ArgumentException("You must supply assembly or commands");
            }

            if (configuration.Logger == null)
            {
                configuration.Logger = Logger.None;
            }

            if (configuration.Commands == null)
            {
                configuration.Commands = LoadTypeFromAssembly<ICommand>(configuration.Assembly);
            }
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

        public BotBuilder WithStaticCommands(IEnumerable<ICommand> commands)
        {
            configuration.Commands = commands;
            return this;
        }

        protected static IEnumerable<T> LoadTypeFromAssembly<T>(Assembly assembly, bool getStatic = false)
        {
            return assembly
                   .GetTypes()
                   .Where(t => (t.IsSubclassOf(typeof(T)) || t.GetInterfaces().Contains(typeof(T))) && !t.IsAbstract)
                   .Where(c => !getStatic &&
                               c.IsDefined(typeof(StaticCommandAttribute), true))
                   .Select(Activator.CreateInstance)
                   .Cast<T>();
        }
    }
}