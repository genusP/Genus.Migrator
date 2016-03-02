using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Migrator.Migrations.Operations
{
    public class ExecuteSql:MigrationOperation
    {
        public string Sql { get; set; }
    }
}
