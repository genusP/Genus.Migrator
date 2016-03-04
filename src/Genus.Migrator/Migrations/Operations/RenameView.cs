using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Migrator.Migrations.Operations
{
    public class RenameView : MigrationOperation
    {
        public string NewSchema { get; internal set; }
        public string NewViewName { get; internal set; }
        public string Schema { get; internal set; }
        public string ViewName { get; set; }
    }
}
