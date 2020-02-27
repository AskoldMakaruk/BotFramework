using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using BotFramework.Commands;
using Optional;
using Serilog;
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
            var logger = configuration.Logger.FromOptional(Logger.None);
            var (staticCommands, onStartCommands) =
            configuration.Assembly.FromOptional(
                assembly => (GetStaticCommands(assembly).Concat(configuration.Commands).ToList(),
                                GetOnStartCommand(assembly).Concat(configuration.OnStartCommands).ToList()),
                (configuration.Commands, configuration.OnStartCommands));
            configuration.Assembly.FromOptional(assembly => GetStaticCommands(assembly).Concat(configuration.Commands),
                configuration.Commands);
            var client = from name in configuration.Name
                         from token in configuration.Token
                         let botConf = new BotConfiguration
                         {
                             Name    = name, Token = token, Commands = staticCommands,
                             Logger  = logger,
                             Webhook = configuration.Webhook, OnStartCommands = onStartCommands
                         }
                         select new Client(botConf);

            if (client.IsEmpty)
                throw new ArgumentException("");
            return client.FromOptional((Client) null);
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

        public BotBuilder WithStaticCommands(List<ICommand> commands)
        {
            configuration.Commands = commands;
            return this;
        }

        public BotBuilder OnStartCommand(List<ICommand> command)
        {
            configuration.OnStartCommands = command;
            return this;
        }

        protected static List<ICommand> GetStaticCommands(Assembly assembly)
        {
            return assembly
                   .GetTypes()
                   .Where(t => (t.IsSubclassOf(typeof(T)) || t.GetInterfaces().Contains(typeof(T))) && !t.IsAbstract)
                   .Where(c => !getStatic &&
                               c.IsDefined(typeof(StaticCommandAttribute), true))
                   .Where(t => (t.IsSubclassOf(typeof(ICommand)) || t.GetInterfaces().Contains(typeof(ICommand))) &&
                               !t.IsAbstract)
                   .Where(c =>
#pragma warning disable 618
                   (c.GetInterfaces().Contains(typeof(IStaticCommand)) ||
#pragma warning restore 618
                    c.IsDefined(typeof(StaticCommandAttribute), true)))
                   .Select(Activator.CreateInstance)
                   .Cast<ICommand>()
                   .ToList();
        }

        protected static List<ICommand> GetOnStartCommand(Assembly assembly)
        {
            return assembly
                   .GetTypes()
                   .Where(t => (t.IsSubclassOf(typeof(ICommand)) || t.GetInterfaces().Contains(typeof(ICommand))) &&
                               !t.IsAbstract)
                   .Where(c =>
                   c.IsDefined(typeof(OnStartCommand), true))
                   .Select(Activator.CreateInstance)
                   .Cast<ICommand>()
                   .ToList();
        }
    }
}