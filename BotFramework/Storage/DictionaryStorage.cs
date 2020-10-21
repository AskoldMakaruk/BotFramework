using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using BotFramework.Clients;

namespace BotFramework.Storage
{
    public class DictionaryStorage : IClientStorage
    {
        static DictionaryStorage()
        {
            Clients = new ConcurrentDictionary<long, Client>();
        }

        private static ConcurrentDictionary<long, Client> Clients { get; }


        public Client GetOrAdd(long id, Func<long, Client> clientCreator)
        {
            return Clients.GetOrAdd(id, clientCreator);
        }
    }
}