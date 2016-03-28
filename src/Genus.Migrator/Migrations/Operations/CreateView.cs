using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Migrator.Migrations.Operations
{
    public class CreateView:ViewOperation
    {
        public override int OrderHint
            => 9000;
    }
}
