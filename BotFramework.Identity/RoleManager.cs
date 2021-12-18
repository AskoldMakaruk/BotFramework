﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace BotFramework.Identity;

/// <summary>
/// Provides the APIs for managing roles in a persistence store.
/// </summary>
/// <typeparam name="TRole">The type encapsulating a role.</typeparam>
public class RoleManager<TRole> : IDisposable where TRole : class
{
    private bool _disposed;

    /// <summary>
    /// The cancellation token used to cancel operations.
    /// </summary>
    protected virtual CancellationToken CancellationToken => CancellationToken.None;

    /// <summary>
    /// Constructs a new instance of <see cref="RoleManager{TRole}"/>.
    /// </summary>
    /// <param name="store">The persistence store the manager will operate over.</param>
    /// <param name="logger">The logger used to log messages, warnings and errors.</param>
    public RoleManager(IRoleStore<TRole>           store,
                       ILogger<RoleManager<TRole>> logger)
    {
        Store  = store ?? throw new ArgumentNullException(nameof(store));
        Logger = logger;
    }

    /// <summary>
    /// Gets the persistence store this instance operates over.
    /// </summary>
    /// <value>The persistence store this instance operates over.</value>
    protected IRoleStore<TRole> Store { get; private set; }

    /// <summary>
    /// Gets the <see cref="ILogger"/> used to log messages from the manager.
    /// </summary>
    /// <value>
    /// The <see cref="ILogger"/> used to log messages from the manager.
    /// </value>
    public virtual ILogger Logger { get; set; }

    /// <summary>
    /// Gets an IQueryable collection of Roles if the persistence store is an <see cref="IQueryableRoleStore{TRole}"/>,
    /// otherwise throws a <see cref="NotSupportedException"/>.
    /// </summary>
    /// <value>An IQueryable collection of Roles if the persistence store is an <see cref="IQueryableRoleStore{TRole}"/>.</value>
    /// <exception cref="NotSupportedException">Thrown if the persistence store is not an <see cref="IQueryableRoleStore{TRole}"/>.</exception>
    /// <remarks>
    /// Callers to this property should use <see cref="SupportsQueryableRoles"/> to ensure the backing role store supports 
    /// returning an IQueryable list of roles.
    /// </remarks>
    public virtual IQueryable<TRole> Roles
    {
        get
        {
            var queryableStore = Store as IQueryableRoleStore<TRole>;
            if (queryableStore == null)
            {
                throw new NotSupportedException("StoreNotIQueryableRoleStore");
            }

            return queryableStore.Roles;
        }
    }

    /// <summary>
    /// Gets a flag indicating whether the underlying persistence store supports returning an <see cref="IQueryable"/> collection of roles.
    /// </summary>
    /// <value>
    /// true if the underlying persistence store supports returning an <see cref="IQueryable"/> collection of roles, otherwise false.
    /// </value>
    public virtual bool SupportsQueryableRoles
    {
        get
        {
            ThrowIfDisposed();
            return Store is IQueryableRoleStore<TRole>;
        }
    }

    /// <summary>
    /// Gets a flag indicating whether the underlying persistence store supports <see cref="Claim"/>s for roles.
    /// </summary>
    /// <value>
    /// true if the underlying persistence store supports <see cref="Claim"/>s for roles, otherwise false.
    /// </value>
    public virtual bool SupportsRoleClaims
    {
        get
        {
            ThrowIfDisposed();
            return Store is IRoleClaimStore<TRole>;
        }
    }

    /// <summary>
    /// Creates the specified <paramref name="role"/> in the persistence store.
    /// </summary>
    /// <param name="role">The role to create.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation.
    /// </returns>
    public virtual async Task<IdentityResult> CreateAsync(TRole role)
    {
        ThrowIfDisposed();
        if (role == null)
        {
            throw new ArgumentNullException(nameof(role));
        }

        await UpdateNormalizedRoleNameAsync(role);
        var result = await Store.CreateAsync(role, CancellationToken);
        return result;
    }

    /// <summary>
    /// Updates the normalized name for the specified <paramref name="role"/>.
    /// </summary>
    /// <param name="role">The role whose normalized name needs to be updated.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation.
    /// </returns>
    public virtual async Task UpdateNormalizedRoleNameAsync(TRole role)
    {
        var name = await GetRoleNameAsync(role);
        await Store.SetNormalizedRoleNameAsync(role, name, CancellationToken);
    }

    /// <summary>
    /// Updates the specified <paramref name="role"/>.
    /// </summary>
    /// <param name="role">The role to updated.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> for the update.
    /// </returns>
    public virtual Task<IdentityResult> UpdateAsync(TRole role)
    {
        ThrowIfDisposed();
        if (role == null)
        {
            throw new ArgumentNullException(nameof(role));
        }

        return UpdateRoleAsync(role);
    }

    /// <summary>
    /// Deletes the specified <paramref name="role"/>.
    /// </summary>
    /// <param name="role">The role to delete.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> for the delete.
    /// </returns>
    public virtual Task<IdentityResult> DeleteAsync(TRole role)
    {
        ThrowIfDisposed();
        if (role == null)
        {
            throw new ArgumentNullException(nameof(role));
        }

        return Store.DeleteAsync(role, CancellationToken);
    }

    /// <summary>
    /// Gets a flag indicating whether the specified <paramref name="roleName"/> exists.
    /// </summary>
    /// <param name="roleName">The role name whose existence should be checked.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing true if the role name exists, otherwise false.
    /// </returns>
    public virtual async Task<bool> RoleExistsAsync(string roleName)
    {
        ThrowIfDisposed();
        if (roleName == null)
        {
            throw new ArgumentNullException(nameof(roleName));
        }

        return await FindByNameAsync(roleName) != null;
    }

    /// <summary>
    /// Finds the role associated with the specified <paramref name="roleId"/> if any.
    /// </summary>
    /// <param name="roleId">The role ID whose role should be returned.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the role 
    /// associated with the specified <paramref name="roleId"/>
    /// </returns>
    public virtual Task<TRole> FindByIdAsync(long roleId)
    {
        ThrowIfDisposed();
        return Store.FindByIdAsync(roleId, CancellationToken);
    }

    /// <summary>
    /// Gets the name of the specified <paramref name="role"/>.
    /// </summary>
    /// <param name="role">The role whose name should be retrieved.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the name of the 
    /// specified <paramref name="role"/>.
    /// </returns>
    public virtual Task<string> GetRoleNameAsync(TRole role)
    {
        ThrowIfDisposed();
        return Store.GetRoleNameAsync(role, CancellationToken);
    }

    /// <summary>
    /// Sets the name of the specified <paramref name="role"/>.
    /// </summary>
    /// <param name="role">The role whose name should be set.</param>
    /// <param name="name">The name to set.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
    /// of the operation.
    /// </returns>
    public virtual async Task<IdentityResult> SetRoleNameAsync(TRole role, string name)
    {
        ThrowIfDisposed();

        await Store.SetRoleNameAsync(role, name, CancellationToken);
        await UpdateNormalizedRoleNameAsync(role);
        return IdentityResult.Success;
    }

    /// <summary>
    /// Gets the ID of the specified <paramref name="role"/>.
    /// </summary>
    /// <param name="role">The role whose ID should be retrieved.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the ID of the 
    /// specified <paramref name="role"/>.
    /// </returns>
    public virtual Task<long> GetRoleIdAsync(TRole role)
    {
        ThrowIfDisposed();
        return Store.GetRoleIdAsync(role, CancellationToken);
    }

    /// <summary>
    /// Finds the role associated with the specified <paramref name="roleName"/> if any.
    /// </summary>
    /// <param name="roleName">The name of the role to be returned.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the role 
    /// associated with the specified <paramref name="roleName"/>
    /// </returns>
    public virtual Task<TRole> FindByNameAsync(string roleName)
    {
        ThrowIfDisposed();
        if (roleName == null)
        {
            throw new ArgumentNullException(nameof(roleName));
        }

        return Store.FindByNameAsync(roleName, CancellationToken);
    }

    /// <summary>
    /// Adds a claim to a role.
    /// </summary>
    /// <param name="role">The role to add the claim to.</param>
    /// <param name="claim">The claim to add.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
    /// of the operation.
    /// </returns>
    public virtual async Task<IdentityResult> AddClaimAsync(TRole role, Claim claim)
    {
        ThrowIfDisposed();
        var claimStore = GetClaimStore();
        if (claim == null)
        {
            throw new ArgumentNullException(nameof(claim));
        }

        if (role == null)
        {
            throw new ArgumentNullException(nameof(role));
        }

        await claimStore.AddClaimAsync(role, claim, CancellationToken);
        return await UpdateRoleAsync(role);
    }

    /// <summary>
    /// Removes a claim from a role.
    /// </summary>
    /// <param name="role">The role to remove the claim from.</param>
    /// <param name="claim">The claim to remove.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
    /// of the operation.
    /// </returns>
    public virtual async Task<IdentityResult> RemoveClaimAsync(TRole role, Claim claim)
    {
        ThrowIfDisposed();
        var claimStore = GetClaimStore();
        if (role == null)
        {
            throw new ArgumentNullException(nameof(role));
        }

        await claimStore.RemoveClaimAsync(role, claim, CancellationToken);
        return await UpdateRoleAsync(role);
    }

    /// <summary>
    /// Gets a list of claims associated with the specified <paramref name="role"/>.
    /// </summary>
    /// <param name="role">The role whose claims should be returned.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the list of <see cref="Claim"/>s
    /// associated with the specified <paramref name="role"/>.
    /// </returns>
    public virtual Task<IList<Claim>> GetClaimsAsync(TRole role)
    {
        ThrowIfDisposed();
        var claimStore = GetClaimStore();
        if (role == null)
        {
            throw new ArgumentNullException(nameof(role));
        }

        return claimStore.GetClaimsAsync(role, CancellationToken);
    }

    /// <summary>
    /// Releases all resources used by the role manager.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
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
        }

        _disposed = true;
    }

    /// <summary>
    /// Called to update the role after validating and updating the normalized role name.
    /// </summary>
    /// <param name="role">The role.</param>
    /// <returns>Whether the operation was successful.</returns>
    protected virtual async Task<IdentityResult> UpdateRoleAsync(TRole role)
    {
        await UpdateNormalizedRoleNameAsync(role);
        return await Store.UpdateAsync(role, CancellationToken);
    }

    // IRoleClaimStore methods
    private IRoleClaimStore<TRole> GetClaimStore()
    {
        if (Store is not IRoleClaimStore<TRole> cast)
        {
            throw new NotSupportedException("StoreNotIRoleClaimStore");
        }

        return cast;
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