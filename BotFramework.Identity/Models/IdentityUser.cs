using System;
using Telegram.Bot.Types;

namespace BotFramework.Identity;

/// <summary>
/// Represents a user in the identity system
/// </summary>
public class IdentityUser
{
    /// <summary>
    /// Initializes a new instance of <see cref="IdentityUser"/>.
    /// </summary>
    public IdentityUser() { }

    /// <summary>
    /// Initializes a new instance of <see cref="IdentityUser"/>.
    /// </summary>
    /// <param name="user">The user name.</param>
    public IdentityUser(User user) : this()
    {
        UserName = user.Username;
            
    }

    /// <summary>
    /// Gets or sets the primary key for this user.
    /// </summary>
    public virtual long Id { get; set; }

    /// <summary>
    /// Gets or sets the user name for this user.
    /// </summary>
    public virtual string UserName { get; set; }

    /// <summary>
    /// A random value that must change whenever a user is persisted to the store
    /// </summary>
    public virtual string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Gets or sets a telephone number for the user.
    /// </summary>
    public virtual string PhoneNumber { get; set; }

    /// <summary>
    /// Gets or sets the date and time, in UTC, when any user lockout ends.
    /// </summary>
    /// <remarks>
    /// A value in the past means the user is not locked out.
    /// </remarks>
    public virtual DateTimeOffset? LockoutEnd { get; set; }

    /// <summary>
    /// Gets or sets a flag indicating if the user could be locked out.
    /// </summary>
    /// <value>True if the user could be locked out, otherwise false.</value>
    public virtual bool LockoutEnabled { get; set; }

    /// <summary>
    /// Returns the username for this user.
    /// </summary>
    public override string ToString()
        => UserName;
}