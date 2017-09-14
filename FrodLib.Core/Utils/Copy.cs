using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FrodLib.Extensions;

namespace FrodLib.Utils
{
    public static class Copy
    {
        public static void CopyTo<T>(this T from, T to)
        {
            CopyTo(from, to, null);
        }

        public static void CopyTo<T>(this T from, T to, Type withPropertyAttribute)
        {
            if (from == null || to == null) return;

            foreach (var prop in from.GetProperties(withPropertyAttribute))
            {
                var value = prop.GetValue(from, null);
                prop.SetValue(to, value, null);
            }
        }
    }
}
