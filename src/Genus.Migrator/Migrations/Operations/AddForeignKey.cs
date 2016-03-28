using Genus.Migrator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Migrator.Migrations.Operations
{
    public sealed class AddForeignKey:TableOperation
    {
        public string ForeignKeyName { get; set; }
        public string FieldName { get; set; }
        public string PrincipalSchema { get; set; }
        public string PrincipalTable { get; set; }
        public string PrincipalField { get; set; }
        public ForeignKeyAction? OnDelete { get; set; }
        public ForeignKeyAction? OnUpdate { get; set; }

        public override int OrderHint
            => 12000;
    }
}
