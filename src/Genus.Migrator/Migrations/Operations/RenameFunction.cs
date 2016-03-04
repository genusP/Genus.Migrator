using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Migrator.Migrations.Operations
{
    public class RenameFunction : MigrationOperation
    {
        public string FunctionName { get; internal set; }
        public string NewFunctionName { get; internal set; }
        public string NewSchema { get; internal set; }
        public string Schema { get; internal set; }
    }
}
