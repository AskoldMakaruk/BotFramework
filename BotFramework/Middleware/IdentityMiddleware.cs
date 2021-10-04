using System.Collections.Generic;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using BotFramework.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;

#nullable enable
namespace BotFramework.Middleware
{
    // public class Claim
    // {
    //     public long   UserId { get; set; }
    //     public string Value  { get; set; }
    // }
    //
    // public interface IUser
    // {
    //     public List<Claim> Claims { get; }
    // }

    public interface IUserRepository<TUser>
    {
        Task<TUser?> GetUser(long    userId);
        Task<TUser>  CreateUser(User user);
    }

    public class IdentityMiddleware<TUser>
    {
        private readonly UpdateDelegate _next;

        public IdentityMiddleware(UpdateDelegate next)
        {
            _next = next;
        }

        internal async Task Invoke(Update update, UserContext<TUser> userContext, IUserRepository<TUser> repository)
        {
            if (update.GetUser() is not { } user)
            {
                await _next.Invoke(update);
                return;
            }

            if (userContext.User != null)
            {
                var dbUser = await repository.GetUser(user.Id) ?? await repository.CreateUser(user);
                userContext.User = dbUser;
            }

            await _next.Invoke(update);
        }
    }

    public static class UseIdentityMiddleware
    {
        public static void UseIdentity<TUser>(this IAppBuilder builder) where TUser : class
        {
            builder.Services.AddScoped<UserContext<TUser>>();
            builder.Services.AddScoped(provider => provider.GetService<UserContext<TUser>>()?.User!);
            builder.UseMiddleware<IdentityMiddleware<TUser>>();
        }
    }

    internal class UserContext<TUser>
    {
        public TUser User { get; internal set; }
    }
}