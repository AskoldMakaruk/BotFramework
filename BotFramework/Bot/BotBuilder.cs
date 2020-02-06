using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            if (configuration.Logger == null)
            {
                configuration.Logger = Logger.None;
            }

            if (configuration.Commands == null)
            {
                configuration.Commands = LoadTypeFromAssembly<ICommand>(configuration.Assembly);
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