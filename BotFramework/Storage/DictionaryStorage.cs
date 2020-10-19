using System.Collections.Generic;
using BotFramework.Clients;

namespace BotFramework.Storage
{
    public class DictionaryStorage : IClientStorage
    {
        static DictionaryStorage()
        {
            Clients = new Dictionary<long, Client>();
        }

        private static Dictionary<long, Client> Clients { get; }

        public Client? GetClient(long id)
        {
            if (Clients.ContainsKey(id))
                return Clients[id];
            else return null;
        }

        public void SetClient(long id, Client client)
        {
            if(Clients.ContainsKey(id))
                Clients[id] = client;
            else Clients.Add(id, client);
        }
    }
}