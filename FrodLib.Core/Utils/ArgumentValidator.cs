using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrodLib.Utils
{
    public static class ArgumentValidator
    {
        public static void IsNotNull<TClass>(TClass argValue, string argName) where TClass : class
        {
            if (argValue == null)
            {
                throw new ArgumentNullException(argName);
            }
        }

        public static void IsNotNull<TStruct>(TStruct? argValue, string argName) where TStruct : struct
        {
            if (argValue == null)
            {
                throw new ArgumentNullException(argName);
            }
        }

        public static void IsGreaterThen<TType>(TType argValue, string argName, TType minAllowedValue) where TType : IComparable<TType>
        {
            if (argValue.CompareTo(minAllowedValue) <= 0)
            {
                throw new ArgumentOutOfRangeException(argName, argName + " has to be greater then " + minAllowedValue);
            }
        }

        public static void IsGreaterThenOrEqual<TType>(TType argValue, string argName, TType minAllowedValue) where TType : IComparable<TType>
        {
            if (argValue.CompareTo(minAllowedValue) < 0)
            {
                throw new ArgumentOutOfRangeException(argName, argName + " has to be greater then or equal to" + minAllowedValue);
            }
        }

        public static void IsLesserThen<TType>(TType argValue, string argName, TType maxAllowedValue) where TType : IComparable<TType>
        {
            if (argValue.CompareTo(maxAllowedValue) >= 0)
            {
                throw new ArgumentOutOfRangeException(argName, argName + " has to be lesser then " + maxAllowedValue);
            }
        }

        public static void IsLesserThenOrEqual<TType>(TType argValue, string argName, TType maxAllowedValue) where TType : IComparable<TType>
        {
            if (argValue.CompareTo(maxAllowedValue) > 0)
            {
                throw new ArgumentOutOfRangeException(argName, argName + " has to be lesser then or equal to " + maxAllowedValue);
            }
        }

        public static void IsInRange<TType>(TType argValue, string argName, TType minAllowedValue, TType maxAllowedValue) where TType : IComparable<TType>
        {
            if (argValue.CompareTo(minAllowedValue) < 0 || argValue.CompareTo(maxAllowedValue) > 0)
            {
                throw new ArgumentOutOfRangeException(argName, argName + " has to be in range [" + minAllowedValue + "," + maxAllowedValue + "]");
            }
        }
    }
}

namespace FrodLib.Extensions
{
    public static class ArgumentValidatorExtensions
    {
        public static void IsNotNull<TClass>(this TClass argValue, string argName) where TClass : class
        {
            FrodLib.Utils.ArgumentValidator.IsNotNull(argValue, argName);
        }

        public static void IsNotNull<TStruct>(this TStruct? argValue, string argName) where TStruct : struct
        {
            FrodLib.Utils.ArgumentValidator.IsNotNull(argValue, argName);
        }

        public static void IsGreaterThen<TType>(this TType argValue, string argName, TType minAllowedValue) where TType : IComparable<TType>
        {
            FrodLib.Utils.ArgumentValidator.IsGreaterThen(argValue, argName, minAllowedValue);
        }

        public static void IsGreaterThenOrEqual<TType>(this TType argValue, string argName, TType minAllowedValue) where TType : IComparable<TType>
        {
            FrodLib.Utils.ArgumentValidator.IsGreaterThenOrEqual(argValue, argName, minAllowedValue);
        }

        public static void IsLesserThen<TType>(this TType argValue, string argName, TType maxAllowedValue) where TType : IComparable<TType>
        {
            FrodLib.Utils.ArgumentValidator.IsLesserThen(argValue, argName, maxAllowedValue);
        }

        public static void IsLesserThenOrEqual<TType>(this TType argValue, string argName, TType maxAllowedValue) where TType : IComparable<TType>
        {
            FrodLib.Utils.ArgumentValidator.IsLesserThenOrEqual(argValue, argName, maxAllowedValue);
        }

        public static void IsInRange<TType>(this TType argValue, string argName, TType minAllowedValue, TType maxAllowedValue) where TType : IComparable<TType>
        {
            FrodLib.Utils.ArgumentValidator.IsInRange(argValue, argName, minAllowedValue, maxAllowedValue);
        }
    }
}
