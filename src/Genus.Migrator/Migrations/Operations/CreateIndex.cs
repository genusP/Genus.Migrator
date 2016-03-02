using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Migrator.Migrations.Operations
{
    public sealed class CreateIndex:TableOperation
    {
        public IEnumerable<string> Fields { get; set; }
        public string IndexName { get; set; }
        public bool IsUnique { get; set; }
    }
}
