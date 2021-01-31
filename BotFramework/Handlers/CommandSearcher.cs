using System;
using System.Collections.Generic;
using System.Linq;
using BotFramework.Commands;
using BotFramework.Injectors;
using Telegram.Bot.Types;

namespace BotFramework.Handlers
{
    public class CommandSearcher
    {
        private readonly IReadOnlyList<(StaticCommand, Type)> StaticCommands;
        private readonly IInjector                            IInjector;
        public CommandSearcher(IReadOnlyList<(StaticCommand, Type)> staticCommands, IInjector injector)
        {
            StaticCommands = staticCommands;
            IInjector      = injector;
        }

        public StaticCommand? FindSuitableFirst(Update update)
        {
            var commandType = StaticCommands.Where(t => t.Item1.SuitableFirst(update)).Select(t => t.Item2).FirstOrDefault();
            if (commandType is not null)
                return (StaticCommand?) IInjector.Create(commandType);
            return null;
        }
        public StaticCommand? FindSuitableLast(Update update)
        {
            var commandType = StaticCommands.Where(t => t.Item1.SuitableLast(update)).Select(t => t.Item2).FirstOrDefault();
            if (commandType is not null)
                return (StaticCommand?) IInjector.Create(commandType);
            return null;
        }
    }
}