using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Migrator.Migrations.Operations
{
    public abstract class TableOperation: MigrationOperation
    {
        public string TableName { get; set; }
        public string Schema { get; set; }
    }
}
