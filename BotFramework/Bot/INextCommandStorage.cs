using System;
using System.Collections.Generic;

namespace BotFramework.Bot
{
    public interface INextCommandStorage
    {
        void              SetNextCommands(long chatId, IEnumerable<Type> commands);
        IEnumerable<Type> GetCommands(long     chatId);
    }
}