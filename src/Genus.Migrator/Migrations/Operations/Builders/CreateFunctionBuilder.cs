using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Migrator.Migrations.Operations.Builders
{
    public class CreateFunctionBuilder:WithScriptBuilder<CreateFunctionBuilder, CreateFunction>
    {
        public CreateFunctionBuilder(CreateFunction operation)
            :base(operation)
        {

        }
    }
}
