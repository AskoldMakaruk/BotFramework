using System;

namespace BotFramework.Services.Controllers;

[AttributeUsage(AttributeTargets.Class)]
public class IgnoreReflectionAttribute : Attribute { }