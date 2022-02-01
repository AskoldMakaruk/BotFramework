using System;

namespace BotFramework.Identity;

/// <summary>
/// Represents a role in the identity system
/// </summary>
public class IdentityRole 
{
    /// <summary>
    /// Initializes a new instance of <see cref="IdentityRole"/>.
    /// </summary>
    public IdentityRole() { }

    /// <summary>
    /// Initializes a new instance of <see cref="IdentityRole"/>.
    /// </summary>
    /// <param name="roleName">The role name.</param>
    public IdentityRole(string roleName) : this()
    {
        Name = roleName;
    }

    /// <summary>
    /// Gets or sets the primary key for this role.
    /// </summary>
    public virtual long Id { get; set; }

    /// <summary>
    /// Gets or sets the name for this role.
    /// </summary>
    public virtual string Name { get; set; }

    /// <summary>
    /// Gets or sets the normalized name for this role.
    /// </summary>
    public virtual string NormalizedName { get; set; }

    /// <summary>
    /// A random value that should change whenever a role is persisted to the store
    /// </summary>
    public virtual string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Returns the name of the role.
    /// </summary>
    /// <returns>The name of the role.</returns>
    public override string ToString()
    {
        return Name;
    }
}