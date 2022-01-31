using System.Linq;
using BotFramework.Services.Clients;

namespace BotFramework.Services;

public static class PriorityUpdateConsumerExtensions
{
    public static PriorityCommandExcecutor GetPriorityUpdateConsumer(this IFeatureCollection collection)
    {
        if (collection.FirstOrDefault(pair => pair.Key == typeof(PriorityCommandExcecutor)).Value is { } result)
        {
            return (PriorityCommandExcecutor)result;
        }

        var consumer = new PriorityCommandExcecutor();
        collection[typeof(PriorityCommandExcecutor)] = consumer;
        return consumer;
    }
}