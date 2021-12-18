using System.Threading.Tasks;

namespace BotFramework.Identity;

/// <summary>
/// Default implementation of <see cref="IUserConfirmation{TUser}"/>.
/// </summary>
/// <typeparam name="TUser">The type encapsulating a user.</typeparam>
public class DefaultUserConfirmation<TUser> : IUserConfirmation<TUser> where TUser : IdentityUser, new()
{
    /// <summary>
    /// Determines whether the specified <paramref name="user"/> is confirmed.
    /// </summary>
    /// <param name="manager">The <see cref="UserManager{TUser}"/> that can be used to retrieve user properties.</param>
    /// <param name="user">The user.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the confirmation operation.</returns>
    public virtual Task<bool> IsConfirmedAsync(UserManager<TUser> manager, TUser user)
    {
        return Task.FromResult(true);
    }
}