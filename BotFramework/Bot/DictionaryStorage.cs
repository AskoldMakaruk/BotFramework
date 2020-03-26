using System.Collections.Generic;
using BotFramework.Commands;

namespace BotFramework.Bot
{
    public class DictionaryStorage : INextCommandStorage
    {
        static DictionaryStorage()
        {
            NextCommands = new Dictionary<long, IEnumerable<ICommand>>();
        }

        private static Dictionary<long, IEnumerable<ICommand>> NextCommands { get; }

        public void SetNextCommands(long chatId, IEnumerable<ICommand> commands)
        {
            NextCommands[chatId] = commands;
        }

        public IEnumerable<ICommand> GetCommands(long chatId) => NextCommands.ContainsKey(chatId) ? NextCommands[chatId] : null;
    }
}