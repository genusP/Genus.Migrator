using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Migrator.Migrations.Operations
{
    public sealed class DropForeignKey:TableOperation
    {
        public string ForeignKeyName { get; set; }
    }
}
