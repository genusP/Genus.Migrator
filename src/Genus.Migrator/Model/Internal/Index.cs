using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Migrator.Model.Internal
{
    internal class Index : IIndex
    {
        public Index(string name, ITable table)
        {
            DbName = name;
            Table = table;
        }

        public string DbName
        {
            get;
        }

        public IEnumerable<IIndexItem> Fields
        {
            get;
            internal set;
        }

        public bool IsUnique
        {
            get;
            internal set;
        }

        public ITable Table
        {
            get;
        }
    }
}
