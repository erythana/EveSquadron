using System.Collections.Generic;
using System.Linq;

namespace EVEye.Extensions;

public static class EnumerableExtensions
{
    public static IEnumerable<T> Concatenate<T>(this IEnumerable<T> source, params IEnumerable<T>[] lists)
    {
        return source.Concat(lists.SelectMany(x => x));
    }
}