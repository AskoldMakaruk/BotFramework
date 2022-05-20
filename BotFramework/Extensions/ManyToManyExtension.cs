using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace BotFramework.Extensions;

public static class ManyToManyExtension
{
    public static void UpdateManyToMany<T, TKey>(this DbSet<T> repository, ICollection<T> currentItems, ICollection<T> newItems,
                                                 Func<T, TKey> getKey,     Action<T, T>   map) where T : class
    {
        var joined = currentItems
                     .FullOuterJoin(newItems,
                         item => getKey(item),
                         item => getKey(item),
                         (db, toAdd, key) => (db, toAdd)
                     )
                     .ToList();

        var removed = new List<T>();
        var added   = new List<T>();

        foreach (var (db, toAdd) in joined)
        {
            // ignore
            if (db == null && toAdd == null)
            {
                continue;
            }

            // remove
            if (db != null && toAdd == null)
            {
                removed.Add(db);
                continue;
            }

            // add
            if (db == null)
            {
                added.Add(toAdd);
                continue;
            }

            // update
            map(db, toAdd);
        }

        repository.RemoveRange(removed);
        repository.AddRange(added);
    }
}