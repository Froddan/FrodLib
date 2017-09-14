using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace FrodLib.Extensions
{
    public static class NumberExtensions
    {
        /// <summary>
        /// Checks if the object is a numeric type
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNumber(this object value)
        {
            if (value == null)
            {
                return false;
            }

            if (value is int) return true;
            if (value is uint) return true;
            if (value is long) return true;
            if (value is ulong) return true;
            if (value is short) return true;
            if (value is ushort) return true;
            if (value is float) return true;
            if (value is double) return true;
            if (value is decimal) return true;
            if (value is byte) return true;

            return false;
        }

        /// <summary>
        /// Convert string value to double ignore the culture.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>Decimal value.</returns>
        public static decimal ToDecimal(this string value)
        {
            value = value.Replace(" ", "");

            string tempValue = value;

            var punctuation = value.Where(x => char.IsPunctuation(x) && x != '-').Distinct();
            int count = punctuation.Count();

            NumberFormatInfo format = CultureInfo.InvariantCulture.NumberFormat;
            switch (count)
            {
                case 0:
                    break;

                case 1:
                    var firstPunctuation = punctuation.ElementAt(0);
                    var firstPunctuationOccurence = value.Where(x => x == firstPunctuation).Count();

                    if (firstPunctuationOccurence == 1)
                    {
                        // we assume it's a decimal separator (and not a group separator)
                        tempValue = value.Replace(firstPunctuation.ToString(), format.NumberDecimalSeparator);
                    }
                    else
                    {
                        // multiple occurence means that symbol is a group separator
                        tempValue = value.Replace(firstPunctuation.ToString(), format.NumberGroupSeparator);
                    }

                    break;

                case 2:
                    if (punctuation.ElementAt(0) == '.')
                        tempValue = value.SwapChar('.', ',');
                    break;

                default:
                    throw new InvalidCastException();
            }
            decimal number = decimal.Parse(tempValue, format);
            return number;
        }

        public static bool TryToDecimal(this string value, out decimal result)
        {
            try
            {
                result = ToDecimal(value);
                return true;
            }
            catch
            {
                result = 0;
                return false;
            }
        }

        /// <summary>
        /// Convert string value to double ignore the culture.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>Decimal value.</returns>
        public static double ToDouble(this string value)
        {
            value = value.Replace(" " , "");

            double number;
            string tempValue = value;

            var punctuation = value.Where(x => char.IsPunctuation(x) && x != '-').Distinct();
            int count = punctuation.Count();

            NumberFormatInfo format = CultureInfo.InvariantCulture.NumberFormat;
            switch (count)
            {
                case 0:
                    break;

                case 1:
                    var firstPunctuation = punctuation.ElementAt(0);
                    var firstPunctuationOccurence = value.Where(x => x == firstPunctuation).Count();

                    if (firstPunctuationOccurence == 1)
                    {
                        // we assume it's a decimal separator (and not a group separator)
                        tempValue = value.Replace(firstPunctuation.ToString(), format.NumberDecimalSeparator);
                    }
                    else
                    {
                        // multiple occurence means that symbol is a group separator
                        tempValue = value.Replace(firstPunctuation.ToString(), format.NumberGroupSeparator);
                    }

                    break;

                case 2:
                    if (punctuation.ElementAt(0) == '.')
                        tempValue = value.SwapChar('.', ',');
                    break;

                default:
                    throw new InvalidCastException();
            }
            if (tempValue == "INF") return double.PositiveInfinity;
            else if (tempValue == "-INF") return double.NegativeInfinity;

            number = double.Parse(tempValue, format);
            return number;
        }

        public static bool TryToDouble(this string value, out double result)
        {
            try
            {
                result = ToDouble(value);
                return true;
            }
            catch
            {
                result = 0;
                return false;
            } 
        }

        /// <summary>
        /// Swaps the char.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <returns></returns>
        private static string SwapChar(this string value, char from, char to)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            StringBuilder builder = new StringBuilder();

            foreach (var item in value)
            {
                char c = item;
                if (c == from)
                    c = to;
                else if (c == to)
                    c = from;

                builder.Append(c);
            }
            return builder.ToString();
        }
    }
}
