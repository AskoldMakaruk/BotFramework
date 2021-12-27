using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;

namespace BotFramework.Abstractions;

public class UpdateContext
{
    public IServiceProvider RequestServices { get; set; }
    public ClaimsPrincipal  User            { get; set; }

    public readonly Update           Update;
    public readonly List<IEndpoint?> Endpoints = new();
    /// <summary>
    /// Gets or sets a key/value collection that can be used to share data within the scope of this request.
    /// </summary>
    public  IDictionary<object, object?> Items { get; set; }


    public UpdateContext(Update update)
    {
        Update = update;
    }
}