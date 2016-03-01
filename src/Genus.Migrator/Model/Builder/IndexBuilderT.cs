using Genus.Migrator.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Genus.Migrator.Model.Builder
{
    public class IndexBuilder<T>:IndexBuilder
    {
        public IndexBuilder<T> OnColumn<TRet>(Expression<Func<T,TRet>> fieldExp, bool descending=false)
        {
            OnColumn(ExpressionHelper.GetPropertyName(fieldExp), descending);
            return this;
        }
    }
}
