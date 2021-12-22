using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using BotFramework.Services.Clients;
using FluentCache;
using FluentCache.Simple;
using Telegram.Bot.Types;

namespace BotFramework.Middleware;

/// <summary>
/// Represents a collection of HTTP features.
/// </summary>
public interface IFeatureCollection : IEnumerable<KeyValuePair<Type, object>>
{
    /// <summary>
    /// Indicates if the collection can be modified.
    /// </summary>
    bool IsReadOnly { get; }

    /// <summary>
    /// Incremented for each modification and can be used to verify cached results.
    /// </summary>
    int Revision { get; }

    /// <summary>
    /// Gets or sets a given feature. Setting a null value removes the feature.
    /// </summary>
    /// <param name="key"></param>
    /// <returns>The requested feature, or null if it is not present.</returns>
    object? this[Type key] { get; set; }

    /// <summary>
    /// Retrieves the requested feature from the collection.
    /// </summary>
    /// <typeparam name="TFeature">The feature key.</typeparam>
    /// <returns>The requested feature, or null if it is not present.</returns>
    TFeature? Get<TFeature>();

    /// <summary>
    /// Sets the given feature in the collection.
    /// </summary>
    /// <typeparam name="TFeature">The feature key.</typeparam>
    /// <param name="instance">The feature value.</param>
    void Set<TFeature>(TFeature instance);
}

public class FeatureCollection : IFeatureCollection
{
    private static   KeyComparer                FeatureKeyComparer = new KeyComparer();
    private readonly IFeatureCollection?        _defaults;
    private          IDictionary<Type, object>? _features;
    private volatile int                        _containerRevision;

    public FeatureCollection() { }

    public FeatureCollection(IFeatureCollection defaults)
    {
        _defaults = defaults;
    }

    public virtual int Revision => _containerRevision + (_defaults?.Revision ?? 0);

    public bool IsReadOnly => false;

    public object? this[Type key]
    {
        get
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            object? result;
            return _features != null && _features.TryGetValue(key, out result) ? result : _defaults?[key];
        }
        set
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (value == null)
            {
                if (_features != null && _features.Remove(key))
                {
                    _containerRevision++;
                }

                return;
            }

            if (_features == null)
            {
                _features = new Dictionary<Type, object>();
            }

            _features[key] = value;
            _containerRevision++;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IEnumerator<KeyValuePair<Type, object>> GetEnumerator()
    {
        if (_features != null)
        {
            foreach (var pair in _features)
            {
                yield return pair;
            }
        }

        if (_defaults != null)
        {
            // Don't return features masked by the wrapper.
            foreach (var pair in _features == null ? _defaults : _defaults.Except(_features, FeatureKeyComparer))
            {
                yield return pair;
            }
        }
    }

    public TFeature? Get<TFeature>()
    {
        return (TFeature?)this[typeof(TFeature)];
    }

    public void Set<TFeature>(TFeature instance)
    {
        this[typeof(TFeature)] = instance;
    }

    private class KeyComparer : IEqualityComparer<KeyValuePair<Type, object>>
    {
        public bool Equals(KeyValuePair<Type, object> x, KeyValuePair<Type, object> y)
        {
            return x.Key == y.Key;
        }

        public int GetHashCode(KeyValuePair<Type, object> obj)
        {
            return obj.Key.GetHashCode();
        }
    }
}

public class FeatureCollectionRepo
{
    public IFeatureCollection Get() => new FeatureCollection();
}

public class UserScopeStorage
{
    private readonly IFeatureCollection _consumers = new FeatureCollection();

    private readonly ICache                c    = new FluentDictionaryCache();
    private readonly FeatureCollectionRepo Repo = new FeatureCollectionRepo();


    public T Get<T>(long usedId)
    {
        Cache<FeatureCollectionRepo> Features = c.WithSource(Repo);
        Features.Method(repo => repo.Get())
                .ExpireAfter(TimeSpan.FromMinutes(20))
                .GetValue();

        // return _cachingService.Get<T>(usedId.ToString());
    }
}

public class EndpointMiddleware
{
    private readonly UpdateDelegate   _next;
    private readonly UserScopeStorage _storage;


    public EndpointMiddleware(UpdateDelegate   next,
                              UserScopeStorage storage)
    {
        _next    = next;
        _storage = storage;
    }

    public Task Invoke(Update                 update,
                       UpdateContext          updateContext,
                       ICommandUpdateConsumer client)
    {
        var id = update.GetId()!.Value;

        var scope = _storage[id];
        if (scope != null)
        {
            scope.Consume(updateContext, client);
        }
        else
        {
            var a = new PriorityUpdateConsumer();

            a.Consume(updateContext, client);
        }

        return _next.Invoke(update);
    }
}