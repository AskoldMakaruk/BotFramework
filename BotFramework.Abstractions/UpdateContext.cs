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

    public IList<CommandAttribute> Attributes { get; set; }
}

public delegate bool? CommandPredicate(UpdateContext update);

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public abstract class CommandAttribute : Attribute
{
    public virtual EndpointPriority? EndpointPriority                => null;
    public virtual bool?             Suitable(UpdateContext context) => null;
}

public interface IPersistentCommand : ICommand
{
    public string Name  { get; }
    public int    State { get; }
}

public abstract class PersistentCommandBase : IPersistentCommand
{
    public Task Execute(UpdateContext context)
    {
        Sinc ??= context.RequestServices.GetService<IRequestSinc>()!;
        return Execute(context.Update, context.GetUserCommandState()!.State);
    }

    public abstract Task Execute(Update update, int state);

    public abstract bool? Suitable(UpdateContext context);


    public virtual  string Name  => ToString() ?? GetType().Name;
    public abstract int    State { get; }

    public IRequestSinc? Sinc { get; private set; }
}

public interface IUserCommandState
{
    long   UserId       { get; }
    string EndpointName { get; }
    int    State        { get; }
}

public static class UserCommandStateFeatureExtensions
{
    public static IUserCommandState? GetUserCommandState(this UpdateContext context)
    {
        var id = context.Update.GetId();
        if (id == null)
        {
            return null;
        }

        var storage = context.RequestServices.GetService<IUserScopeStorage>()?.Get(id.Value);
        var state   = storage?.Get<UserCommandStateFeature>()?.State;
        return state;
    }
}

public record UserCommandStateFeature(IUserCommandState State);

public interface IUserScopeStorage
{
    IFeatureCollection Get(long usedId);
}

public interface IPersistentCommandStorage
{
    public Task<IUserCommandState> GetUserCommandState(long userId);
}

/// <summary>
/// Represents a collection of HTTP features.
/// </summary>
public interface IFeatureCollection : IEnumerable<KeyValuePair<Type, object>>
{
    /// <summary>
    /// Indicates if the collection can be modified.
    /// </summary>
    bool IsReadOnly { get; }

    /// <summary>
    /// Incremented for each modification and can be used to verify cached results.
    /// </summary>
    int Revision { get; }

    /// <summary>
    /// Gets or sets a given feature. Setting a null value removes the feature.
    /// </summary>
    /// <param name="key"></param>
    /// <returns>The requested feature, or null if it is not present.</returns>
    object? this[Type key] { get; set; }

    /// <summary>
    /// Retrieves the requested feature from the collection.
    /// </summary>
    /// <typeparam name="TFeature">The feature key.</typeparam>
    /// <returns>The requested feature, or null if it is not present.</returns>
    TFeature? Get<TFeature>();

    /// <summary>
    /// Sets the given feature in the collection.
    /// </summary>
    /// <typeparam name="TFeature">The feature key.</typeparam>
    /// <param name="instance">The feature value.</param>
    void Set<TFeature>(TFeature instance);
}