using System.Collections.Generic;
using BotFramework.Commands;

namespace BotFramework.Bot
{
    public interface INextCommandStorage
    {
        void                  SetNextCommands(long chatId, IEnumerable<ICommand> commands);
        IEnumerable<ICommand> GetCommands(long     chatId);
    }
}