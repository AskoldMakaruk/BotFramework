namespace BotFramework.Authorization.Requirements;

/// <summary>
/// Implements an <see cref="IAuthorizationHandler"/> and <see cref="IAuthorizationRequirement"/>
/// which requires at least one role claim whose value must be any of the allowed roles.
/// </summary>
public class RolesAuthorizationRequirement : AuthorizationHandler<RolesAuthorizationRequirement>, IAuthorizationRequirement
{
    /// <summary>
    /// Creates a new instance of <see cref="RolesAuthorizationRequirement"/>.
    /// </summary>
    /// <param name="allowedRoles">A collection of allowed roles.</param>
    public RolesAuthorizationRequirement(IList<string> allowedRoles)
    {
        if (allowedRoles == null)
        {
            throw new ArgumentNullException(nameof(allowedRoles));
        }

        if (!allowedRoles.Any())
        {
            throw new InvalidOperationException("Resources.Exception_RoleRequirementEmpty");
        }
        AllowedRoles = allowedRoles;
    }

    /// <summary>
    /// Gets the collection of allowed roles.
    /// </summary>
    public IEnumerable<string> AllowedRoles { get; }

    /// <summary>
    /// Makes a decision if authorization is allowed based on a specific requirement.
    /// </summary>
    /// <param name="context">The authorization context.</param>
    /// <param name="requirement">The requirement to evaluate.</param>

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RolesAuthorizationRequirement requirement)
    {
        bool found = false;
        if (!requirement.AllowedRoles.Any())
        {
            // Review: What do we want to do here?  No roles requested is auto success?
        }
        else
        {
            found = requirement.AllowedRoles.Any(r => context.User.IsInRole(r));
        }

        if (found)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        var roles = $"User.IsInRole must be true for one of the following roles: ({string.Join("|", AllowedRoles)})";

        return $"{nameof(RolesAuthorizationRequirement)}:{roles}";
    }
}