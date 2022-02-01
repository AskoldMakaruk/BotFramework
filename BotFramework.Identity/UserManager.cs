using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BotFramework.Identity;

/// <summary>
/// Claims related extensions for <see cref="ClaimsPrincipal"/>.
/// </summary>
public static class PrincipalExtensions
{
    /// <summary>
    /// Returns the value for the first claim of the specified type, otherwise null if the claim is not present.
    /// </summary>
    /// <param name="principal">The <see cref="ClaimsPrincipal"/> instance this method extends.</param>
    /// <param name="claimType">The claim type whose first value should be returned.</param>
    /// <returns>The value of the first instance of the specified claim type, or null if the claim is not present.</returns>
    public static string FindFirstValue(this ClaimsPrincipal principal, string claimType)
    {
        if (principal == null)
        {
            throw new ArgumentNullException(nameof(principal));
        }

        var claim = principal.FindFirst(claimType);
        return claim?.Value;
    }
}

/// <summary>
/// Used for store specific options
/// </summary>
public class StoreOptions
{
    /// <summary>
    /// If set to a positive number, the default OnModelCreating will use this value as the max length for any 
    /// properties used as keys, i.e. UserId, LoginProvider, ProviderKey.
    /// </summary>
    public int MaxLengthForKeys { get; set; }

    /// <summary>
    /// If set to true, the store must protect all personally identifying data for a user. 
    /// This will be enforced by requiring the store to implement <see cref="IProtectedUserStore{TUser}"/>.
    /// </summary>
    public bool ProtectPersonalData { get; set; }
}

/// <summary>
/// Options for configuring sign in.
/// </summary>
public class SignInOptions
{
    /// <summary>
    /// Gets or sets a flag indicating whether a confirmed email address is required to sign in. Defaults to false.
    /// </summary>
    /// <value>True if a user must have a confirmed email address before they can sign in, otherwise false.</value>
    public bool RequireConfirmedEmail { get; set; }

    /// <summary>
    /// Gets or sets a flag indicating whether a confirmed telephone number is required to sign in. Defaults to false.
    /// </summary>
    /// <value>True if a user must have a confirmed telephone number before they can sign in, otherwise false.</value>
    public bool RequireConfirmedPhoneNumber { get; set; }

    /// <summary>
    /// Gets or sets a flag indicating whether a confirmed <see cref="IUserConfirmation{TUser}"/> account is required to sign in. Defaults to false.
    /// </summary>
    /// <value>True if a user must have a confirmed account before they can sign in, otherwise false.</value>
    public bool RequireConfirmedAccount { get; set; }
}

/// <summary>
/// Options for configuring user lockout.
/// </summary>
public class LockoutOptions
{
    /// <summary>
    /// Gets or sets a flag indicating whether a new user can be locked out. Defaults to true.
    /// </summary>
    /// <value>
    /// True if a newly created user can be locked out, otherwise false.  
    /// </value>
    public bool AllowedForNewUsers { get; set; } = true;

    /// <summary>
    /// Gets or sets the number of failed access attempts allowed before a user is locked out,
    /// assuming lock out is enabled. Defaults to 5.
    /// </summary>
    /// <value>
    /// The number of failed access attempts allowed before a user is locked out, if lockout is enabled.
    /// </value>
    public int MaxFailedAccessAttempts { get; set; } = 5;

    /// <summary>
    /// Gets or sets the <see cref="TimeSpan"/> a user is locked out for when a lockout occurs. Defaults to 5 minutes.
    /// </summary>
    /// <value>The <see cref="TimeSpan"/> a user is locked out for when a lockout occurs.</value>
    public TimeSpan DefaultLockoutTimeSpan { get; set; } = TimeSpan.FromMinutes(5);
}

/// <summary>
/// Specifies options for password requirements.
/// </summary>
public class PasswordOptions
{
    /// <summary>
    /// Gets or sets the minimum length a password must be. Defaults to 6.
    /// </summary>
    public int RequiredLength { get; set; } = 6;

    /// <summary>
    /// Gets or sets the minimum number of unique characters which a password must contain. Defaults to 1.
    /// </summary>
    public int RequiredUniqueChars { get; set; } = 1;

    /// <summary>
    /// Gets or sets a flag indicating if passwords must contain a non-alphanumeric character. Defaults to true.
    /// </summary>
    /// <value>True if passwords must contain a non-alphanumeric character, otherwise false.</value>
    public bool RequireNonAlphanumeric { get; set; } = true;

    /// <summary>
    /// Gets or sets a flag indicating if passwords must contain a lower case ASCII character. Defaults to true.
    /// </summary>
    /// <value>True if passwords must contain a lower case ASCII character.</value>
    public bool RequireLowercase { get; set; } = true;

    /// <summary>
    /// Gets or sets a flag indicating if passwords must contain a upper case ASCII character. Defaults to true.
    /// </summary>
    /// <value>True if passwords must contain a upper case ASCII character.</value>
    public bool RequireUppercase { get; set; } = true;

    /// <summary>
    /// Gets or sets a flag indicating if passwords must contain a digit. Defaults to true.
    /// </summary>
    /// <value>True if passwords must contain a digit.</value>
    public bool RequireDigit { get; set; } = true;
}

/// <summary>
/// Options for user validation.
/// </summary>
public class UserOptions
{
    /// <summary>
    /// Gets or sets the list of allowed characters in the username used to validate user names. Defaults to abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+
    /// </summary>
    /// <value>
    /// The list of allowed characters in the username used to validate user names.
    /// </value>
    public string AllowedUserNameCharacters { get; set; } =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

    /// <summary>
    /// Gets or sets a flag indicating whether the application requires unique emails for its users. Defaults to false.
    /// </summary>
    /// <value>
    /// True if the application requires each user to have their own, unique email, otherwise false.
    /// </value>
    public bool RequireUniqueEmail { get; set; }
}

/// <summary>
/// Options used to configure the claim types used for well known claims.
/// </summary>
public class ClaimsIdentityOptions
{
    /// <summary>
    /// Gets or sets the ClaimType used for a Role claim. Defaults to <see cref="ClaimTypes.Role"/>.
    /// </summary>
    public string RoleClaimType { get; set; } = ClaimTypes.Role;

    /// <summary>
    /// Gets or sets the ClaimType used for the user name claim. Defaults to <see cref="ClaimTypes.Name"/>.
    /// </summary>
    public string UserNameClaimType { get; set; } = ClaimTypes.Name;

    /// <summary>
    /// Gets or sets the ClaimType used for the user identifier claim. Defaults to <see cref="ClaimTypes.NameIdentifier"/>.
    /// </summary>
    public string UserIdClaimType { get; set; } = ClaimTypes.NameIdentifier;

    /// <summary>
    /// Gets or sets the ClaimType used for the user email claim. Defaults to <see cref="ClaimTypes.Email"/>.
    /// </summary>
    public string EmailClaimType { get; set; } = ClaimTypes.Email;

    /// <summary>
    /// Gets or sets the ClaimType used for the security stamp claim. Defaults to "AspNet.Identity.SecurityStamp".
    /// </summary>
    public string SecurityStampClaimType { get; set; } = "AspNet.Identity.SecurityStamp";
}

/// <summary>
/// Represents all the options you can use to configure the identity system.
/// </summary>
public class IdentityOptions
{
    /// <summary>
    /// Gets or sets the <see cref="ClaimsIdentityOptions"/> for the identity system.
    /// </summary>
    /// <value>
    /// The <see cref="ClaimsIdentityOptions"/> for the identity system.
    /// </value>
    public ClaimsIdentityOptions ClaimsIdentity { get; set; } = new ClaimsIdentityOptions();

    /// <summary>
    /// Gets or sets the <see cref="UserOptions"/> for the identity system.
    /// </summary>
    /// <value>
    /// The <see cref="UserOptions"/> for the identity system.
    /// </value>
    public UserOptions User { get; set; } = new UserOptions();

    /// <summary>
    /// Gets or sets the <see cref="PasswordOptions"/> for the identity system.
    /// </summary>
    /// <value>
    /// The <see cref="PasswordOptions"/> for the identity system.
    /// </value>
    public PasswordOptions Password { get; set; } = new PasswordOptions();

    /// <summary>
    /// Gets or sets the <see cref="LockoutOptions"/> for the identity system.
    /// </summary>
    /// <value>
    /// The <see cref="LockoutOptions"/> for the identity system.
    /// </value>
    public LockoutOptions Lockout { get; set; } = new LockoutOptions();

    /// <summary>
    /// Gets or sets the <see cref="SignInOptions"/> for the identity system.
    /// </summary>
    /// <value>
    /// The <see cref="SignInOptions"/> for the identity system.
    /// </value>
    public SignInOptions SignIn { get; set; } = new SignInOptions();

    /// <summary>
    /// Gets or sets the <see cref="StoreOptions"/> for the identity system.
    /// </summary>
    /// <value>
    /// The <see cref="StoreOptions"/> for the identity system.
    /// </value>
    public StoreOptions Stores { get; set; } = new StoreOptions();
}

/// <summary>
/// Provides the APIs for managing user in a persistence store.
/// </summary>
/// <typeparam name="TUser">The type encapsulating a user.</typeparam>
public class UserManager<TUser> : IDisposable where TUser : class, new()
{
    /// <summary>
    /// The data protection purpose used for the change phone number methods.
    /// </summary>
    private bool _disposed;

    /// <summary>
    /// The cancellation token used to cancel operations.
    /// </summary>
    protected virtual CancellationToken CancellationToken => CancellationToken.None;

    public UserManager() { }

    /// <summary>
    /// Constructs a new instance of <see cref="UserManager{TUser}"/>.
    /// </summary>
    /// <param name="store">The persistence store the manager will operate over.</param>
    /// <param name="optionsAccessor">The accessor used to access the <see cref="IdentityOptions"/>.</param>
    /// <param name="logger">The logger used to log messages, warnings and errors.</param>
    public UserManager(IUserStore<TUser>           store,
                       IOptions<IdentityOptions>   optionsAccessor,
                       ILogger<UserManager<TUser>> logger)
    {
        Store = store ?? throw new ArgumentNullException(nameof(store));

        Options = optionsAccessor?.Value ?? new IdentityOptions();
        Logger  = logger;
    }

    /// <summary>
    /// Gets or sets the persistence store the manager operates over.
    /// </summary>
    /// <value>The persistence store the manager operates over.</value>
    protected internal IUserStore<TUser> Store { get; set; }

    /// <summary>
    /// The <see cref="ILogger"/> used to log messages from the manager.
    /// </summary>
    /// <value>
    /// The <see cref="ILogger"/> used to log messages from the manager.
    /// </value>
    public virtual ILogger Logger { get; set; }

    /// <summary>
    /// The <see cref="IdentityOptions"/> used to configure Identity.
    /// </summary>
    public IdentityOptions Options { get; set; }


    /// <summary>
    /// Gets a flag indicating whether the backing user store supports user roles.
    /// </summary>
    /// <value>
    /// true if the backing user store supports user roles, otherwise false.
    /// </value>
    public virtual bool SupportsUserRole
    {
        get
        {
            ThrowIfDisposed();
            return Store is IUserRoleStore<TUser>;
        }
    }

    /// <summary>
    /// Gets a flag indicating whether the backing user store supports user telephone numbers.
    /// </summary>
    /// <value>
    /// true if the backing user store supports user telephone numbers, otherwise false.
    /// </value>
    public virtual bool SupportsUserPhoneNumber
    {
        get
        {
            ThrowIfDisposed();
            return Store is IUserPhoneNumberStore<TUser>;
        }
    }

    /// <summary>
    /// Gets a flag indicating whether the backing user store supports user claims.
    /// </summary>
    /// <value>
    /// true if the backing user store supports user claims, otherwise false.
    /// </value>
    public virtual bool SupportsUserClaim
    {
        get
        {
            ThrowIfDisposed();
            return Store is IUserClaimStore<TUser>;
        }
    }

    /// <summary>
    /// Gets a flag indicating whether the backing user store supports user lock-outs.
    /// </summary>
    /// <value>
    /// true if the backing user store supports user lock-outs, otherwise false.
    /// </value>
    public virtual bool SupportsUserLockout
    {
        get
        {
            ThrowIfDisposed();
            return Store is IUserLockoutStore<TUser>;
        }
    }

    /// <summary>
    /// Gets a flag indicating whether the backing user store supports returning
    /// <see cref="IQueryable"/> collections of information.
    /// </summary>
    /// <value>
    /// true if the backing user store supports returning <see cref="IQueryable"/> collections of
    /// information, otherwise false.
    /// </value>
    public virtual bool SupportsQueryableUsers
    {
        get
        {
            ThrowIfDisposed();
            return Store is IQueryableUserStore<TUser>;
        }
    }

    /// <summary>
    ///     Returns an IQueryable of users if the store is an IQueryableUserStore
    /// </summary>
    public virtual IQueryable<TUser> Users
    {
        get
        {
            if (!(Store is IQueryableUserStore<TUser> queryableStore))
            {
                throw new NotSupportedException("StoreNotIQueryableUserStore");
            }

            return queryableStore.Users;
        }
    }

    /// <summary>
    /// Releases all resources used by the user manager.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Returns the Name claim value if present otherwise returns null.
    /// </summary>
    /// <param name="principal">The <see cref="ClaimsPrincipal"/> instance.</param>
    /// <returns>The Name claim value, or null if the claim is not present.</returns>
    /// <remarks>The Name claim is identified by <see cref="ClaimsIdentity.DefaultNameClaimType"/>.</remarks>
    public virtual string GetUserName(ClaimsPrincipal principal)
    {
        if (principal == null)
        {
            throw new ArgumentNullException(nameof(principal));
        }

        return principal.FindFirstValue(Options.ClaimsIdentity.UserNameClaimType);
    }

    /// <summary>
    /// Returns the User ID claim value if present otherwise returns null.
    /// </summary>
    /// <param name="principal">The <see cref="ClaimsPrincipal"/> instance.</param>
    /// <returns>The User ID claim value, or null if the claim is not present.</returns>
    /// <remarks>The User ID claim is identified by <see cref="ClaimTypes.NameIdentifier"/>.</remarks>
    public virtual string GetUserId(ClaimsPrincipal principal)
    {
        if (principal == null)
        {
            throw new ArgumentNullException(nameof(principal));
        }

        return principal.FindFirstValue(Options.ClaimsIdentity.UserIdClaimType);
    }

    /// <summary>
    /// Returns the user corresponding to the IdentityOptions.ClaimsIdentity.UserIdClaimType claim in
    /// the id or null.
    /// </summary>
    /// <param name="principal">The id which contains the user id claim.</param>
    /// <returns>The user corresponding to the IdentityOptions.ClaimsIdentity.UserIdClaimType claim in
    /// the id or null</returns>
    public virtual Task<TUser> GetUserAsync(ClaimsPrincipal principal)
    {
        if (principal == null)
        {
            throw new ArgumentNullException(nameof(principal));
        }

        return long.TryParse(GetUserId(principal), out var id)
               ? FindByIdAsync(id)
               : Task.FromResult<TUser>(null);
    }

    /// <summary>
    /// Generates a value suitable for use in concurrency tracking.
    /// </summary>
    /// <param name="user">The user to generate the stamp for.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the security
    /// stamp for the specified <paramref name="user"/>.
    /// </returns>
    public virtual Task<string> GenerateConcurrencyStampAsync(TUser user)
    {
        return Task.FromResult(Guid.NewGuid().ToString());
    }

    /// <summary>
    /// Creates the specified <paramref name="user"/> in the backing store with no password,
    /// as an asynchronous operation.
    /// </summary>
    /// <param name="user">The user to create.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
    /// of the operation.
    /// </returns>
    public virtual async Task<IdentityResult> CreateAsync(TUser user)
    {
        ThrowIfDisposed();

        if (Options.Lockout.AllowedForNewUsers && SupportsUserLockout)
        {
            await GetUserLockoutStore().SetLockoutEnabledAsync(user, true, CancellationToken);
        }

        return await Store.CreateAsync(user, CancellationToken);
    }

    /// <summary>
    /// Updates the specified <paramref name="user"/> in the backing store.
    /// </summary>
    /// <param name="user">The user to update.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
    /// of the operation.
    /// </returns>
    public virtual Task<IdentityResult> UpdateAsync(TUser user)
    {
        ThrowIfDisposed();
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        return UpdateUserAsync(user);
    }

    /// <summary>
    /// Deletes the specified <paramref name="user"/> from the backing store.
    /// </summary>
    /// <param name="user">The user to delete.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
    /// of the operation.
    /// </returns>
    public virtual Task<IdentityResult> DeleteAsync(TUser user)
    {
        ThrowIfDisposed();
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        return Store.DeleteAsync(user, CancellationToken);
    }

    /// <summary>
    /// Finds and returns a user, if any, who has the specified <paramref name="userId"/>.
    /// </summary>
    /// <param name="userId">The user ID to search for.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the user matching the specified <paramref name="userId"/> if it exists.
    /// </returns>
    public virtual Task<TUser> FindByIdAsync(long userId)
    {
        ThrowIfDisposed();
        return Store.FindByIdAsync(userId, CancellationToken);
    }

    /// <summary>
    /// Finds and returns a user, if any, who has the specified user name.
    /// </summary>
    /// <param name="userName">The user name to search for.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the user matching the specified <paramref name="userName"/> if it exists.
    /// </returns>
    public virtual async Task<TUser> FindByNameAsync(string userName)
    {
        ThrowIfDisposed();
        if (userName == null)
        {
            throw new ArgumentNullException(nameof(userName));
        }

        var user = await Store.FindByNameAsync(userName, CancellationToken);
        return user;
    }

    /// <summary>
    /// Gets the user name for the specified <paramref name="user"/>.
    /// </summary>
    /// <param name="user">The user whose name should be retrieved.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the name for the specified <paramref name="user"/>.</returns>
    public virtual async Task<string> GetUserNameAsync(TUser user)
    {
        ThrowIfDisposed();
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        return await Store.GetUserNameAsync(user, CancellationToken);
    }

    /// <summary>
    /// Sets the given <paramref name="userName" /> for the specified <paramref name="user"/>.
    /// </summary>
    /// <param name="user">The user whose name should be set.</param>
    /// <param name="userName">The user name to set.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
    public virtual async Task<IdentityResult> SetUserNameAsync(TUser user, string userName)
    {
        ThrowIfDisposed();
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        await Store.SetUserNameAsync(user, userName, CancellationToken);
        return await UpdateUserAsync(user);
    }

    /// <summary>
    /// Gets the user identifier for the specified <paramref name="user"/>.
    /// </summary>
    /// <param name="user">The user whose identifier should be retrieved.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the identifier for the specified <paramref name="user"/>.</returns>
    public virtual async Task<long> GetUserIdAsync(TUser user)
    {
        ThrowIfDisposed();
        return await Store.GetUserIdAsync(user, CancellationToken);
    }

    /// <summary>
    /// Adds the specified <paramref name="claim"/> to the <paramref name="user"/>.
    /// </summary>
    /// <param name="user">The user to add the claim to.</param>
    /// <param name="claim">The claim to add.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
    /// of the operation.
    /// </returns>
    public virtual Task<IdentityResult> AddClaimAsync(TUser user, Claim claim)
    {
        ThrowIfDisposed();
        if (claim == null)
        {
            throw new ArgumentNullException(nameof(claim));
        }

        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        return AddClaimsAsync(user, new Claim[] { claim });
    }

    /// <summary>
    /// Adds the specified <paramref name="claims"/> to the <paramref name="user"/>.
    /// </summary>
    /// <param name="user">The user to add the claim to.</param>
    /// <param name="claims">The claims to add.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
    /// of the operation.
    /// </returns>
    public virtual async Task<IdentityResult> AddClaimsAsync(TUser user, IEnumerable<Claim> claims)
    {
        ThrowIfDisposed();
        var claimStore = GetClaimStore();
        if (claims == null)
        {
            throw new ArgumentNullException(nameof(claims));
        }

        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        await claimStore.AddClaimsAsync(user, claims, CancellationToken);
        return await UpdateUserAsync(user);
    }

    /// <summary>
    /// Replaces the given <paramref name="claim"/> on the specified <paramref name="user"/> with the <paramref name="newClaim"/>
    /// </summary>
    /// <param name="user">The user to replace the claim on.</param>
    /// <param name="claim">The claim to replace.</param>
    /// <param name="newClaim">The new claim to replace the existing <paramref name="claim"/> with.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
    /// of the operation.
    /// </returns>
    public virtual async Task<IdentityResult> ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim)
    {
        ThrowIfDisposed();
        var claimStore = GetClaimStore();
        if (claim == null)
        {
            throw new ArgumentNullException(nameof(claim));
        }

        if (newClaim == null)
        {
            throw new ArgumentNullException(nameof(newClaim));
        }

        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        await claimStore.ReplaceClaimAsync(user, claim, newClaim, CancellationToken);
        return await UpdateUserAsync(user);
    }

    /// <summary>
    /// Removes the specified <paramref name="claim"/> from the given <paramref name="user"/>.
    /// </summary>
    /// <param name="user">The user to remove the specified <paramref name="claim"/> from.</param>
    /// <param name="claim">The <see cref="Claim"/> to remove.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
    /// of the operation.
    /// </returns>
    public virtual Task<IdentityResult> RemoveClaimAsync(TUser user, Claim claim)
    {
        ThrowIfDisposed();
        var claimStore = GetClaimStore();
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        if (claim == null)
        {
            throw new ArgumentNullException(nameof(claim));
        }

        return RemoveClaimsAsync(user, new Claim[] { claim });
    }

    /// <summary>
    /// Removes the specified <paramref name="claims"/> from the given <paramref name="user"/>.
    /// </summary>
    /// <param name="user">The user to remove the specified <paramref name="claims"/> from.</param>
    /// <param name="claims">A collection of <see cref="Claim"/>s to remove.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
    /// of the operation.
    /// </returns>
    public virtual async Task<IdentityResult> RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims)
    {
        ThrowIfDisposed();
        var claimStore = GetClaimStore();
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        if (claims == null)
        {
            throw new ArgumentNullException(nameof(claims));
        }

        await claimStore.RemoveClaimsAsync(user, claims, CancellationToken);
        return await UpdateUserAsync(user);
    }

    /// <summary>
    /// Gets a list of <see cref="Claim"/>s to be belonging to the specified <paramref name="user"/> as an asynchronous operation.
    /// </summary>
    /// <param name="user">The user whose claims to retrieve.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> that represents the result of the asynchronous query, a list of <see cref="Claim"/>s.
    /// </returns>
    public virtual async Task<IList<Claim>> GetClaimsAsync(TUser user)
    {
        ThrowIfDisposed();
        var claimStore = GetClaimStore();
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        return await claimStore.GetClaimsAsync(user, CancellationToken);
    }

    /// <summary>
    /// Add the specified <paramref name="user"/> to the named role.
    /// </summary>
    /// <param name="user">The user to add to the named role.</param>
    /// <param name="role">The name of the role to add the user to.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
    /// of the operation.
    /// </returns>
    public virtual async Task<IdentityResult> AddToRoleAsync(TUser user, string role)
    {
        ThrowIfDisposed();
        var userRoleStore = GetUserRoleStore();
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        if (await userRoleStore.IsInRoleAsync(user, role, CancellationToken))
        {
            return UserAlreadyInRoleError(role);
        }

        await userRoleStore.AddToRoleAsync(user, role, CancellationToken);
        return await UpdateUserAsync(user);
    }

    /// <summary>
    /// Add the specified <paramref name="user"/> to the named roles.
    /// </summary>
    /// <param name="user">The user to add to the named roles.</param>
    /// <param name="roles">The name of the roles to add the user to.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
    /// of the operation.
    /// </returns>
    public virtual async Task<IdentityResult> AddToRolesAsync(TUser user, IEnumerable<string> roles)
    {
        ThrowIfDisposed();
        var userRoleStore = GetUserRoleStore();
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        if (roles == null)
        {
            throw new ArgumentNullException(nameof(roles));
        }

        foreach (var role in roles.Distinct())
        {
            if (await userRoleStore.IsInRoleAsync(user, role, CancellationToken))
            {
                return UserAlreadyInRoleError(role);
            }

            await userRoleStore.AddToRoleAsync(user, role, CancellationToken);
        }

        return await UpdateUserAsync(user);
    }

    /// <summary>
    /// Removes the specified <paramref name="user"/> from the named role.
    /// </summary>
    /// <param name="user">The user to remove from the named role.</param>
    /// <param name="role">The name of the role to remove the user from.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
    /// of the operation.
    /// </returns>
    public virtual async Task<IdentityResult> RemoveFromRoleAsync(TUser user, string role)
    {
        ThrowIfDisposed();
        var userRoleStore = GetUserRoleStore();
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        if (!await userRoleStore.IsInRoleAsync(user, role, CancellationToken))
        {
            return UserNotInRoleError(role);
        }

        await userRoleStore.RemoveFromRoleAsync(user, role, CancellationToken);
        return await UpdateUserAsync(user);
    }

    private IdentityResult UserAlreadyInRoleError(string role)
    {
        Logger.LogWarning(5, "User is already in role {role}.", role);
        return IdentityResult.Failed(new IdentityError { Description = $"UserAlreadyInRole {role}" });
    }

    private IdentityResult UserNotInRoleError(string role)
    {
        Logger.LogWarning(6, "User is not in role {role}.", role);
        return IdentityResult.Failed(new IdentityError { Description = $"UserNotInRole {role}" });
    }

    /// <summary>
    /// Removes the specified <paramref name="user"/> from the named roles.
    /// </summary>
    /// <param name="user">The user to remove from the named roles.</param>
    /// <param name="roles">The name of the roles to remove the user from.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
    /// of the operation.
    /// </returns>
    public virtual async Task<IdentityResult> RemoveFromRolesAsync(TUser user, IEnumerable<string> roles)
    {
        ThrowIfDisposed();
        var userRoleStore = GetUserRoleStore();
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        if (roles == null)
        {
            throw new ArgumentNullException(nameof(roles));
        }

        foreach (var role in roles)
        {
            if (!await userRoleStore.IsInRoleAsync(user, role, CancellationToken))
            {
                return UserNotInRoleError(role);
            }

            await userRoleStore.RemoveFromRoleAsync(user, role, CancellationToken);
        }

        return await UpdateUserAsync(user);
    }

    /// <summary>
    /// Gets a list of role names the specified <paramref name="user"/> belongs to.
    /// </summary>
    /// <param name="user">The user whose role names to retrieve.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing a list of role names.</returns>
    public virtual async Task<IList<string>> GetRolesAsync(TUser user)
    {
        ThrowIfDisposed();
        var userRoleStore = GetUserRoleStore();
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        return await userRoleStore.GetRolesAsync(user, CancellationToken);
    }

    /// <summary>
    /// Returns a flag indicating whether the specified <paramref name="user"/> is a member of the given named role.
    /// </summary>
    /// <param name="user">The user whose role membership should be checked.</param>
    /// <param name="role">The name of the role to be checked.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing a flag indicating whether the specified <paramref name="user"/> is
    /// a member of the named role.
    /// </returns>
    public virtual async Task<bool> IsInRoleAsync(TUser user, string role)
    {
        ThrowIfDisposed();
        var userRoleStore = GetUserRoleStore();
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        return await userRoleStore.IsInRoleAsync(user, role, CancellationToken);
    }

    /// <summary>
    /// Gets the telephone number, if any, for the specified <paramref name="user"/>.
    /// </summary>
    /// <param name="user">The user whose telephone number should be retrieved.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the user's telephone number, if any.</returns>
    public virtual async Task<string> GetPhoneNumberAsync(TUser user)
    {
        ThrowIfDisposed();
        var store = GetPhoneNumberStore();
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        return await store.GetPhoneNumberAsync(user, CancellationToken);
    }

    /// <summary>
    /// Sets the phone number for the specified <paramref name="user"/>.
    /// </summary>
    /// <param name="user">The user whose phone number to set.</param>
    /// <param name="phoneNumber">The phone number to set.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
    /// of the operation.
    /// </returns>
    public virtual async Task<IdentityResult> SetPhoneNumberAsync(TUser user, string phoneNumber)
    {
        ThrowIfDisposed();
        var store = GetPhoneNumberStore();
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        await store.SetPhoneNumberAsync(user, phoneNumber, CancellationToken);
        return await UpdateUserAsync(user);
    }

    /// <summary>
    /// Returns a flag indicating whether the specified <paramref name="user"/> is locked out,
    /// as an asynchronous operation.
    /// </summary>
    /// <param name="user">The user whose locked out status should be retrieved.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, true if the specified <paramref name="user "/>
    /// is locked out, otherwise false.
    /// </returns>
    public virtual async Task<bool> IsLockedOutAsync(TUser user)
    {
        ThrowIfDisposed();
        var store = GetUserLockoutStore();
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        if (!await store.GetLockoutEnabledAsync(user, CancellationToken))
        {
            return false;
        }

        var lockoutTime = await store.GetLockoutEndDateAsync(user, CancellationToken);
        return lockoutTime >= DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Sets a flag indicating whether the specified <paramref name="user"/> is locked out,
    /// as an asynchronous operation.
    /// </summary>
    /// <param name="user">The user whose locked out status should be set.</param>
    /// <param name="enabled">Flag indicating whether the user is locked out or not.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, the <see cref="IdentityResult"/> of the operation
    /// </returns>
    public virtual async Task<IdentityResult> SetLockoutEnabledAsync(TUser user, bool enabled)
    {
        ThrowIfDisposed();
        var store = GetUserLockoutStore();
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        await store.SetLockoutEnabledAsync(user, enabled, CancellationToken);
        return await UpdateUserAsync(user);
    }

    /// <summary>
    /// Retrieves a flag indicating whether user lockout can be enabled for the specified user.
    /// </summary>
    /// <param name="user">The user whose ability to be locked out should be returned.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, true if a user can be locked out, otherwise false.
    /// </returns>
    public virtual async Task<bool> GetLockoutEnabledAsync(TUser user)
    {
        ThrowIfDisposed();
        var store = GetUserLockoutStore();
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        return await store.GetLockoutEnabledAsync(user, CancellationToken);
    }

    /// <summary>
    /// Gets the last <see cref="DateTimeOffset"/> a user's last lockout expired, if any.
    /// A time value in the past indicates a user is not currently locked out.
    /// </summary>
    /// <param name="user">The user whose lockout date should be retrieved.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> that represents the lookup, a <see cref="DateTimeOffset"/> containing the last time a user's lockout expired, if any.
    /// </returns>
    public virtual async Task<DateTimeOffset?> GetLockoutEndDateAsync(TUser user)
    {
        ThrowIfDisposed();
        var store = GetUserLockoutStore();
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        return await store.GetLockoutEndDateAsync(user, CancellationToken);
    }

    /// <summary>
    /// Locks out a user until the specified end date has passed. Setting a end date in the past immediately unlocks a user.
    /// </summary>
    /// <param name="user">The user whose lockout date should be set.</param>
    /// <param name="lockoutEnd">The <see cref="DateTimeOffset"/> after which the <paramref name="user"/>'s lockout should end.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the operation.</returns>
    public virtual async Task<IdentityResult> SetLockoutEndDateAsync(TUser user, DateTimeOffset? lockoutEnd)
    {
        ThrowIfDisposed();
        var store = GetUserLockoutStore();
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        if (!await store.GetLockoutEnabledAsync(user, CancellationToken))
        {
            Logger.LogWarning(11, "Lockout for user failed because lockout is not enabled for this user.");
            return IdentityResult.Failed(new IdentityError { Description = "UserLockoutNotEnabled" });
        }

        await store.SetLockoutEndDateAsync(user, lockoutEnd, CancellationToken);
        return await UpdateUserAsync(user);
    }

    /// <summary>
    /// Returns a list of users from the user store who have the specified <paramref name="claim"/>.
    /// </summary>
    /// <param name="claim">The claim to look for.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> that represents the result of the asynchronous query, a list of <typeparamref name="TUser"/>s who
    /// have the specified claim.
    /// </returns>
    public virtual Task<IList<TUser>> GetUsersForClaimAsync(Claim claim)
    {
        ThrowIfDisposed();
        var store = GetClaimStore();
        if (claim == null)
        {
            throw new ArgumentNullException(nameof(claim));
        }

        return store.GetUsersForClaimAsync(claim, CancellationToken);
    }

    /// <summary>
    /// Returns a list of users from the user store who are members of the specified <paramref name="roleName"/>.
    /// </summary>
    /// <param name="roleName">The name of the role whose users should be returned.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> that represents the result of the asynchronous query, a list of <typeparamref name="TUser"/>s who
    /// are members of the specified role.
    /// </returns>
    public virtual Task<IList<TUser>> GetUsersInRoleAsync(string roleName)
    {
        ThrowIfDisposed();
        var store = GetUserRoleStore();
        if (roleName == null)
        {
            throw new ArgumentNullException(nameof(roleName));
        }

        return store.GetUsersInRoleAsync(roleName, CancellationToken);
    }

    /// <summary>
    /// Releases the unmanaged resources used by the role manager and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing && !_disposed)
        {
            Store.Dispose();
            _disposed = true;
        }
    }

    private IUserRoleStore<TUser> GetUserRoleStore()
    {
        var cast = Store as IUserRoleStore<TUser>;
        if (cast == null)
        {
            throw new NotSupportedException("StoreNotIUserRoleStore");
        }

        return cast;
    }

    private IUserClaimStore<TUser> GetClaimStore()
    {
        var cast = Store as IUserClaimStore<TUser>;
        if (cast == null)
        {
            throw new NotSupportedException("StoreNotIUserClaimStore");
        }

        return cast;
    }


    private IUserLockoutStore<TUser> GetUserLockoutStore()
    {
        var cast = Store as IUserLockoutStore<TUser>;
        if (cast == null)
        {
            throw new NotSupportedException("StoreNotIUserLockoutStore");
        }

        return cast;
    }

    private IUserPhoneNumberStore<TUser> GetPhoneNumberStore()
    {
        var cast = Store as IUserPhoneNumberStore<TUser>;
        if (cast == null)
        {
            throw new NotSupportedException("StoreNotIUserPhoneNumberStore");
        }

        return cast;
    }


    /// <summary>
    /// Called to update the user after validating and updating the normalized email/user name.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <returns>Whether the operation was successful.</returns>
    protected virtual async Task<IdentityResult> UpdateUserAsync(TUser user)
    {
        return await Store.UpdateAsync(user, CancellationToken);
    }

    /// <summary>
    /// Throws if this class has been disposed.
    /// </summary>
    protected void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(GetType().Name);
        }
    }
}