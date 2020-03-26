using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using BotFramework.Commands;
using Serilog;
using Serilog.Core;

namespace BotFramework.Bot
{
    public class BotBuilder
    {
        private readonly BotConfiguration configuration;
        private          Assembly         _assembly;

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

        public BotConfiguration BuildConfiguration()
        {
            CheckConfiguration();
            return configuration;
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

            if (_assembly == null && configuration.Commands == null)
            {
                throw new ArgumentException("You must supply assembly or commands");
            }

            if (configuration.Storage == null)
            {
                configuration.Storage = new DictionaryStorage();
            }

            configuration.Logger ??= Logger.None;

            (configuration.Commands, configuration.StartCommands) =
            _assembly != null
            ? (
                  GetStaticCommands(_assembly)
                  .Concat(configuration.Commands)
                  .ToList(),
                  GetOnStartCommand(_assembly)
                  .Concat(configuration.StartCommands)
                  .ToList()
              )
            : (configuration.Commands, configuration.StartCommands);
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

        public BotBuilder UseConsoleLogger()
        {
            configuration.Logger = new LoggerConfiguration()
                                   .MinimumLevel.Debug()
                                   .WriteTo.Console()
                                   .Enrich.FromLogContext()
                                   .CreateLogger();
            return this;
        }

        public BotBuilder UseWebhook()
        {
            configuration.Webhook = true;
            return this;
        }

        public BotBuilder UseNextCommandStorage(INextCommandStorage storage)
        {
            configuration.Storage = storage;
            return this;
        }

#region Commands

        public BotBuilder UseAssembly(Assembly assembly)
        {
            _assembly = assembly;
            return this;
        }

        public BotBuilder WithStaticCommands(params ICommand[] commands)
        {
            return WithStaticCommands(commands.ToList());
        }

        public BotBuilder WithStaticCommands(IEnumerable<ICommand> commands)
        {
            configuration.Commands = commands.ToList();
            return this;
        }

        public BotBuilder WithStartCommands(params ICommand[] commands)
        {
            return WithStartCommands(commands.ToList());
        }

        public BotBuilder WithStartCommands(IEnumerable<ICommand> commands)
        {
            configuration.StartCommands = commands.ToList();
            return this;
        }


        protected static List<ICommand> GetStaticCommands(Assembly assembly)
        {
            return assembly
                   .GetTypes()
                   .Where(t => (t.IsSubclassOf(typeof(ICommand)) || t.GetInterfaces().Contains(typeof(ICommand))) &&
                               !t.IsAbstract)
                   .Where(c => c.IsDefined(typeof(StaticCommandAttribute), true))
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

#endregion
    }
}