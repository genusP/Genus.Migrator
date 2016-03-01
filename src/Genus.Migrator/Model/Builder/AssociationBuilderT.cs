using Genus.Migrator.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Genus.Migrator.Model.Builder
{
    public class AssociationBuilder<T, TPrincipal>:AssociationBuilder
    {
        public AssociationBuilder<T, TPrincipal> WithNavigation(
            Expression<Func<T, TPrincipal>> dependNavigation, 
            Expression<Func<TPrincipal, T>> principalNavigation = null
            )
        {

            var foreignKey = ExpressionHelper.GetPropertyName(dependNavigation);
            string principalKey = null;
            if (principalNavigation != null)
                principalKey = ExpressionHelper.GetPropertyName(principalNavigation);
            base.WithNavigation(foreignKey, principalKey);
            return this;
        }

        public AssociationBuilder<T, TPrincipal> WithNavigation(
            Expression<Func<T, TPrincipal>> dependNavigation, 
            Expression<Func<TPrincipal, IEnumerable<T>>> principalNavigation
            )
        {
            var foreignKey = ExpressionHelper.GetPropertyName(dependNavigation);
            string principalKey = null;
            if (principalNavigation == null)
                principalKey = ExpressionHelper.GetPropertyName(principalNavigation);
            base.WithNavigation(foreignKey, principalKey);
            return this;
        }
    }
}
