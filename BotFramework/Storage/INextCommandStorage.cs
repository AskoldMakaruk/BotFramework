using System;
using BotFramework.Clients;

namespace BotFramework.Storage
{
    public interface IClientStorage
    {
        Client GetOrAdd(long  id, Func<long, Client> clientCreator);
    }
}