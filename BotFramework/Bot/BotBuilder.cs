using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BotFramework.Commands;
using Monad;
using Serilog;
using Serilog.Core;

namespace BotFramework.Bot
{
    public class BotBuilder
    {
        class MBotConfiguration
        {
            public bool                  Webhook  { get; set; }
            public Optional<string>      Token    { get; set; }
            public Optional<string>      Name     { get; set; }
            public Optional<Assembly>    Assembly { get; set; }
            public Optional<ILogger>     Logger   { get; set; }
            public IEnumerable<ICommand> Commands { get; set; } = new List<ICommand>();
            public Optional<ICommand> OnStartCommand { get; set; } 
        }

        private readonly MBotConfiguration configuration;

        public BotBuilder()
        {
            configuration = new MBotConfiguration
            {
                Webhook = false
            };
        }

        public Client Build()
        {
            var logger = configuration.Logger.FromOptional(Logger.None);
            var commands =
            configuration.Assembly.FromOptional(t => LoadTypeFromAssembly<ICommand>(t).Concat(configuration.Commands),
                configuration.Commands);
            var client = from name in configuration.Name
                         from token in configuration.Token
                         let botConf = new BotConfiguration
                         {
                             Name    = name, Token = token, Commands = commands.ToList(), Logger = logger,
                             Webhook = configuration.Webhook, OnStartCommand = configuration.OnStartCommand
                         }
                         select new Client(botConf);

            if (client.IsEmpty)
                throw new ArgumentException("");
            return client.FromOptional((Client) null);
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
        public BotBuilder OnStartCommand(ICommand command)
        {
            configuration.OnStartCommand = command.ToOptional();
            return this;
        }

        protected static IEnumerable<T> LoadTypeFromAssembly<T>(Assembly assembly, bool getStatic = false)
        {
            return assembly
                   .GetTypes()
                   .Where(t => (t.IsSubclassOf(typeof(T)) || t.GetInterfaces().Contains(typeof(T))) && !t.IsAbstract)
                   .Where(c => !getStatic &&
                               (c.GetInterfaces().Contains(typeof(IStaticCommand)) ||
                                c.IsDefined(typeof(StaticCommandAttribute), true)))
                   .Select(Activator.CreateInstance)
                   .Cast<T>();
        }
    }
}