using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace BotFramework.Middleware
{
    public class DictionaryCreatorMiddleware<T> : IMiddleware<T> where T : IDictionaryContext
    {
        private readonly ConcurrentDictionary<User, LinkedList<IUpdateConsumer>> dictionary = new();

        public IMiddleware<T> Next { get; set; } = null!;

        public Task Invoke(T context)
        {
            dictionary.AddOrUpdate(context.ChatId, _ => new LinkedList<IUpdateConsumer>(),
                (_, list) => new LinkedList<IUpdateConsumer>(list.Where(t => !t.IsDone)));
            context.Handlers = dictionary[context.ChatId];
            return Next.Invoke(context);
        }
    }
}