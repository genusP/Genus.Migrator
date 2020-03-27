using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Migrator.Migrations.Operations
{
    public class AlterTrigger : CreateTrigger
    {
        public override int OrderHint => 10000;
    }
}
