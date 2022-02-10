using System;
using System.Threading.Tasks;
using BotFramework.Abstractions;

namespace BotFramework.Middleware;

public class UserCommandStateMiddleware
{
    private UpdateDelegate _next;

    public UserCommandStateMiddleware(IServiceProvider services, UpdateDelegate next)
    {
        _next = next;
    }


    public Task Invoke(UpdateContext context)
    {
        return _next.Invoke(context);
    }
}