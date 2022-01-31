// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace BotFramework.Authorization;

/// <summary>
/// Authorization extension methods for <see cref="IEndpointConventionBuilder"/>.
/// </summary>
public static class AuthorizationEndpointConventionBuilderExtensions
{
    private static readonly IAllowAnonymous _allowAnonymousMetadata = new AllowAnonymousAttribute();

    /// <summary>
    /// Adds the default authorization policy to the endpoint(s).
    /// </summary>
    /// <param name="builder">The endpoint convention builder.</param>
    /// <returns>The original convention builder parameter.</returns>
    public static TBuilder RequireAuthorization<TBuilder>(this TBuilder builder) where TBuilder : IEndpointConventionBuilder
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.RequireAuthorization(new AuthorizeAttribute());
    }

    /// <summary>
    /// Adds authorization policies with the specified names to the endpoint(s).
    /// </summary>
    /// <param name="builder">The endpoint convention builder.</param>
    /// <param name="policyNames">A collection of policy names. If empty, the default authorization policy will be used.</param>
    /// <returns>The original convention builder parameter.</returns>
    public static TBuilder RequireAuthorization<TBuilder>(this TBuilder builder, params string[] policyNames) where TBuilder : IEndpointConventionBuilder
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        if (policyNames == null)
        {
            throw new ArgumentNullException(nameof(policyNames));
        }

        return builder.RequireAuthorization(policyNames.Select(n => new AuthorizeAttribute(n)).ToArray());
    }

    /// <summary>
    /// Adds authorization policies with the specified <see cref="IAuthorizeData"/> to the endpoint(s).
    /// </summary>
    /// <param name="builder">The endpoint convention builder.</param>
    /// <param name="authorizeData">
    /// A collection of <paramref name="authorizeData"/>. If empty, the default authorization policy will be used.
    /// </param>
    /// <returns>The original convention builder parameter.</returns>
    public static TBuilder RequireAuthorization<TBuilder>(this TBuilder builder, params IAuthorizeData[] authorizeData)
    where TBuilder : IEndpointConventionBuilder
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        if (authorizeData == null)
        {
            throw new ArgumentNullException(nameof(authorizeData));
        }

        if (authorizeData.Length == 0)
        {
            authorizeData = new IAuthorizeData[] { new AuthorizeAttribute(), };
        }

        RequireAuthorizationCore(builder, authorizeData);
        return builder;
    }

    /// <summary>
    /// Allows anonymous access to the endpoint by adding <see cref="AllowAnonymousAttribute" /> to the endpoint metadata. This will bypass
    /// all authorization checks for the endpoint including the default authorization policy and fallback authorization policy.
    /// </summary>
    /// <param name="builder">The endpoint convention builder.</param>
    /// <returns>The original convention builder parameter.</returns>
    public static TBuilder AllowAnonymous<TBuilder>(this TBuilder builder) where TBuilder : IEndpointConventionBuilder
    {
        builder.Add(endpointBuilder =>
        {
            endpointBuilder.Metadata.Add(_allowAnonymousMetadata);
        });
        return builder;
    }

    private static void RequireAuthorizationCore<TBuilder>(TBuilder builder, IEnumerable<IAuthorizeData> authorizeData)
    where TBuilder : IEndpointConventionBuilder
    {
        builder.Add(endpointBuilder =>
        {
            foreach (var data in authorizeData)
            {
                endpointBuilder.Metadata.Add(data);
            }
        });
    }
}