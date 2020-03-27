using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Migrator.Migrations.Operations
{
    public class DropTrigger : MigrationOperation
    {
        public override int OrderHint => 1000;

        public string TriggerName { get; set; }
        public string TriggerSchema { get; set; }
    }
}
