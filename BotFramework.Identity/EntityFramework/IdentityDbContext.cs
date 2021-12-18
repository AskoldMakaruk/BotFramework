using System;
using Microsoft.EntityFrameworkCore;

namespace BotFramework.Identity.EntityFramework;

/// <summary>
/// Base class for the Entity Framework database context used for identity.
/// </summary>
public class IdentityDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
{
    /// <summary>
    /// Initializes a new instance of <see cref="IdentityDbContext"/>.
    /// </summary>
    /// <param name="options">The options to be used by a <see cref="DbContext"/>.</param>
    public IdentityDbContext(DbContextOptions options) : base(options) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="IdentityDbContext" /> class.
    /// </summary>
    protected IdentityDbContext() { }
}

/// <summary>
/// Base class for the Entity Framework database context used for identity.
/// </summary>
/// <typeparam name="TUser">The type of the user objects.</typeparam>
public class IdentityDbContext<TUser> : IdentityDbContext<TUser, IdentityRole, string> where TUser : IdentityUser
{
    /// <summary>
    /// Initializes a new instance of <see cref="IdentityDbContext"/>.
    /// </summary>
    /// <param name="options">The options to be used by a <see cref="DbContext"/>.</param>
    public IdentityDbContext(DbContextOptions options) : base(options) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="IdentityDbContext" /> class.
    /// </summary>
    protected IdentityDbContext() { }
}

/// <summary>
/// Base class for the Entity Framework database context used for identity.
/// </summary>
/// <typeparam name="TUser">The type of user objects.</typeparam>
/// <typeparam name="TRole">The type of role objects.</typeparam>
/// <typeparam name="TKey">The type of the primary key for users and roles.</typeparam>
public class IdentityDbContext<TUser, TRole, TKey> : IdentityDbContext<TUser, TRole, IdentityUserClaim,
    IdentityUserRole, IdentityRoleClaim>
where TUser : IdentityUser
where TRole : IdentityRole
where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Initializes a new instance of the db context.
    /// </summary>
    /// <param name="options">The options to be used by a <see cref="DbContext"/>.</param>
    public IdentityDbContext(DbContextOptions options) : base(options) { }

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    protected IdentityDbContext() { }
}

/// <summary>
/// Base class for the Entity Framework database context used for identity.
/// </summary>
/// <typeparam name="TUser">The type of user objects.</typeparam>
/// <typeparam name="TRole">The type of role objects.</typeparam>
/// <typeparam name="TUserClaim">The type of the user claim object.</typeparam>
/// <typeparam name="TUserRole">The type of the user role object.</typeparam>
/// <typeparam name="TRoleClaim">The type of the role claim object.</typeparam>
public abstract class
IdentityDbContext<TUser, TRole, TUserClaim, TUserRole, TRoleClaim> : IdentityUserContext<TUser, TUserClaim>
where TUser : IdentityUser
where TRole : IdentityRole
where TUserClaim : IdentityUserClaim
where TUserRole : IdentityUserRole
where TRoleClaim : IdentityRoleClaim
{
    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="options">The options to be used by a <see cref="DbContext"/>.</param>
    public IdentityDbContext(DbContextOptions options) : base(options) { }

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    protected IdentityDbContext() { }

    /// <summary>
    /// Gets or sets the <see cref="DbSet{TEntity}"/> of User roles.
    /// </summary>
    public virtual DbSet<TUserRole> UserRoles { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="DbSet{TEntity}"/> of roles.
    /// </summary>
    public virtual DbSet<TRole> Roles { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="DbSet{TEntity}"/> of role claims.
    /// </summary>
    public virtual DbSet<TRoleClaim> RoleClaims { get; set; }

    /// <summary>
    /// Configures the schema needed for the identity framework.
    /// </summary>
    /// <param name="builder">
    /// The builder being used to construct the model for this context.
    /// </param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<TUser>(b => { b.HasMany<TUserRole>().WithOne().HasForeignKey(ur => ur.UserId).IsRequired(); });

        builder.Entity<TRole>(b =>
        {
            b.HasKey(r => r.Id);
            b.HasIndex(r => r.NormalizedName).HasDatabaseName("RoleNameIndex").IsUnique();
            b.ToTable("BFRoles");
            b.Property(r => r.ConcurrencyStamp).IsConcurrencyToken();

            b.Property(u => u.Name).HasMaxLength(256);
            b.Property(u => u.NormalizedName).HasMaxLength(256);

            b.HasMany<TUserRole>().WithOne().HasForeignKey(ur => ur.RoleId).IsRequired();
            b.HasMany<TRoleClaim>().WithOne().HasForeignKey(rc => rc.RoleId).IsRequired();
        });

        builder.Entity<TRoleClaim>(b =>
        {
            b.HasKey(rc => rc.Id);
            b.ToTable("BFRoleClaims");
        });

        builder.Entity<TUserRole>(b =>
        {
            b.HasKey(r => new { r.UserId, r.RoleId });
            b.ToTable("BFUserRoles");
        });
    }
}