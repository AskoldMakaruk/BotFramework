using System;
using BotFramework.Abstractions;
using Telegram.Bot.Types;

namespace BotFramework.Services.Controllers.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public abstract class CommandAttribute : Attribute
{
    public virtual EndpointPriority? EndpointPriority         => null;
    public virtual bool?             Suitable(Update context) => null;
}