using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Migrator.Migrations.Operations
{
    public class RenameTable:TableOperation
    {
        public string NewTableName { get; set; }
        public string NewSchema { get; set; }
    }
}
