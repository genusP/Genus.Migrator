using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genus.Migrator.Model.Internal
{
    [System.Diagnostics.DebuggerDisplay("DbName = {DbName}")]
    internal class PrimaryKey : IPrimaryKey
    {
        public string DbName
        {
            get;
            set;
        }

        public IEnumerable<IField> Items
        {
            get;
            set;
        }
    }
}
