using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using BotFramework.Commands;
using BotFramework.Injectors;
using Ninject.Modules;
using Serilog;
using Telegram.Bot;

namespace BotFramework.Handlers
{
    public class HandlerConfigurationBuilder
    {
        public string Token { get; set; }

        public IReadOnlyList<Type>? CommandTypes     { get; set; }
        public IInjector?           CommandInjector  { get; set; }
        public ILogger?             Logger           { get; set; }
        public TelegramBotClient?   BotClient        { get; set; }
        public HttpClient?          CustomHttpClient { get; set; }

        private Assembly?                             Assembly;
        private Func<IReadOnlyList<Type>, IInjector>? InjectorCreator;

        private readonly List<INinjectModule> NinjectModules = new List<INinjectModule>();

        /// <summary>
        /// Initializes builder.
        /// </summary>
        /// <param name="token">Telegram bot token.</param>
        /// <param name="assembly">Assembly with all needed commands.</param>
        public HandlerConfigurationBuilder(string token, Assembly? assembly = null)
        {
            Token = token;
            Assembly = assembly
                       ?? AppDomain.CurrentDomain.GetAssemblies()
                                   .FirstOrDefault(a => a.GetTypes().Any(t => t.Name == "Program"))
                       ?? throw new ArgumentNullException(nameof(assembly));
        }

        public HandlerConfigurationBuilder(string token, IReadOnlyList<Type> allCommands)
        {
            Token        = token;
            CommandTypes = allCommands ?? throw new ArgumentNullException(nameof(allCommands));
        }

        public HandlerConfigurationBuilder WithCustomHttpClient(HttpClient client)
        {
            CustomHttpClient = client;
            return this;
        }

        public HandlerConfigurationBuilder WithCustomNinjectModules(params INinjectModule[] modules)
        {
            NinjectModules.AddRange(modules);
            return this;
        }

        public HandlerConfigurationBuilder WithLogger(ILogger logger)
        {
            Logger = logger;
            return this;
        }

        public HandlerConfigurationBuilder WithInjector(Func<IReadOnlyList<Type>, IInjector> injectorCreator)
        {
            InjectorCreator = injectorCreator;
            return this;
        }

        public HandlerConfigurationBuilder UseConsoleDefaultLogger()
        {
            Logger = new LoggerConfiguration()
                     .MinimumLevel.Debug()
                     .WriteTo.Console()
                     .Enrich.FromLogContext()
                     .CreateLogger();
            return this;
        }

        public HandlerConfiguration Build()
        {
            if (!Regex.IsMatch(Token, "\\d{9}:[0-9A-Za-z_-]{35}"))
                throw new ArgumentException("Invalid telegram api token.");

            BotClient ??= new TelegramBotClient(Token, CustomHttpClient);
            Logger    ??= Serilog.Core.Logger.None;

            Logger.Information("Starting bot...");
            var me = BotClient.GetMeAsync().Result;
            Logger.Information("Name: {BotFirstName} UserName: @{BotName}", me.FirstName, me.Username);

            if (CommandTypes is null)
            {
                if (Assembly is null) throw new ArgumentNullException(nameof(Assembly));
                CommandTypes = Assembly.GetTypes()
                                       .Where(t => t.GetInterfaces().Contains(typeof(ICommand)) && !t.IsAbstract)
                                       .ToList();
            }
            
            CommandInjector ??= InjectorCreator?.Invoke(CommandTypes);
            CommandInjector ??= new NinjectInjector(CommandTypes, NinjectModules);

            Logger.Debug("Loading static commands...");

            IReadOnlyList<(IStaticCommand, Type)> staticCommands = CommandTypes
                                                                   .Where(t => t.GetInterfaces().Contains(typeof(IStaticCommand))
                                                                               && !t.IsAbstract)
                                                                   .Select(t => (CommandInjector.Create(t) as IStaticCommand, t))
                                                                   .ToList()!;

            Logger.Debug("Loaded {StaticCommandsCount} commands.", staticCommands.Count);
            Logger.Debug("{StaticCommands}", string.Join(", ", staticCommands.Select(c => c.Item1.GetType().Name)));

            return new HandlerConfiguration
            {
                StaticCommands  = staticCommands,
                CommandInjector = CommandInjector,
                Logger          = Logger,
                BotClient       = BotClient
            };
        }
    }
}