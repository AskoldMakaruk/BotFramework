using BotFramework.Clients;

namespace BotFramework.Storage
{
    public interface IClientStorage
    {
        Client? GetClient(long id);
        void           SetClient(long id, Client client);
    }
}