using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotFramework.Clients;
using BotFramework.Commands;
using BotFramework.Injectors;
using Telegram.Bot;

namespace BotFramework.Middleware
{
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
}