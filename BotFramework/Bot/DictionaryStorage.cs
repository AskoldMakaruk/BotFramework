using System;
using System.Collections.Generic;
using BotFramework.Commands;

namespace BotFramework.Bot
{
    public class DictionaryStorage : INextCommandStorage
    {
        static DictionaryStorage()
        {
            NextCommands = new Dictionary<long, IEnumerable<Type>>();
        }

        private static Dictionary<long, IEnumerable<Type>> NextCommands { get; }

        public void SetNextCommands(long chatId, IEnumerable<Type> commands)
        {
            NextCommands[chatId] = commands;
        }

        public IEnumerable<Type> GetCommands(long chatId) => NextCommands.ContainsKey(chatId) ? NextCommands[chatId] : new List<Type>();
    }
}