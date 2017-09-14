using System;
using System.Linq.Expressions;
using System.Diagnostics.CodeAnalysis;

namespace FrodLib.Utils
{

    public static class PropertyNameHelper
    {
        public static string GetPropertyName<T>(Expression<Func<T>> property)
        {
            MemberExpression memExp = property.Body as MemberExpression;
            if(memExp != null)
            {
                return memExp.Member.Name;
            }
            return null;
        }

        public static string GetPropertyNameFrom<T>(Expression<Func<T, object>> exp)
        {
            var unaryExp = exp.Body as UnaryExpression;
            if (unaryExp != null)
            {
                var memExp = unaryExp.Operand as MemberExpression;
                if (memExp != null)
                {
                    return memExp.Member.Name;
                }
            }
            else
            {
                var memExp = exp.Body as MemberExpression;
                if (memExp != null)
                {
                    return memExp.Member.Name;
                }
            }
            return string.Empty;
        }
    }
}
