using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using FrodLib.Collections.Concurrent;

namespace FrodLib.Extensions
{
    public static class CollectionExtensions
    {
        public static ObservableCollection<T> ToObservableCollection<T>(IEnumerable<T> collection)
        {
            return new ObservableCollection<T>(collection);
        }

        public static IList<T> MakeConcurrent<T>(this IList<T> collection)
        {
            return new ConcurrentCollectionWrapper<T>(collection);
        }

        public static void AddRange<T>(this ICollection<T> collection, params T[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                collection.Add(values[i]);
            }
        }

        public static bool IsEmpty<T>(this IEnumerable<T> collection)
        {
            if (collection is ICollection<T>)
            {
                return ((ICollection<T>)collection).Count == 0;
            }
            else
            {
                return collection.Any() == false;
            }
        }

        public static T[] Slice<T>(this IList<T> source, int start, int end)
        {
            if(end < 0)
            {
                end = source.Count + end;
            }

            int length = end - start;

            T[] res = new T[length];
            for(int i = 0; i< length; i++)
            {
                res[i] = source[start + i];
            }
            return res;
        }

        /// <summary>
        /// Checks if the two lists and it's content are equal
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="l1"></param>
        /// <param name="l2"></param>
        /// <returns></returns>
        public static bool AreEquals<T>(this IList<T> l1, IList<T> l2)
        {
            if (l1 == null || l2 == null)
            {
                return false;
            }
            if(object.ReferenceEquals(l1, l2))
            {
                return true;
            }
            if (l1.Count != l2.Count)
            {
                return false;
            }

            for (int i = 0; i < l1.Count; i++)
            {
                if (!l1[i].Equals(l2[i]))
                {
                    return false;
                }
            }

            return true;
        }

        internal static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var item in collection)
            {
                action(item);
            }
        }
    }
}
