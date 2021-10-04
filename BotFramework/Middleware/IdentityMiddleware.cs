using System.Threading.Tasks;
using BotFramework.Abstractions;
using BotFramework.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;

namespace BotFramework.Middleware
{
    public interface IUserFactory<TUser>
    {
        Task<TUser> GetUser(long userId);
    }

    public class IdentityMiddleware<TUser>
    {
        private readonly UpdateDelegate _next;

        public IdentityMiddleware(UpdateDelegate next)
        {
            _next = next;
        }

        internal async Task Invoke(Update update, AccountContext<TUser> accountContext, IUserFactory<TUser> factory)
        {
            if (update.GetUser() is not { } user)
            {
                await _next.Invoke(update);
                return;
            }

            if (accountContext.User != null)
            {
                accountContext.User = await factory.GetUser(user.Id);
            }

            await _next.Invoke(update);
        }
    }

    public static class UseIdentityMiddleware
    {
        public static void UseIdentity<TUser>(this IAppBuilder builder) where TUser : class
        {
            builder.Services.AddScoped<AccountContext<TUser>>();
            builder.Services.AddScoped(provider => provider.GetService<AccountContext<TUser>>()?.User!);
            builder.UseMiddleware<IdentityMiddleware<TUser>>();
        }
    }

    internal class AccountContext<TUser>
    {
        public TUser User { get; internal set; }
    }
}