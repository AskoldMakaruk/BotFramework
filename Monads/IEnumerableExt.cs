using System.Collections.Generic;

namespace Monads
{
    public static class IEnumerableExt
    {
        public static IEnumerable<T> AsEnumerable<T>(this T item)
        {
            yield return item;
        }
    }
}