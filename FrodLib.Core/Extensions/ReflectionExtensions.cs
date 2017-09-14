using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;

namespace FrodLib.Extensions
{
    public static class ReflectionExtensions
    {
        public static PropertyInfo[] GetProperties<T>(this T obj)
        {
            return GetProperties(obj, null);
        }

        /// <summary>
        /// Returns a collection of properties that has a specific attribute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="attributeType"></param>
        /// <returns></returns>
        public static PropertyInfo[] GetProperties<T>(this T obj, Type attributeType)
        {
            if(attributeType != null)
            {
                if (obj == null) return new PropertyInfo[0];
                var type = obj.GetType();
                var properties = type.GetRuntimeProperties();
                return properties.Where(p => p.GetCustomAttributes(attributeType, true).Any()).ToArray();
            }
            else
            {
                if (obj == null) return new PropertyInfo[0];
                var type = obj.GetType();
                return type.GetRuntimeProperties().ToArray();
            }
        }

        public static PropertyInfo GetProperty<T>(this T obj, string propertyName)
        {
            if (obj == null) return null;
            var type = obj.GetType();
            return type.GetRuntimeProperty(propertyName);
        }

        public static PropertyInfo GetProperty<T>(this T obj, Expression<Func<T>> property)
        {
            var memExp = property.Body as MemberExpression;
            if(memExp != null)
            {
                return GetProperty(obj, memExp.Member.Name);
            }
            return null;
        }
    }
}
