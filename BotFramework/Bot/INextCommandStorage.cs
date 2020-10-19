using System.Collections.Generic;
using BotFramework.BotTask;
using BotFramework.Commands;
using BotFramework.Responses;

namespace BotFramework.Bot
{
    public interface IClientStorage
    {
        PerUserClient?     GetClient(long id);
        void               SetClient(long id, PerUserClient client);
    }
}