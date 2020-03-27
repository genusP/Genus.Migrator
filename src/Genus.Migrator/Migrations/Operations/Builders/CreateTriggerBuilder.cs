using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Migrator.Migrations.Operations.Builders
{
    public class CreateTriggerBuilder:WithScriptBuilder<CreateTriggerBuilder, CreateTrigger>
    {
        public CreateTriggerBuilder(CreateTrigger operation)
            :base(operation)
        {

        }
    }
}
