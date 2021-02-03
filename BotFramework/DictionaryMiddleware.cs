using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotFramework.Clients;
using BotFramework.Commands;
using BotFramework.Injectors;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotFramework
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

    public class SuitableFirstMiddleware<T> : IMiddleware<T> where T : IStaticCommandsContext
    {
        private readonly IInjector               injector;
        private          List<IStaticCommand<T>> commands = null!;
        private          bool                    IsConfigured;

        public SuitableFirstMiddleware(IInjector injector)
        {
            this.injector = injector;
        }

        public IMiddleware<T> Next { get; set; } = null!;

        public Task Invoke(T context)
        {
            if (!IsConfigured)
            {
                var scope = injector.UseScope();
                commands     = context.StaticCommands.Select(scope.Get).Cast<IStaticCommand<T>>().ToList();
                IsConfigured = true;
            }

            var command = commands.FirstOrDefault(t => t.SuitableFirst(context));
            if (command is not null)
            {
                command = (IStaticCommand<T>) injector.UseScope().Get(command.GetType());
                context.Handlers.AddFirst(new Client<T>(command, context, injector.Get<ITelegramBotClient>()));
            }

            return Next.Invoke(context);
        }
    }

    public class SuitableLastMiddleware<T> : IMiddleware<T> where T : IStaticCommandsContext
    {
        private readonly IInjector               injector;
        private          List<IStaticCommand<T>> commands = null!;
        private          bool                    IsConfigured;

        public SuitableLastMiddleware(IInjector injector)
        {
            this.injector = injector;
        }

        public IMiddleware<T> Next { get; set; } = null!;

        public Task Invoke(T context)
        {
            if (!IsConfigured)
            {
                var scope = injector.UseScope();
                commands     = context.StaticCommands.Select(scope.Get).Cast<IStaticCommand<T>>().ToList();
                IsConfigured = true;
            }

            var freeHandler = context.Handlers.FirstOrDefault(t => !t.IsDone);
            if (freeHandler is null)
            {
                var command = commands.FirstOrDefault(t => t.SuitableLast(context));
                if (command is not null)
                {
                    context.Handlers.AddFirst(new Client<T>(command, context, injector.Get<ITelegramBotClient>()));
                }
            }

            return Next.Invoke(context);
        }
    }

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