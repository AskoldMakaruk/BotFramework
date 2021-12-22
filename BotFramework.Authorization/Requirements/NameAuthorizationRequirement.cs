// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace BotFramework.Authorization.Requirements;

/// <summary>
/// Implements an <see cref="IAuthorizationHandler"/> and <see cref="IAuthorizationRequirement"/>
/// which requires the current user name must match the specified value.
/// </summary>
public class NameAuthorizationRequirement : AuthorizationHandler<NameAuthorizationRequirement>, IAuthorizationRequirement
{
    /// <summary>
    /// Constructs a new instance of <see cref="NameAuthorizationRequirement"/>.
    /// </summary>
    /// <param name="requiredName">The required name that the current user must have.</param>
    public NameAuthorizationRequirement(string requiredName)
    {
        RequiredName = requiredName ?? throw new ArgumentNullException(nameof(requiredName));
    }

    /// <summary>
    /// Gets the required name that the current user must have.
    /// </summary>
    public string RequiredName { get; }

    /// <summary>
    /// Makes a decision if authorization is allowed based on a specific requirement.
    /// </summary>
    /// <param name="context">The authorization context.</param>
    /// <param name="requirement">The requirement to evaluate.</param>
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, NameAuthorizationRequirement requirement)
    {
        if (context.User.Identities.Any(i => string.Equals(i.Name, requirement.RequiredName)))
        {
            context.Succeed(requirement);
        }
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{nameof(NameAuthorizationRequirement)}:Requires a user identity with Name equal to {RequiredName}";
    }
}