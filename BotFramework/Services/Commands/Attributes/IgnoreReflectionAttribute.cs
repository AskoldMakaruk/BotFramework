using System;

namespace BotFramework.Services.Commands.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class IgnoreReflectionAttribute : Attribute { }