using Genus.Migrator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Migrator.Migrations.Operations
{
    public class ExecuteSql:MigrationOperation
    {
        public string Sql { get; set; }
        public ProviderName Provider { get; set; }

        public override int OrderHint
            => 1;
    }
}
