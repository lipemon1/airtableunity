using System;
using System.Collections.Generic;
using System.Linq;

public static class EnumerableExtension
{
    public static string ToQuery(this IEnumerable<string> values)
    {
        return String.Join(",", values.Where(l => l != null).ToArray());
    }
}