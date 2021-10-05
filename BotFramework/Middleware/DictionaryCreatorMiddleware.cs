using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using BotFramework.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;

namespace BotFramework.Middleware
{
    public class ContextDictionary
    {
        public readonly ConcurrentDictionary<long, Dictionary<Type, object>> Objects = new();

        public T? Get<T>(long id) where T : class
        {
            if (Objects.TryGetValue(id, out var objects) && objects.TryGetValue(typeof(T), out var res))
                return (T)res;
            return null;
        }

        public void Add<T>(long id, T item) where T : class =>
        Objects.AddOrUpdate(id,
            static (_, value) => new Dictionary<Type, object> { { typeof(T), value } },
            static (_, objects, item) =>
            {
                objects[typeof(T)] = item;
                return objects;
            }, item);

        public void Remove<T>(long id) => Objects.AddOrUpdate(id, _ => new(), (_, objects) =>
        {
            objects.Remove(typeof(T));
            return objects;
        });
    }

    public class DictionaryCreatorMiddleware
    {
        private readonly UpdateDelegate    _next;
        private readonly ContextDictionary _contextDictionary;

        public DictionaryCreatorMiddleware(UpdateDelegate next, ContextDictionary contextDictionary)
        {
            _next              = next;
            _contextDictionary = contextDictionary;
        }

        public Task Invoke(Update update, IServiceProvider provider)
        {
            if (update.GetId() is not { } id)
            {
                return _next.Invoke(update);
            }

            // does this part ever called?
            
            _contextDictionary.Objects.AddOrUpdate(id,
                _ => new Dictionary<Type, object>(),
                (_, objects) =>
                {
                    if (objects.Remove(typeof(IServiceProvider), out var serviceObj))
                    {
                        provider.SetWrappedService((IServiceProvider)serviceObj);
                    }

                    foreach (var (type, instance) in objects)
                    {
                        provider.SetWrappedService(type, instance);
                    }

                    return objects;
                });

            return _next.Invoke(update);
        }
    }

    public static class UseHandlersMiddleware
    {
        public static void UseHandlers(this IAppBuilder builder)
        {
            builder.Services.AddSingleton<ContextDictionary>();
            builder.UseMiddleware<DictionaryCreatorMiddleware>();
        }
    }
}