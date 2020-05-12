using System;
using System.Collections.Generic;
using BotFramework.Bot;
using Telegram.Bot.Types;

namespace BotFramework.Commands.Injectors
{
    public interface DependencyInjector
    {
        public IEnumerable<ICommand> GetPossible(IEnumerable<Type> commandTypes, Update tgUpdate, IGetOnlyClient client);
    }
}