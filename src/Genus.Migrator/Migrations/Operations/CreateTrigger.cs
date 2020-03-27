using Genus.Migrator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Migrator.Migrations.Operations
{
    public class CreateTrigger : TableOperation
    {
        public override int OrderHint => 12000;

        public string TriggerSchema { get; set; }

        public string TriggerName { get; set; }

        public TriggerType  TriggerType { get; set; }

        public TriggerOperation TriggerOperation { get; set; }
    }
}
