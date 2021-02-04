using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotFramework.Clients.ClientExtensions
{
    public static class ClientGetExtensions
    {
        public static UpdateFilter<Update> GetUpdateFilter(this IClient client)
        {
            return new(client, x => x, new());
        }

        public static UpdateFilter<Message> GetTextMessage(this IClient client)
        {
            return GetUpdateFilter(client)
                   .Where(u => !string.IsNullOrEmpty(u.Message?.Text))
                   .Select(t => t.Message);
        }

        public static UpdateFilter<Message> GetOnlyButtonResult(this IClient client, ReplyKeyboardMarkup replyMarkup)
        {
            return GetUpdateFilter(client)
                   .Where(u =>
                   replyMarkup.Keyboard.SelectMany(t => t).Any(t => t.Text == u.Message?.Text))
                   .Select(t => t.Message);
        }
    }

    public class UpdateFilter<T>
    {
        public readonly  List<Func<Update, bool>> Filters;
        public readonly  Func<Update, T>          Mapper;
        private readonly IClient                  client;

        public UpdateFilter(IClient client, Func<Update, T> mapper, List<Func<Update, bool>> filters)
        {
            this.client = client;
            Mapper      = mapper;
            Filters     = filters;
        }

        public ValueTaskAwaiter<T> GetAwaiter() =>
        GetTask().GetAwaiter();

        public async ValueTask<T> GetTask()
        {
            Func<Update, bool> f = u => Filters.Aggregate(true, (current, filter) => current && filter(u));
            var                u = await client.GetUpdate(f);
            return Mapper(u);
        }

        public UpdateFilter<T> Where(Func<T, bool> andFunc)
        {
            Filters.Add(update => andFunc(Mapper(update)));
            return this;
        }

        public UpdateFilter<T1> Select<T1>(Func<T, T1> mapper)
        {
            return new(client, update => mapper(Mapper(update)), Filters);
        }
    }
}