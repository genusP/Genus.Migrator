using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Migrator.Migrations.Operations
{
    public class CreateFunction : MigrationOperation
    {
        public string FunctionName { get; internal set; }
        public string Schema { get; internal set; }

        public override int OrderHint
            => 9000;
    }
}
