using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;

namespace FrodLib.Extensions
{
    public static class ObjectExtension
    {
        public static bool IsIn<T>(this T source, params T[] list)
        {
            return list.Contains(source);
        }

        public static bool IsNull<T>(this T obj) where T : class
        {
            return obj == null;
        }

        /// <summary>
        /// Throws if the value is null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value of the null check</param>
        /// <param name="exception">Defaults to ArgumentNullException if no exception type is specified</param>
        [Obsolete("Please use ArgumentValidator.IsNotNull instead")]
        public static void ThrowIfNull<T>(this T value, Type exception = null) where T : class
        {
            if (value == null)
            {
                var types = new Type[2];
                types[0] = typeof(string);
                types[1] = typeof(Exception);

                if (exception == null)
                {
                    exception = typeof(ArgumentNullException);
                }

                var constructorInfo = exception.GetTypeInfo().DeclaredConstructors.FirstOrDefault(c =>
                {
                    var constructorParameters = c.GetParameters();
                    if (constructorParameters.Length == types.Length)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            if (!types[i].Equals(constructorParameters[i].ParameterType)) return false;
                        }
                        return true;
                    }
                    return false;
                });
                var createdActivator = GetActivator<Exception>(constructorInfo);

                var instance = createdActivator(typeof(T).Name + " is null", null);
                throw instance;
            }
        }

        /// <summary>
        /// Throws if the value is null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value of the null check</param>
        /// <param name="exception">Defaults to ArgumentNullException if no exception type is specified</param>
        [Obsolete("Please use ArgumentValidator.IsNotNull instead")]
        public static void ThrowIfNull<T>(this T value, string message, Type exception = null) where T : class
        {
            if (value == null)
            {
                var types = new Type[2];
                types[0] = typeof(string);
                types[1] = typeof(Exception);

                if (exception == null)
                {
                    exception = typeof(ArgumentNullException);
                }

                var constructorInfo = exception.GetTypeInfo().DeclaredConstructors.FirstOrDefault(c =>
                {
                    var constructorParameters = c.GetParameters();
                    if (constructorParameters.Length == types.Length)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            if (!types[i].Equals(constructorParameters[i].ParameterType)) return false;
                        }
                        return false;
                    }
                    return false;
                });
                var createdActivator = GetActivator<Exception>(constructorInfo);

                var instance = createdActivator(message, null);
                throw instance;
            }
        }

        private delegate T ObjectActivator<T>(params object[] args);

        private static ObjectActivator<T> GetActivator<T>(ConstructorInfo ctor)
        {
            Type type = ctor.DeclaringType;
            ParameterInfo[] paramsInfo = ctor.GetParameters();

            //create a single param of type object[]
            ParameterExpression param = Expression.Parameter(typeof(object[]), "args");

            var argsExp = new Expression[paramsInfo.Length];

            //pick each arg from the params array 
            //and create a typed expression of them
            for (int i = 0; i < paramsInfo.Length; i++)
            {
                Expression index = Expression.Constant(i);
                Type paramType = paramsInfo[i].ParameterType;
                Expression paramAccessorExp = Expression.ArrayIndex(param, index);
                Expression paramCastExp = Expression.Convert(paramAccessorExp, paramType);
                argsExp[i] = paramCastExp;
            }

            //make a NewExpression that calls the
            //ctor with the args we just created
            var newExp = Expression.New(ctor, argsExp);

            //create a lambda with the New
            //Expression as body and our param object[] as arg
            var lambda = Expression.Lambda(typeof(ObjectActivator<T>), newExp, param);

            //compile it
            var compiled = (ObjectActivator<T>)lambda.Compile();
            return compiled;
        }

        public static void TryDisposeObject(this object objToDispose)
        {
            if (objToDispose == null) return;
            var disposable = objToDispose as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
    }
}
