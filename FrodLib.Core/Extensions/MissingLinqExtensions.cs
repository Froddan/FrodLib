using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace FrodLib.Extensions
{
    internal static class MissingLinqExtensions
    {
        internal static int FindIndex<T>(this IList<T> list, Func<T, bool> predicate)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if(predicate(list[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        internal static IEnumerable<char> Where(this string s, Func<char, bool> predicate)
        {
            foreach (var c in s)
            {
                if (predicate(c))
                {
                    yield return c;
                }
            }
        }

        internal static int Count(this string s, Func<char, bool> predicate)
        {
            int count = 0;
            foreach (var c in s)
            {
                if (predicate(c))
                {
                    count++;
                }
            }

            return count;
        }
    }
}
