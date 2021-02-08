using System;
using BotFramework.Abstractions;
using Telegram.Bot.Types;

namespace BotFramework.Clients.ClientExtensions
{
    public static class UpdateFilterExtensions
    {
        public static UpdateFilter<Update> GetUpdateFilter(this IClient client)
        {
            return new(client, x => x, new());
        }

        public static UpdateFilter<TResult> Select<TResult, TSource>(this UpdateFilter<TSource> filter,
                                                                     Func<TSource, TResult>     mapper)
        {
            return new(filter.client, update => mapper(filter.Mapper(update)), filter.Filters);
        }

        public static UpdateFilter<T> Where<T>(this UpdateFilter<T> filter, Func<T, bool> andFunc)
        {
            filter.Filters.Add(update => andFunc(filter.Mapper(update)));
            return filter;
        }
    }
}