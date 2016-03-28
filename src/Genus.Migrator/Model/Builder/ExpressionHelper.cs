using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Genus.Migrator.Model.Builder
{
    static class ExpressionHelper
    {
        public static string GetPropertyName<T, TRet>(Expression<Func<T,TRet>> expression)
        {
            var body = expression.Body;
            if (body.NodeType == ExpressionType.Convert)
                body = ((UnaryExpression)body).Operand;
            if (body.NodeType == ExpressionType.MemberAccess)
            {
                var exp = (MemberExpression)body;
                var pi = exp.Member as PropertyInfo;
                if (pi != null && exp.Expression.Type == typeof(T))
                    return pi.Name;
            }
            throw new NotSupportedException("Need propery of entity");
        }
    }
}
