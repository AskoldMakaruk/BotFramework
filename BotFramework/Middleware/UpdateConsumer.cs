using System.Collections.Concurrent;
using System.Linq;
using BotFramework.Abstractions;

namespace BotFramework.Middleware
{
    public class UpdateConsumer
    {
        private readonly ConcurrentBag<IUpdateConsumer> consumers = new();

        public void Consume(UpdateContext   context,
                            IUpdateConsumer client)
        {
            var commands = context.Endpoints;
            if (commands.FirstOrDefault(a => a?.Priority == EndpointPriority.First) is { } first)
            {
                client.Initialize(first.Action, context.Update);
                consumers.Add(client);
            }
            else if (consumers.FirstOrDefault(a => !a.IsDone) is { } consumer)
            {
                consumer.Consume(context.Update);
            }
            else if (commands.FirstOrDefault(a => a?.Priority == EndpointPriority.Last) is { } last)
            {
                client.Initialize(last.Action, context.Update);
                consumers.Add(client);
            }
        }
    }
}