using System.Collections.Generic;
using System.Threading.Tasks;

namespace BotFramework.Middleware
{
    public class CancelCommandMiddleware<T> : IMiddleware<T> where T : IDictionaryContext
    {
        public IMiddleware<T> Next { get; set; } = null!;

        public Task Invoke(T context)
        {
            if (context.CurrentUpdate.Message?.Text == "/cancel")
            {
                context.Handlers = new LinkedList<IUpdateConsumer>();
                return Task.CompletedTask;
            }

            return Next.Invoke(context);
        }
    }
}