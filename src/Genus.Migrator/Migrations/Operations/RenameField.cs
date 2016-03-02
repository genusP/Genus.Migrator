using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Migrator.Migrations.Operations
{
    public class RenameField:TableOperation
    {
        public string FieldName { get; set; }
        public string NewFieldName { get; set; }
    }
}
