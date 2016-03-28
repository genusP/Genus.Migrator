using Genus.Migrator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Migrator.Migrations.Design.Internal
{
    public class IndexItemEqualityComparer : IEqualityComparer<IIndexItem>
    {
        public bool Equals(IIndexItem x, IIndexItem y)
        {
            return x.Field.DbName == y.Field.DbName && x.Decending == y.Decending;
        }

        public int GetHashCode(IIndexItem obj)
        {
            var forHash =obj.Field.DbName + (obj.Decending?"D":"A");
            return forHash.GetHashCode();
        }
    }
}
