using System.Threading.Tasks;

namespace BotFramework.Identity;

/// <summary>
/// Provides an abstraction for confirmation of user accounts.
/// </summary>
/// <typeparam name="TUser">The type encapsulating a user.</typeparam>
public interface IUserConfirmation<TUser> where TUser : IdentityUser, new()
{
    /// <summary>
    /// Determines whether the specified <paramref name="user"/> is confirmed.
    /// </summary>
    /// <param name="manager">The <see cref="UserManager{TUser}"/> that can be used to retrieve user properties.</param>
    /// <param name="user">The user.</param>
    /// <returns>Whether the user is confirmed.</returns>
    Task<bool> IsConfirmedAsync(UserManager<TUser> manager, TUser user);
}