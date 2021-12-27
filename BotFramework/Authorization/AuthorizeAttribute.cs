using System;

namespace BotFramework.Authorization;

public class AuthorizeAttribute : Attribute
{
    public string[] Claims { get; }

    public AuthorizeAttribute(params string[] claims)
    {
        Claims = claims;
    }
}