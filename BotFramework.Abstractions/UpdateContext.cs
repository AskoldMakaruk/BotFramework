using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;

namespace BotFramework.Abstractions;

public class UpdateContext
{
    public IClient          Client          { get; set; }
    public IServiceProvider RequestServices { get; set; }
    public ClaimsPrincipal  User            { get; set; }

    public readonly Update         Update;
    public readonly List<Endpoint> Endpoints = new();

    /// <summary>
    /// Gets or sets a key/value collection that can be used to share data within the scope of this request.
    /// </summary>
    public IDictionary<object, object?> Items { get; set; }


    public UpdateContext(Update update, IClient client, IServiceProvider requestServices)
    {
        Update          = update;
        Client          = client;
        RequestServices = requestServices;
    }
}

public class Endpoint
{
    public string           Name             { get; set; }
    public EndpointPriority Priority         { get; set; }
    public int?             CommandState     { get; set; }
    public CommandPredicate CommandPredicate { get; set; }
    public UpdateDelegate   Delegate         { get; set; }

    public List<CommandAttributeBase> Attributes { get; set; }
}

public delegate bool? CommandPredicate(UpdateContext update);

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public abstract class CommandAttributeBase : Attribute
{
    public virtual EndpointPriority? Priority                        { get; init; }
    public virtual bool?             Suitable(UpdateContext context) => null;
}

public interface IPersistentCommand : ICommand
{
    public string Name  { get; }
    public int    State { get; }
}

public abstract class PersistentCommandBase : IPersistentCommand
{
    public async Task Execute(UpdateContext context)
    {
        Sinc ??= context.RequestServices.GetService<IRequestSinc>()!;
        await Execute(context.Update, (await context.GetUserCommandStateFromDb())!.State);
    }

    public abstract Task Execute(Update update, int state);

    public abstract bool? Suitable(UpdateContext context);


    public virtual string Name  => ToString() ?? GetType().Name;
    public         int    State { get; }

    public IRequestSinc? Sinc { get; private set; }
}

public interface IUserCommandState
{
    long   UserId       { get; set; }
    string EndpointName { get; set; }
    int    State        { get; set; }
}

public static class UserCommandStateFeatureExtensions
{
    public static IUserCommandState? GetUserCommandStateFromCache(this UpdateContext context)
    {
        var id = context.Update.GetId();
        if (id == null)
        {
            return null;
        }

        var state = context.RequestServices.GetService<IUserCommandState>();
        if (state != null)
        {
            return state;
        }

        var storage = context.RequestServices.GetService<IUserScopeStorage>()?.Get(id.Value);
        state = storage?.Get<UserCommandStateFeature>()?.State;

        return state;
    }

    public static async Task<IUserCommandState?> GetUserCommandStateFromDb(this UpdateContext context)
    {
        var state = context.GetUserCommandStateFromCache();

        if (state != null)
        {
            return state;
        }

        var id      = context.Update.GetId();
        var service = context.RequestServices.GetService<IPersistentCommandStorage>();
        var task    = service?.GetUserCommandState(id.Value);
        var storage = context.RequestServices.GetService<IUserScopeStorage>()?.Get(id.Value);

        if (task != null)
        {
            state = await task;
        }

        if (state != null && storage != null)
        {
            storage.Set(new UserCommandStateFeature(state));
        }


        return state;
    }
}

public record UserCommandStateFeature(IUserCommandState State);

public interface IPersistentCommandStorage
{
    public Task<IUserCommandState> GetUserCommandState(long userId);

    public Task SetUserCommandState(long userId, IUserCommandState state);
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class PersistentStateAttribute : CommandAttributeBase
{
    public string EndpointName { get; set; }
    public int    State        { get; }

    public PersistentStateAttribute(int state)
    {
        State = state;
    }

    public PersistentStateAttribute(int state, string endpointName)
    {
        State        = state;
        EndpointName = endpointName;
    }

    public override bool? Suitable(UpdateContext context)
    {
        var state = context.GetUserCommandStateFromCache();
        return state?.State == State;
    }
}

public class UserCommandState : IUserCommandState
{
    public long   UserId       { get; set; }
    public string EndpointName { get; set; }
    public int    State        { get; set; }
}