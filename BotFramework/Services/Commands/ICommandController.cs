using BotFramework.Abstractions;
using Telegram.Bot.Types;

namespace BotFramework.Services.Commands;

public interface ICommandController
{
    public IClient Client { get; }

    public Update Update { get; }

    internal void Init(UpdateContext update, IClient client);
}