using System.Threading;
using System.Threading.Tasks;

namespace BotFramework.Identity
{
    /// <summary>
    /// Provides an abstraction for a store containing users' telephone numbers.
    /// </summary>
    /// <typeparam name="TUser">The type encapsulating a user.</typeparam>
    public interface IUserPhoneNumberStore<TUser> : IUserStore<TUser> where TUser : class
    {
        /// <summary>
        /// Sets the telephone number for the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user whose telephone number should be set.</param>
        /// <param name="phoneNumber">The telephone number to set.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task SetPhoneNumberAsync(TUser user, string phoneNumber, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the telephone number, if any, for the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user whose telephone number should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the user's telephone number, if any.</returns>
        Task<string> GetPhoneNumberAsync(TUser user, CancellationToken cancellationToken);
    }
}