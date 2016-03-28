using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Migrator.Migrations.Operations
{
    public sealed class DropField:TableOperation
    {
        public string FieldName { get; set; }

        public override int OrderHint
            => 2000;
    }
}
