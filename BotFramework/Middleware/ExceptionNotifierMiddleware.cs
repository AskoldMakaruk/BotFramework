using System;
using System.Threading.Tasks;

//todo migrate IBotContext to IContextCollection
namespace BotFramework.Middleware
{
    public class ExceptionNotifierMiddleware<T> : IMiddleware<T> where T : IBotContext
    {
        public long[] Chats { get; init; }

        public IMiddleware<T> Next { get; set; }

        public Task Invoke(T context)
        {
            throw new NotImplementedException();
        }
    }

    public static class ExceptionNotifierMiddlewareInjector
    {
        public static AppBuilder<IBotContext> UseExceptionMiddleware(this AppBuilder<IBotContext> builder)
        {
            builder.UseSingletonMiddleware<ExceptionNotifierMiddleware<IBotContext>>();
            return builder;
        }
    }
}