using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Akka.Routing;
using BotFramework.Clients;
using BotFramework.Commands;
using Serilog;
using Serilog.Core;
using Telegram.Bot;

namespace BotFramework.Bot
{
    public class HandlerBuilder
    {
        public IReadOnlyList<(IStaticCommand, Type)> StaticCommands  { get; private set; } = null!;
        public IInjector                             CommandInjector { get; private set; }
        public ILogger                               Logger          { get; private set; }
        public TelegramBotClient                     BotClient       { get; private set; } = null!;

        public HandlerBuilder(string    token,
                              IInjector injector,
                              Assembly  assembly,
                              ILogger?  logger = null)
        {
            InitClient(token);
            Logger          = logger   ?? Serilog.Core.Logger.None;
            CommandInjector = injector ?? throw new ArgumentNullException(nameof(injector));
            LoadCommands(GetStaticCommands(assembly));
        }

        public HandlerBuilder(string               token,
                              IInjector            injector,
                              ILogger?             logger         = null,
                              IReadOnlyList<Type>? staticCommands = null,
                              IReadOnlyList<Type>? startCommands  = null)
        {
            InitClient(token);
            Logger          = logger   ?? Serilog.Core.Logger.None;
            CommandInjector = injector ?? throw new ArgumentNullException(nameof(injector));
            LoadCommands(staticCommands);
        }

        private void InitClient(string token)
        {
            if (token == null)
                throw new ArgumentNullException(nameof(token));
            if (!Regex.IsMatch(token, "\\d{9}:[0-9A-Za-z_-]{35}"))
                throw new ArgumentException("Invalid telegram api token.");
            BotClient = new TelegramBotClient(token);
        }

        private void LoadCommands(IReadOnlyList<Type>? staticCommands)
        {
            if (staticCommands == null)
                throw new ArgumentException("You must supply at least one static command");
            Logger.Debug("Loading static commands...");
            StaticCommands = staticCommands
                             .Select(t => (CommandInjector.Create(t) as IStaticCommand, t))
                             .ToList()!;
            Logger.Debug("Loaded {StaticCommandsCount} commands.", StaticCommands.Count);
            Logger.Debug("{StaticCommands}",
                string.Join(',', StaticCommands.Select(c => c.Item1.GetType().Name)));
        }

        private static List<Type> GetStaticCommands(Assembly assembly)
        {
            return assembly
                   .GetTypes()
                   .Where(t => (t.IsSubclassOf(typeof(ICommand)) || t.GetInterfaces().Contains(typeof(IStaticCommand))) &&
                               !t.IsAbstract)
                   .ToList();
        }

    }
}