using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Migrator.Migrations.Operations
{
    public class RenameIndex:TableOperation
    {
        public string IndexName { get; set; }
        public string NewIndexName { get; set; }

        public override int OrderHint
            => 8000;
    }
}
