using System;
using System.Reflection;
using BotFramework.Abstractions;
using BotFramework.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BotFramework.Identity;

/// <summary>
/// Contains extension methods to <see cref="IServiceCollection"/> for configuring identity builder.
/// </summary>
public static class IdentityServiceCollectionExtensions
{
    public static IdentityBuilder AddIdentityWithPersistence<TUser, TRole>(
        this IAppBuilder services)
    where TUser : IdentityUser, IUserCommandState, new()
    where TRole : class
    {
        services.Services.AddScoped<IUserCommandState>(provider => provider.GetService<UserContext<TUser>>()?.User!);
        return services.AddIdentity<TUser, TRole>();
    }

    /// <summary>
    /// Adds the default identity system configuration for the specified User and Role types.
    /// </summary>
    /// <typeparam name="TUser">The type representing a User in the system.</typeparam>
    /// <typeparam name="TRole">The type representing a Role in the system.</typeparam>
    /// <param name="services">The builder available in the application.</param>
    /// <returns>An <see cref="IdentityBuilder"/> for creating and configuring the identity system.</returns>
    public static IdentityBuilder AddIdentity<TUser, TRole>(
        this IAppBuilder services)
    where TUser : IdentityUser, new()
    where TRole : class
        => services.AddIdentity<TUser, TRole>(setupAction: null);

    /// <summary>
    /// Adds and configures the identity system for the specified User and Role types.
    /// </summary>
    /// <typeparam name="TUser">The type representing a User in the system.</typeparam>
    /// <typeparam name="TRole">The type representing a Role in the system.</typeparam>
    /// <param name="builder">The builder available in the application.</param>
    /// <param name="setupAction">An action to configure the <see cref="IdentityOptions"/>.</param>
    /// <returns>An <see cref="IdentityBuilder"/> for creating and configuring the identity system.</returns>
    public static IdentityBuilder AddIdentity<TUser, TRole>(
        this IAppBuilder        builder,
        Action<IdentityOptions> setupAction)
    where TUser : IdentityUser, new()
    where TRole : class
    {
        var services = builder.Services;

        // Builder used by identity
        services.AddScoped<UserContext<TUser>>();
        builder.Services.AddScoped(provider => provider.GetService<UserContext<TUser>>()?.User!);
        builder.UseMiddleware<IdentityMiddleware<TUser>>();

        // No interface for the error describer so we can add errors without rev'ing the interface
        services.TryAddScoped<IUserConfirmation<TUser>, DefaultUserConfirmation<TUser>>();
        services.TryAddScoped<UserManager<TUser>>();
        services.TryAddScoped<RoleManager<TRole>>();

        if (setupAction != null)
        {
            services.Configure(setupAction);
        }

        return new IdentityBuilder(typeof(TUser), typeof(TRole), builder);
    }
}

/// <summary>
/// Helper functions for configuring identity builder.
/// </summary>
public class IdentityBuilder
{
    /// <summary>
    /// Creates a new instance of <see cref="IdentityBuilder"/>.
    /// </summary>
    /// <param name="user">The <see cref="Type"/> to use for the users.</param>
    /// <param name="builder">The <see cref="IServiceCollection"/> to attach to.</param>
    public IdentityBuilder(Type user, IAppBuilder builder)
    {
        UserType = user;
        Builder  = builder;
    }

    /// <summary>
    /// Creates a new instance of <see cref="IdentityBuilder"/>.
    /// </summary>
    /// <param name="user">The <see cref="Type"/> to use for the users.</param>
    /// <param name="role">The <see cref="Type"/> to use for the roles.</param>
    /// <param name="services">The <see cref="IServiceCollection"/> to attach to.</param>
    public IdentityBuilder(Type user, Type role, IAppBuilder services) : this(user, services)
        => RoleType = role;

    /// <summary>
    /// Gets the <see cref="Type"/> used for users.
    /// </summary>
    /// <value>
    /// The <see cref="Type"/> used for users.
    /// </value>
    public Type UserType { get; private set; }


    /// <summary>
    /// Gets the <see cref="Type"/> used for roles.
    /// </summary>
    /// <value>
    /// The <see cref="Type"/> used for roles.
    /// </value>
    public Type RoleType { get; private set; }

    /// <summary>
    /// Gets the <see cref="IServiceCollection"/> builder are attached to.
    /// </summary>
    /// <value>
    /// The <see cref="IServiceCollection"/> builder are attached to.
    /// </value>
    public IAppBuilder Builder { get; private set; }

    private IdentityBuilder AddScoped(Type serviceType, Type concreteType)
    {
        Builder.Services.AddScoped(serviceType, concreteType);
        return this;
    }

    /// <summary>
    /// Adds an <see cref="IUserStore{TUser}"/> for the <see cref="UserType"/>.
    /// </summary>
    /// <typeparam name="TStore">The user store type.</typeparam>
    /// <returns>The current <see cref="IdentityBuilder"/> instance.</returns>
    public virtual IdentityBuilder AddUserStore<TStore>() where TStore : class
        => AddScoped(typeof(IUserStore<>).MakeGenericType(UserType), typeof(TStore));

    /// <summary>
    /// Adds a <see cref="UserManager{TUser}"/> for the <see cref="UserType"/>.
    /// </summary>
    /// <typeparam name="TUserManager">The type of the user manager to add.</typeparam>
    /// <returns>The current <see cref="IdentityBuilder"/> instance.</returns>
    public virtual IdentityBuilder AddUserManager<TUserManager>() where TUserManager : class
    {
        var userManagerType = typeof(UserManager<>).MakeGenericType(UserType);
        var customType      = typeof(TUserManager);
        if (!userManagerType.GetTypeInfo().IsAssignableFrom(customType.GetTypeInfo()))
        {
            throw new InvalidOperationException($"InvalidManagerType({customType.Name}");
        }

        if (userManagerType != customType)
        {
            Builder.Services.AddScoped(customType, services => services.GetRequiredService(userManagerType));
        }

        return AddScoped(userManagerType, customType);
    }

    /// <summary>
    /// Adds Role related builder for TRole, including IRoleStore, IRoleValidator, and RoleManager.
    /// </summary>
    /// <typeparam name="TRole">The role type.</typeparam>
    /// <returns>The current <see cref="IdentityBuilder"/> instance.</returns>
    public virtual IdentityBuilder AddRoles<TRole>() where TRole : class
    {
        RoleType = typeof(TRole);
        Builder.Services.TryAddScoped<RoleManager<TRole>>();
        return this;
    }


    /// <summary>
    /// Adds a <see cref="IRoleStore{TRole}"/> for the <see cref="RoleType"/>.
    /// </summary>
    /// <typeparam name="TStore">The role store.</typeparam>
    /// <returns>The current <see cref="IdentityBuilder"/> instance.</returns>
    public virtual IdentityBuilder AddRoleStore<TStore>() where TStore : class
    {
        if (RoleType == null)
        {
            throw new InvalidOperationException("NoRoleType");
        }

        return AddScoped(typeof(IRoleStore<>).MakeGenericType(RoleType), typeof(TStore));
    }

    /// <summary>
    /// Adds a <see cref="RoleManager{TRole}"/> for the <see cref="RoleType"/>.
    /// </summary>
    /// <typeparam name="TRoleManager">The type of the role manager to add.</typeparam>
    /// <returns>The current <see cref="IdentityBuilder"/> instance.</returns>
    public virtual IdentityBuilder AddRoleManager<TRoleManager>() where TRoleManager : class
    {
        if (RoleType == null)
        {
            throw new InvalidOperationException("NoRoleType");
        }

        var managerType = typeof(RoleManager<>).MakeGenericType(RoleType);
        var customType  = typeof(TRoleManager);
        if (!managerType.GetTypeInfo().IsAssignableFrom(customType.GetTypeInfo()))
        {
            throw new InvalidOperationException($"FormatInvalidManagerType {customType.Name}, {RoleType.Name}");
        }

        if (managerType != customType)
        {
            Builder.Services.AddScoped(typeof(TRoleManager), services => services.GetRequiredService(managerType));
        }

        return AddScoped(managerType, typeof(TRoleManager));
    }
}