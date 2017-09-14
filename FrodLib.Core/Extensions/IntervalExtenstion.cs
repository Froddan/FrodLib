using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrodLib.Extensions
{
    public static class IntervalExtenstion
    {

        public static bool IsInRange<T>(this T actualValue, T lower, T upper, bool includeUpperLimit = true, bool includeLowerLimit = true) where T : IComparable<T>
        {
            if (lower.CompareTo(upper) > 0)
            {
                Swap(ref lower, ref upper);
            }

            bool isBiggerThen;
            bool isLesserThen;

            if (includeLowerLimit)
            {
                isBiggerThen = actualValue.CompareTo(lower) >= 0;
            }
            else
            {
                isBiggerThen = actualValue.CompareTo(lower) > 0;
            }
            if (includeUpperLimit)
            {
                isLesserThen = actualValue.CompareTo(upper) <= 0;
            }
            else
            {
                isLesserThen = actualValue.CompareTo(upper) < 0;
            }

            return isBiggerThen && isLesserThen;
        }

        private static void Swap<T>(ref T t1, ref T t2) where T : IComparable<T>
        {
            T tmp = t1;
            t1 = t2;
            t2 = t1;
        }
    }
}
