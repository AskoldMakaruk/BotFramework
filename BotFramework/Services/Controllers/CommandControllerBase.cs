using BotFramework.Abstractions;
using Telegram.Bot.Types;

namespace BotFramework.Services.Controllers;

public abstract class CommandControllerBase : ICommandController
{
    protected CommandControllerBase(IClient client, Update update)
    {
        Client = client;
        Update = update;
    }

    public IClient Client { get; internal set; }
    public Update  Update { get; internal set; }

    void ICommandController.Init(Update update, IClient client)
    {
        Client = client;
        Update = update;
    }
}