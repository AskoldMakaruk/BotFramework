using BotFramework.Abstractions;
using Telegram.Bot.Types;

namespace BotFramework.Services.Controllers;

public interface ICommandController
{
    public IClient Client { get; }

    public Update Update { get; }

    internal void Init(Update update, IClient client);
}