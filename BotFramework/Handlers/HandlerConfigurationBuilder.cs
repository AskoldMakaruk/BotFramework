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

        public  IReadOnlyList<Type>?                  CommandTypes     { get; set; }
        public  IInjector?                            CommandInjector  { get; set; }
        public  ILogger?                              Logger           { get; set; }
        public  TelegramBotClient?                    BotClient        { get; set; }
        public  HttpClient?                           CustomHttpClient { get; set; }
        private Assembly?                             assembly;
        private INinjectModule[]                      ninjectModules = null!;
        private Func<IReadOnlyList<Type>, IInjector>? injectorCreator;

        public HandlerConfigurationBuilder(string token, Assembly assembly)
        {
            this.Token    = token;
            this.assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
        }

        public HandlerConfigurationBuilder(string token, IReadOnlyList<Type> allCommands)
        {
            this.Token   = token;
            CommandTypes = allCommands ?? throw new ArgumentNullException(nameof(allCommands));
        }

        public HandlerConfigurationBuilder WithCustomHttpClient(HttpClient client)
        {
            CustomHttpClient = client;
            return this;
        }

        public HandlerConfigurationBuilder WithCustomNinjectModules(params INinjectModule[] modules)
        {
            ninjectModules = modules;
            return this;
        }

        public HandlerConfigurationBuilder WithLogger(ILogger logger)
        {
            Logger = logger;
            return this;
        }

        public HandlerConfigurationBuilder WithInjector(Func<IReadOnlyList<Type>, IInjector> injectorCreator)
        {
            this.injectorCreator = injectorCreator;
            return this;
        }

        public HandlerConfiguration Build()
        {
            {
                if (Token is null)
                    throw new ArgumentNullException(nameof(Token));
                if (!Regex.IsMatch(Token, "\\d{9}:[0-9A-Za-z_-]{35}"))
                    throw new ArgumentException("Invalid telegram api token.");
                BotClient ??= new TelegramBotClient(Token, CustomHttpClient);
            }
            Logger ??= Serilog.Core.Logger.None;
            if (CommandTypes is null)
            {
                if (assembly is null) throw new ArgumentNullException(nameof(assembly));
                CommandTypes = assembly.GetTypes()
                                       .Where(t => t.GetInterfaces().Contains(typeof(ICommand)) && !t.IsAbstract)
                                       .ToList();
            }

            CommandInjector ??= injectorCreator?.Invoke(CommandTypes);
            CommandInjector ??= new NinjectInjector(CommandTypes, ninjectModules);
            Logger.Debug("Loading static commands...");
            var staticCommands = CommandTypes.Where(t => t.GetInterfaces().Contains(typeof(IStaticCommand)) && !t.IsAbstract)
                                         .Select(t => (CommandInjector.Create(t) as IStaticCommand, t))
                                         .ToList()!;
            Logger.Debug("Loaded {StaticCommandsCount} commands.", staticCommands.Count);
            Logger.Debug("{StaticCommands}",
                string.Join(',', staticCommands.Select(c => c.Item1.GetType().Name)));
            return new HandlerConfiguration()
            {
                StaticCommands = staticCommands,
                CommandInjector = CommandInjector,
                Logger = Logger,
                BotClient = BotClient
            };
        }

    }
}