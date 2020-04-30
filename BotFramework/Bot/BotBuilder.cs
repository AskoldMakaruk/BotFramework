using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using BotFramework.Commands;
using BotFramework.Commands.Validators;
using Serilog;
using Serilog.Core;
using Telegram.Bot.Types;

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
                throw new ArgumentNullException(nameof(configuration.Token));

            if (!Regex.IsMatch(configuration.Token, "\\d{9}:[0-9A-Za-z_-]{35}"))
                throw new ArgumentException("Invalid telegram api token.");

            if (_assembly == null && configuration.Commands == null)
                throw new ArgumentException("You must supply assembly or commands");

            if (configuration.Storage == null)
                configuration.Storage = new DictionaryStorage();

            configuration.Logger ??= Logger.None;

            if (_assembly != null)
            {
                var commands      = new HashSet<Type>(configuration.Commands);
                var startCommands = new HashSet<Type>(configuration.Commands);
                commands.UnionWith(GetStaticCommands(_assembly));
                startCommands.UnionWith(GetOnStartCommand(_assembly));
                configuration.Commands = commands.ToList();
                configuration.StartCommands = startCommands.ToList();
                var validators = GetValidators(_assembly);
                foreach (var (t, validatorT) in validators)
                    configuration.Validators[t] = validatorT;
            }

            if (configuration.UseBuiltInValidators)
            {
                configuration.Validators[typeof(Message)] = typeof(MessageValidator);
                configuration.Validators[typeof(ParsedCallBackQuery)] = typeof(CallBackQueryValidator);
            }
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

        public BotBuilder UseAssembly(Assembly assembly)
        {
            _assembly = assembly;
            return this;
        }

#region Commands

        public BotBuilder WithStaticCommands(params ICommand[] commands)
        {
            return WithStaticCommands(commands.ToList());
        }

        public BotBuilder WithStaticCommands(IEnumerable<ICommand> commands)
        {
            configuration.Commands = commands.Select(t => t.GetType()).ToList();
            return this;
        }

        public BotBuilder WithStartCommands(params ICommand[] commands)
        {
            return WithStartCommands(commands.ToList());
        }

        public BotBuilder WithStartCommands(IEnumerable<ICommand> commands)
        {
            configuration.StartCommands = commands.Select(t => t.GetType()).ToList();
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

        protected static Dictionary<Type, Type> GetValidators(Assembly assembly)
        {
            return assembly.GetTypes()
                           .Where(t => t.GetInterfaces().Contains(typeof(Validator<>)) && !t.IsAbstract)
                           .ToDictionary(t => t.GenericTypeArguments[0], t => t);
        }

#endregion
    }
}