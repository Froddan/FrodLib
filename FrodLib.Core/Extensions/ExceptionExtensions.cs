using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace FrodLib.Extensions
{
    public static class ExceptionExtensions
    {
        [DebuggerStepThrough]
        public static void TrueThenThrow<T>(this bool predicate, string message, params object[] messageArguments) where T : Exception
        {
            if (predicate)
            {
                var exception = String.IsNullOrEmpty(message)
                    ? (T)Activator.CreateInstance(typeof(T), true)
                    : (T)Activator.CreateInstance(typeof(T), String.Format(message, messageArguments));

                throw exception;
            }
        }

        [DebuggerStepThrough]
        public static void FalseThenThrow<T>(this bool predicate, string message, params object[] messageArguments) where T : Exception
        {
            if (predicate == false)
            {
                var exception = String.IsNullOrEmpty(message)
                    ? (T)Activator.CreateInstance(typeof(T), true)
                    : (T)Activator.CreateInstance(typeof(T), String.Format(message, messageArguments));

                throw exception;
            }
        }
    }
}
