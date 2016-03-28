using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genus.Migrator.Migrations.Operations
{
    public sealed class AddField:TableOperation
    {
        public string Name { get; set; }
        public DbType Type { get; set; }
        public int? Length { get; set; }
        public bool IsNullable { get; set; }
        public bool IsIdentity { get; set; }

        public override int OrderHint
            => 7000;
    }
}
