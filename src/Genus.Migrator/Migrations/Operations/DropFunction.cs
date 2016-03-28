using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Migrator.Migrations.Operations
{
    public class DropFunction : MigrationOperation
    {
        public string FunctionName { get; internal set; }

        public override int OrderHint
            => 1000;

        public string Schema { get; internal set; }
    }
}
