using System.Threading.Tasks;
using BotFramework.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace BotFramework.Identity.EntityFramework;

public class IdentityUserStateContext<TUser, TUserClaim> : IdentityUserContext<TUser, TUserClaim>, IPersistentCommandStorage
where TUser : IdentityUser, IUserCommandState, new()
where TUserClaim : IdentityUserClaim
{
    public async Task<IUserCommandState> GetUserCommandState(long userId)
    {
        return await Users.FirstOrDefaultAsync(a => a.Id == userId);
    }

    public async Task SetUserCommandState(long userId, IUserCommandState state)
    {
        var user = new TUser
        {
            Id = userId
        };
        Users.Attach(user);


        user.State        = state.State;
        user.EndpointName = state.EndpointName;

        await SaveChangesAsync();
    }
}

/// <summary>
/// Base class for the Entity Framework database context used for identity.
/// </summary>
/// <typeparam name="TUser">The type of the user objects.</typeparam>
public class IdentityUserContext<TUser> : IdentityUserContext<TUser, IdentityUserClaim>
where TUser : IdentityUser
{
    /// <summary>
    /// Initializes a new instance of <see cref="IdentityUserContext{TUser}"/>.
    /// </summary>
    /// <param name="options">The options to be used by a <see cref="DbContext"/>.</param>
    public IdentityUserContext(DbContextOptions options) : base(options) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="IdentityUserContext{TUser}" /> class.
    /// </summary>
    protected IdentityUserContext() { }
}

/// <summary>
/// Base class for the Entity Framework database context used for identity.
/// </summary>
/// <typeparam name="TUser">The type of user objects.</typeparam>
/// <typeparam name="TUserClaim">The type of the user claim object.</typeparam>
public abstract class IdentityUserContext<TUser, TUserClaim> : DbContext
where TUser : IdentityUser
where TUserClaim : IdentityUserClaim
{
    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="options">The options to be used by a <see cref="DbContext"/>.</param>
    public IdentityUserContext(DbContextOptions options) : base(options) { }

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    protected IdentityUserContext() { }

    /// <summary>
    /// Gets or sets the <see cref="DbSet{TEntity}"/> of Users.
    /// </summary>
    public virtual DbSet<TUser> Users { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="DbSet{TEntity}"/> of User claims.
    /// </summary>
    public virtual DbSet<TUserClaim> UserClaims { get; set; }


    /// <summary>
    /// Configures the schema needed for the identity framework.
    /// </summary>
    /// <param name="builder">
    /// The builder being used to construct the model for this context.
    /// </param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<TUser>(b =>
        {
            b.HasKey(u => u.Id);
            b.ToTable("BF_Users");
            b.Property(u => u.ConcurrencyStamp).IsConcurrencyToken();

            b.Property(u => u.UserName).HasMaxLength(256);

            b.HasMany<TUserClaim>().WithOne().HasForeignKey(uc => uc.UserId).IsRequired();
        });

        builder.Entity<TUserClaim>(b =>
        {
            b.HasKey(uc => uc.Id);
            b.ToTable("BF_UserClaims");
        });
    }
}