using Genus.Migrator.Migrations.Design;
using Genus.Migrator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Migrator
{
    public class MigratorOptions
    {
        public IDictionary<ProviderName, Lazy<SqlGenerator>> SqlGenerators { get; } 
            = new Dictionary<ProviderName, Lazy<SqlGenerator>>();
    }
}
