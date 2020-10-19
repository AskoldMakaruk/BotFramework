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
            if (configuration.Injector == null)
            {
                throw new ArgumentNullException(nameof(configuration.Injector));
            }
            if (configuration.Token == null)
            {
                throw new ArgumentNullException(nameof(configuration.Token));
            }

            if (!Regex.IsMatch(configuration.Token, "\\d{9}:[0-9A-Za-z_-]{35}"))
            {
                throw new ArgumentException("Invalid telegram api token.");
            }

            if (_assembly == null && configuration.AllCommands == null)
            {
                throw new ArgumentException("You must supply assembly or commands");
            }

            if (configuration.Storage == null)
            {
                configuration.Storage = new DictionaryStorage();
            }

            configuration.Logger ??= Logger.None;

            (configuration.StaticCommands, configuration.StartCommands) =
            _assembly != null
            ? (
                  GetStaticCommands(_assembly)
                  .Concat(configuration.StaticCommands)
                  .ToList(),
                  GetOnStartCommand(_assembly)
                  .Concat(configuration.StartCommands)
                  .ToList()
              )
            : (configuration.StaticCommands, configuration.StartCommands);
        }

        public BotBuilder WithToken(string token)
        {
            configuration.Token = token;
            return this;
        }
        public BotBuilder WithInjector(IInjector injector)
        {
            configuration.Injector = injector;
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

        public BotBuilder UseNextCommandStorage(IClientStorage storage)
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

        public BotBuilder WithStaticCommands(params Type[] commands)
        {
            return WithStaticCommands(commands.ToList());
        }

        public BotBuilder WithStaticCommands(IEnumerable<Type> commands)
        {
            configuration.StaticCommands = commands.ToList();
            return this;
        }

        public BotBuilder WithStartCommands(params Type[] commands)
        {
            return WithStartCommands(commands.ToList());
        }

        public BotBuilder WithStartCommands(IEnumerable<Type> commands)
        {
            configuration.StartCommands = commands.ToList();
            return this;
        }


        protected static List<Type> GetStaticCommands(Assembly assembly)
        {
            return assembly
                   .GetTypes()
                   .Where(t => (t.IsSubclassOf(typeof(ICommand)) || t.GetInterfaces().Contains(typeof(ICommand))) &&
                               !t.IsAbstract)
                   .Where(c => c.IsDefined(typeof(StaticCommandAttribute), true))
                   .ToList();
        }

        protected static List<Type> GetOnStartCommand(Assembly assembly)
        {
            return assembly
                   .GetTypes()
                   .Where(t => (t.IsSubclassOf(typeof(ICommand)) || t.GetInterfaces().Contains(typeof(ICommand))) &&
                               !t.IsAbstract)
                   .Where(c =>
                   c.IsDefined(typeof(OnStartCommand), true))
                   .ToList();
        }

#endregion
    }
}