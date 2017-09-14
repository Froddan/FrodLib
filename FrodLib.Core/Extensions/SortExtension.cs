using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrodLib.Extensions
{
    public static class SortExtension
    {
        public static void QuickSort<TSource, TKey>(this IList<TSource> source, Func<TSource, TKey> keySelector, bool desc = false)
        {
            if (source == null) return;

            Comparer<TKey> comparer = Comparer<TKey>.Default;
            QuickSort(source, 0, source.Count - 1, keySelector, comparer, desc);
        }

        private static void QuickSort<TSource, TKey>(IList<TSource> source, int left, int right, Func<TSource, TKey> keySelector, Comparer<TKey> comparer, bool desc)
        {
            int i = left;
            int j = right;
            double pivotValue = ((left + right) / 2);
            TSource x = source[Convert.ToInt32(pivotValue)];
            TSource w = default(TSource);
            while (i <= j)
            {

                if (desc)
                {
                    while (comparer.Compare(keySelector(source[i]), keySelector(x)) > 0)
                    {
                        i++;
                    }
                    while (comparer.Compare(keySelector(x), keySelector(source[j])) > 0)
                    {
                        j--;
                    }

                    if (i <= j)
                    {
                        w = source[i];
                        source[i++] = source[j];
                        source[j--] = w;
                    }
                }
                else
                {
                    while (comparer.Compare(keySelector(source[i]), keySelector(x)) < 0)
                    {
                        i++;
                    }
                    while (comparer.Compare(keySelector(x), keySelector(source[j])) < 0)
                    {
                        j--;
                    }

                    if (i <= j)
                    {
                        w = source[i];
                        source[i++] = source[j];
                        source[j--] = w;
                    }
                }
            }
            if (left < j)
            {
                QuickSort(source, left, j, keySelector, comparer, desc);
            }
            if (i < right)
            {
                QuickSort(source, i, right, keySelector, comparer, desc);
            }
        }
    }
}
