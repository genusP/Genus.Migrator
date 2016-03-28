using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Migrator.Migrations.Operations
{
    public sealed class DropTable:TableOperation
    {
        public override int OrderHint
            => 3000;
    }
}
