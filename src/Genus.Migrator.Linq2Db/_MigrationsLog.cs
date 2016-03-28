using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Migrator.Linq2Db
{
    [Table]
    class _MigrationsLog
    {
        [Column(IsPrimaryKey =true, Length =14, CanBeNull =false)]
        public string MigrationId { get; set; }

        [Column]
        public DateTime Applied { get; set; }
    }
}
