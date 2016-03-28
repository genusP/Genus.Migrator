using Genus.Migrator.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Genus.Migrator.Model.Builder
{
    public class TableBuilder<T>:TableBuilder
    {
        public FieldBuilder Field<TRet>(Expression<Func<T, TRet>> fieldExp, int length=-1)
        {
            var res = Field(ExpressionHelper.GetPropertyName(fieldExp));
            if(ClrTypeConverter.IsConvertable(typeof(TRet)))
                res.HasType(ClrTypeConverter.ConvertClrTypeToDbType(typeof(TRet)), length);
            return res;
        }

        public new IndexBuilder<T> Index(string name)
        {
            var res = IndexImplement(name, () => new IndexBuilder<T>()) as IndexBuilder<T>;
            if (res == null)
                throw new InvalidOperationException("Index builder not typed");
            return res;
        }

        public PrimaryKeyBuilder HasKey<TRet>(params Expression<Func<T, TRet>>[] fieldExp)
        {
            var names = fieldExp.Select(_ => ExpressionHelper.GetPropertyName(_)).ToArray();
            return base.HasKey(names);
        }

        public AssociationBuilder<T, TPricipal> Association<TPricipal>(
            Expression<Func<T,object>> foreignKey, 
            Expression<Func<TPricipal, object>> principalKey)
        {
            var strForeignKey = ExpressionHelper.GetPropertyName(foreignKey);
            var strPrincipalTable = typeof(TPricipal).FullName;
            var strPrincipalKey = principalKey != null
                ? ExpressionHelper.GetPropertyName(principalKey)
                : null;

            var res = AssociationImplement(
                        strForeignKey, 
                        strPrincipalTable, 
                        strPrincipalKey,
                        () => new AssociationBuilder<T, TPricipal>()
                    ) as AssociationBuilder<T, TPricipal>;
            if (res == null)
                throw new InvalidOperationException("Association not typed");
            return res;
        }
    }
}
