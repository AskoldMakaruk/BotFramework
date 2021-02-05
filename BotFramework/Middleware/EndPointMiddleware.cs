using System.Linq;
using System.Threading.Tasks;

namespace BotFramework.Middleware
{
    public class EndPointMiddleware<T> : IMiddleware<T> where T : IDictionaryContext
    {
        public IMiddleware<T> Next { get; set; }

        public Task Invoke(T context)
        {
            var handler = context.Handlers.FirstOrDefault(t => t.IsWaitingForUpdate);
            handler?.Consume(context.CurrentUpdate);
            return Task.CompletedTask;
        }
    }
}