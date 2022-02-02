using Telegram.Bot.Types;

namespace BotFramework.Abstractions;

public interface ICommandController
{
    public IClient Client { get; }

    public Update Update { get; }

    internal void Init(UpdateContext update, IClient client);
}