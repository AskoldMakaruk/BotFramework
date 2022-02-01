using System;
using System.Collections.Generic;
using System.Security.Claims;
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
    public CommandPredicate CommandPredicate { get; set; }
    public UpdateDelegate   Delegate         { get; set; }
}

public delegate bool? CommandPredicate(UpdateContext update);