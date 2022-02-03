using System;
using BotFramework.Abstractions;
using BotFramework.Services.Factories;
using FluentCache;
using Microsoft.Extensions.Options;

namespace BotFramework.Services;

public class UserScopeStorage : IUserScopeStorage
{
    private readonly TimeSpan scopeTimeSpan;

    public UserScopeStorage(ICache cache, IOptions<UserScopeStorageOptions>? options = null)
    {
        _cache = cache.WithSource(_factory);

        if (options?.Value?.ScopeLifetime != null && options.Value.ScopeLifetime != TimeSpan.Zero)
        {
            scopeTimeSpan = options.Value.ScopeLifetime;
        }
        else
        {
            scopeTimeSpan = TimeSpan.FromMinutes(10);
        }
    }

    private readonly FeatureCollectionFactory        _factory = new();
    private readonly Cache<FeatureCollectionFactory> _cache;

    public IFeatureCollection Get(long usedId) =>
    _cache.Method(repo => repo.Get(usedId))
          .ExpireAfter(scopeTimeSpan)
          .GetValue();
}