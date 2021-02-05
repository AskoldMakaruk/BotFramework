using System.Threading.Tasks;

namespace BotFramework.Middleware
{
    public class ExceptionNotifierMiddleware<T> : IMiddleware<T> where T : IBotContext
    {
        public long[] Chats { get; init; }

        public IMiddleware<T> Next { get; set; }

        public Task Invoke(T context) { }
    }

    public 
}