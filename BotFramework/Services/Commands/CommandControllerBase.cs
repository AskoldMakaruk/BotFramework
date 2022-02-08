using BotFramework.Abstractions;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;

namespace BotFramework.Services.Commands;

public abstract class CommandControllerBase : ICommandController
{
    protected CommandControllerBase(IClient client, UpdateContext update)
    {
        Client  = client;
        Update  = update.Update;
        Context = update;
    }

    public IClient       Client  { get; internal set; }
    public Update        Update  { get; internal set; }
    public UpdateContext Context { get; internal set; }

    void ICommandController.Init(UpdateContext update, IClient client)
    {
        Client  = client;
        Update  = update.Update;
        Context = update;
    }
}

public abstract class PersistentControllerBase : CommandControllerBase
{
    public virtual string Name => GetType().Name;

    public int State { get; }

    protected PersistentControllerBase([NotNull] IClient client, [NotNull] UpdateContext update) : base(client, update)
    {
        State = update.GetUserCommandStateFromCache()!.State;
    }

    public void SetState(int state)
    {
        var storage = Context.RequestServices.GetService<IPersistentCommandStorage>();

        storage.SetUserCommandState(Client.UserId,
            new UserCommandState() { State = state, EndpointName = Name, UserId = Client.UserId });
    }
}