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
    public class DictionaryContext
    {
        public readonly ConcurrentDictionary<long, Dictionary<Type, object>> _objects = new();

        public T? Get<T>(long id) where T : class
        {
            if (_objects.TryGetValue(id, out var objects) && objects.TryGetValue(typeof(T), out var res))
                return (T) res;
            return null;
        }

        public void Add<T>(long id, T item) where T : class => _objects.AddOrUpdate(id,
            (_, item) => new() {{typeof(T), item}},
            (_, objects, item) =>
            {
                objects[typeof(T)] = item;
                return objects;
            }, item);

        public void Remove<T>(long id) => _objects.AddOrUpdate(id, _ => new(), (_, objects) =>
        {
            objects.Remove(typeof(T));
            return objects;
        });
    }

    public class DictionaryCreatorMiddleware
    {
        private readonly UpdateDelegate    _next;
        private readonly DictionaryContext _dictionaryContext;

        public DictionaryCreatorMiddleware(UpdateDelegate next, DictionaryContext dictionaryContext)
        {
            _next                   = next;
            _dictionaryContext = dictionaryContext;
        }

        public Task Invoke(Update update, IServiceProvider provider)
        {
            if (update.GetId() is not { } id)
            {
                return _next.Invoke(update);
            }

            _dictionaryContext._objects.AddOrUpdate(id, _ => new(), (_, objects) =>
            {
                if (objects.Remove(typeof(IServiceProvider), out var serviceObj))
                {
                    provider.SetWrappedService((IServiceProvider) serviceObj);
                }

                foreach (var (type, instance) in objects)
                    provider.SetWrappedService(type, instance);
                return objects;
            });
            return _next.Invoke(update);
        }
    }

    public static class UseHandlersMiddleware
    {
        public static void UseHandlers(this IAppBuilder builder)
        {
            builder.Services.AddSingleton<DictionaryContext>();
            builder.UseMiddleware<DictionaryCreatorMiddleware>();
        }
    }
}