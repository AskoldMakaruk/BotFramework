using System;
using BotFramework.Abstractions;

namespace BotFramework.Services.Controllers.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public abstract class CommandAttribute : Attribute
{
    public virtual EndpointPriority? EndpointPriority                => null;
    public virtual bool?             Suitable(UpdateContext context) => null;
}