using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Migrator.Migrations.Operations
{
    public sealed class DropPrimaryKey:TableOperation
    {
        public string PrimaryKeyName { get; set; }

        public override int OrderHint
            => 1000;
    }
}
