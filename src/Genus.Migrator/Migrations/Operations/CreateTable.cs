using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genus.Migrator.Migrations.Operations
{
    public sealed class CreateTable:TableOperation
    {
        public IEnumerable<AddField> Fields { get; set; }
        public AddPrimaryKey PrimaryKey { get; set; }
    }
}
