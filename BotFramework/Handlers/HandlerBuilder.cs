using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Akka.Routing;
using BotFramework.Clients;
using BotFramework.Commands;
using BotFramework.Injectors;
using Serilog;
using Serilog.Core;
using Telegram.Bot;

namespace BotFramework.Bot
{
    public class HandlerBuilder
    {
        public IReadOnlyList<(IStaticCommand, Type)> StaticCommands  { get; private set; } = null!;
        public IReadOnlyList<Type>                   CommandTypes    { get; private set; } = null!;
        public IInjector                             CommandInjector { get; private set; }
        public ILogger                               Logger          { get; private set; }
        public TelegramBotClient                     BotClient       { get; private set; } = null!;

        public HandlerBuilder(string     token,
                              Assembly   assembly,
                              IInjector? injector = null,
                              ILogger?   logger   = null)
        {
            InitClient(token);
            Logger = logger ?? Serilog.Core.Logger.None;
            CommandTypes = assembly.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(ICommand)) && !t.IsAbstract).ToList();
            CommandInjector = injector ?? new NinjectInjector(CommandTypes);
            LoadStaticCommands(CommandTypes);
        }

        public HandlerBuilder(string              token,
                              IReadOnlyList<Type> allCommands,
                              IInjector?          injector = null,
                              ILogger?            logger   = null
        )
        {
            InitClient(token);
            Logger       = logger      ?? Serilog.Core.Logger.None;
            CommandTypes = allCommands ?? throw new ArgumentNullException(nameof(allCommands));

            CommandInjector = injector ?? new NinjectInjector(CommandTypes);
            LoadStaticCommands(allCommands);
        }

        private void InitClient(string token)
        {
            if (token == null)
                throw new ArgumentNullException(nameof(token));
            if (!Regex.IsMatch(token, "\\d{9}:[0-9A-Za-z_-]{35}"))
                throw new ArgumentException("Invalid telegram api token.");
            BotClient = new TelegramBotClient(token);
        }

        private void LoadStaticCommands(IReadOnlyList<Type> commandTypes)
        {
            Logger.Debug("Loading static commands...");
            StaticCommands = commandTypes.Where(t => t.GetInterfaces().Contains(typeof(IStaticCommand)) && !t.IsAbstract)
                                         .Select(t => (CommandInjector.Create(t) as IStaticCommand, t))
                                         .ToList()!;
            Logger.Debug("Loaded {StaticCommandsCount} commands.", StaticCommands.Count);
            Logger.Debug("{StaticCommands}",
                string.Join(',', StaticCommands.Select(c => c.Item1.GetType().Name)));
        }
    }
}