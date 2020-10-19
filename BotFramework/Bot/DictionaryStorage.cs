using System.Collections.Generic;
using BotFramework.BotTask;
using BotFramework.Commands;

namespace BotFramework.Bot
{
    public class DictionaryStorage : IClientStorage
    {
        static DictionaryStorage()
        {
            Clients = new Dictionary<long, PerUserClient>();
        }

        private static Dictionary<long, PerUserClient> Clients { get; }

        public PerUserClient? GetClient(long id)
        {
            if (Clients.ContainsKey(id))
                return Clients[id];
            else return null;
        }

        public void SetClient(long id, PerUserClient client)
        {
            if(Clients.ContainsKey(id))
                Clients[id] = client;
            else Clients.Add(id, client);
        }
    }
}