using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Migrator.Migrations.Operations
{
    public sealed class AddPrimaryKey:TableOperation
    {
        public string PKName { get; set; }
        public string[] Fields { get; set; }

        public override int OrderHint
            => 11000;
    }
}
