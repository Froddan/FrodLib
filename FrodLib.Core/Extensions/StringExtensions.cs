using FrodLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrodLib.Extensions
{
    public static class StringExtensions
    {
        public static string Format(this string @string, params object[] values)
        {
            ArgumentValidator.IsNotNull(@string, nameof(@string));

            return string.Format(@string, values);
        }

        public static bool IsNullOrEmpty(this string @string)
        {
            return string.IsNullOrEmpty(@string);
        }

        public static string Slice(this string source, int start)
        {
            return Slice(source, start, source.Length - 1);
        }

        public static string Slice(this string source, int start, int end)
        {
            if(end < 0)
            {
                end = source.Length + end;
            }

            int length = end - start;
            return source.Substring(start, length);
        }
    }
}
