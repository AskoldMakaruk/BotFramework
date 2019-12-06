using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Monads
{
    public struct Optional<T>
    {
        private readonly T value;
        public bool IsEmpty => EqualityComparer<T>.Default.Equals(value, default);

        public Optional(T value) =>
            this.value = value;

        public Optional<T2> Bind<T2>(Func<T, Optional<T2>> func) => IsEmpty ? new Optional<T2>() : func.Invoke(value);


        public T2 FromOptional<T2>(Func<T, T2> func, T2 def) => IsEmpty ? def : func.Invoke(value);
        public T FromOptional(T def) => IsEmpty ? def : value;

        public void FromOptional(Action<T> action)
        {
            if (!IsEmpty)
                action.Invoke(value);
        }
        public void FromOptional(Action<T> action, Action a)
        {
            if (!IsEmpty)
                action.Invoke(value);
            else a.Invoke();
        }

        public static implicit operator Optional<T>(T val) =>
            new Optional<T>(val);
    }

    public static class Optional
    {
        public static Optional<T> FirstAsOptional<T>(this IEnumerable<T> t) =>
            new Optional<T>(t.FirstOrDefault());

        public static Optional<T> FirstAsOptional<T>(this IEnumerable<T> t, Func<T, bool> func) =>
            new Optional<T>(t.FirstOrDefault(func));

        public static Optional<T> ToOptional<T>(this T t) =>
            new Optional<T>(t);
    }
}