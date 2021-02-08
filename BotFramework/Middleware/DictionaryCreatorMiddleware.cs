using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace BotFramework.Middleware
{
    public class DictionaryContext
    {
        /// <summary>
        ///     First not done handler will handle CurrentUpdate
        /// </summary>
        public LinkedList<IUpdateConsumer> Handlers { get; set; } = new();
    }

    public class DictionaryCreatorMiddleware
    {
        private readonly ConcurrentDictionary<long, LinkedList<IUpdateConsumer>> dictionary = new();


        private readonly UpdateDelegate _next;

        public DictionaryCreatorMiddleware(UpdateDelegate next)
        {
            _next = next;
        }

        public Task Invoke(Update update, DictionaryContext dictionaryContext)
        {
            dictionary.AddOrUpdate(update.Message.Chat.Id, _ => new LinkedList<IUpdateConsumer>(),
                (_, list) => new LinkedList<IUpdateConsumer>(list.Where(t => !t.IsDone)));
            dictionaryContext.Handlers = dictionary[update.Message.Chat.Id];
            return _next.Invoke(update);
        }
    }
}