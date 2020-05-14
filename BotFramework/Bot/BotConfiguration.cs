using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using BotFramework.Commands;
using BotFramework.Commands.Injectors;
using BotFramework.Commands.Validators;
using Serilog;

namespace BotFramework.Bot
{
    public class BotConfiguration : IBotConfiguration
    {
        public bool Webhook { get; set; }

        private ILogger    logger;
        private List<Type> commands;
        private List<Type> startCommands;

        public BotConfiguration(Assembly assembly, string token)
        {
            Storage       = new DictionaryStorage();
            commands      = GetStaticCommands(assembly);
            startCommands = GetOnStartCommand(assembly);

            if (!Regex.IsMatch(token, "\\d{9}:[0-9A-Za-z_-]{35}"))
                throw new ArgumentException("Invalid telegram api token.");
            Token = token;

            var validators = GetValidators(assembly);
            foreach (var (key, value) in GetValidators(typeof(BotConfiguration).Assembly))
                validators.Add(key, value);                
            Injector = new CompilerInjector(validators);
        }

        public string Token { get; }

        public ILogger             Logger  { get => logger ?? Serilog.Core.Logger.None; set => logger = value; }
        public INextCommandStorage Storage { get;                                       set; }

        public IReadOnlyCollection<Type> Commands      { get => commands;      set => commands = CheckICommand(value); }
        public IReadOnlyCollection<Type> StartCommands { get => startCommands; set => startCommands = CheckICommand(value); }

        public DependencyInjector Injector { get; set; }

        internal static List<Type> CheckICommand(IEnumerable<Type> input)
        {
            var output = new List<Type>();
            foreach (var command in input)
            {
                if (!command.GetInterfaces().Contains(typeof(ICommand)))
                    throw new InvalidCastException($"{command} does not implement ICommand");
                if (command.IsAbstract)
                    throw new InvalidCastException($"{command} is abstract");
                output.Add(command);
            }

            return output;
        }


        public static ILogger ConsoleLogger()
        {
            return new LoggerConfiguration()
                   .MinimumLevel.Debug()
                   .WriteTo.Console()
                   .Enrich.FromLogContext()
                   .CreateLogger();
        }

        public static List<Type> GetStaticCommands(Assembly assembly)
        {
            return assembly
                   .GetTypes()
                   .Where(t => (t.IsSubclassOf(typeof(ICommand)) || t.GetInterfaces().Contains(typeof(ICommand))) &&
                               !t.IsAbstract)
                   .Where(c => c.IsDefined(typeof(StaticCommandAttribute), true))
                   .ToList();
        }

        public static List<Type> GetOnStartCommand(Assembly assembly)
        {
            return assembly
                   .GetTypes()
                   .Where(t => (t.IsSubclassOf(typeof(ICommand)) || t.GetInterfaces().Contains(typeof(ICommand))) &&
                               !t.IsAbstract)
                   .Where(c =>
                   c.IsDefined(typeof(OnStartCommand), true))
                   .ToList();
        }

        public static Dictionary<Type, Type> GetValidators(Assembly assembly)
        {
            var res = from type in assembly.GetTypes()
                      from baseType in type.GetInterfaces()
                      where !type.IsAbstract       &&
                            !type.IsInterface      &&
                            baseType != null       &&
                            baseType.IsGenericType &&
                            typeof(Validator<>).IsAssignableFrom(baseType.GetGenericTypeDefinition())
                      select new KeyValuePair<Type, Type>(baseType.GetGenericArguments().First(), type);
            return new Dictionary<Type, Type>(res);
        }
    }
}