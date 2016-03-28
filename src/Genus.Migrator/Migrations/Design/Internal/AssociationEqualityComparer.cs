using Genus.Migrator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Migrator.Migrations.Design.Internal
{
    public class AssociationEqualityComparer : IEqualityComparer<IAssociation>
    {
        public bool Equals(IAssociation x, IAssociation y)
        {
            return x.Field.ClrName == y.Field.ClrName
                && x.OnDeleteAction == y.OnDeleteAction
                && x.OnUpdateAction == y.OnUpdateAction
                && x.ReferenceField.ClrName == y.ReferenceField.ClrName;
        }

        public int GetHashCode(IAssociation obj)
        {
            return obj.GetHashCode();
        }
    }
}
