using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Migrator.Migrations.Operations
{
    public class DropView : MigrationOperation
    {
        public string Schema { get; internal set; }
        public string ViewName { get; internal set; }

        public override int OrderHint
            => 1000;
    }
}
