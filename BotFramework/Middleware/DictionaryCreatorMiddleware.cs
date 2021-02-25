using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using BotFramework.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;

namespace BotFramework.Middleware
{
    public class DictionaryContext
    {
        public ConcurrentDictionary<long, IServiceProvider> Providers { get; set; } = new();
    }

    public class Consumers
    {
        public LinkedList<IUpdateConsumer> Handlers { get; set; } = new();
    }

    public class DictionaryCreatorMiddleware
    {
        private readonly UpdateDelegate _next;

        public DictionaryCreatorMiddleware(UpdateDelegate next)
        {
            _next = next;
        }

        public Task Invoke(Update update, DictionaryContext dictionaryContext, WrappedServiceProvider provider)
        {
            if (update.GetId() is not { } id)
            {
                return _next.Invoke(update);
            }

            dictionaryContext.Providers.TryAdd(id, provider.Provider);
            provider.Provider = dictionaryContext.Providers[id];
            var consumers = provider.Provider.GetService<Consumers>();
            return _next.Invoke(update);
        }
    }

    public static class UseHandlersMiddleware
    {
        public static void UseHandlers(this IAppBuilder builder)
        {
            builder.Services.AddSingleton<DictionaryContext>();
            builder.Services.AddScoped<Consumers>();
            builder.UseMiddleware<DictionaryCreatorMiddleware>();
        }
    }
}