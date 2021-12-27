using System.Linq;
using BotFramework.Services.Clients;

namespace BotFramework.Services;

public static class PriorityUpdateConsumerExtensions
{
    public static PriorityUpdateConsumer GetPriorityUpdateConsumer(this IFeatureCollection collection)
    {
        if (collection.FirstOrDefault(pair => pair.Key == typeof(PriorityUpdateConsumer)).Value is { } result)
        {
            return (PriorityUpdateConsumer)result;
        }

        var consumer = new PriorityUpdateConsumer();
        collection[typeof(PriorityUpdateConsumer)] = consumer;
        return consumer;
    }
}