using System.Threading.Tasks;
using BotFramework.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;

#nullable enable
namespace BotFramework.Identity;

public class IdentityMiddleware<TUser> where TUser : IdentityUser, new()
{
    private readonly UpdateDelegate _next;

    public IdentityMiddleware(UpdateDelegate next)
    {
        _next = next;
    }

    internal async Task Invoke(UpdateContext update, UserContext<TUser> userContext, UserManager<TUser> manager)
    {
        if (update.Update.GetUser() is not { } user)
        {
            await _next.Invoke(update);
            return;
        }

        if (userContext.User == null)
        {
            var dbUser = update.RequestServices.GetService<IUserScopeStorage>()?.Get(user.Id).Get<TUser>();
            if (dbUser is not null)
            {
                userContext.User = dbUser;
                return;
            }

            dbUser = await manager.FindByIdAsync(user.Id);
            if (dbUser is null)
            {
                await manager.CreateAsync(new TUser { Id = user.Id, UserName = user.Username });
                dbUser = await manager.FindByIdAsync(user.Id);
            }


            userContext.User = dbUser;
        }

        await _next.Invoke(update);
    }
}

internal class UserContext<TUser> where TUser : IdentityUser
{
    public TUser? User { get; internal set; }
}