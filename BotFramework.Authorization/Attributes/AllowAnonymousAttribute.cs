namespace BotFramework.Authorization;

/// <summary>
/// Specifies that the class or method that this attribute is applied to does not require authorization.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AllowAnonymousAttribute : Attribute, IAllowAnonymous { }