using Genus.Migrator.Migrations.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

namespace Genus.Migrator.Migrations
{
    public sealed class MigrationsSource : IMigrationsSource
    {
        public LinkedList<KeyValuePair<string, TypeInfo>> GetMigrations(Assembly asm)
        {
            var migrationTypes = from t in asm.DefinedTypes
                                 where t.IsAbstract == false
                                         && t.IsGenericTypeDefinition == false
                                         && t.IsSubclassOf(typeof(Migration))
                                 let id = t.Name
                                 orderby t.Name
                                 select new KeyValuePair<string, TypeInfo>(t.Name, t );
           return new LinkedList<KeyValuePair<string, TypeInfo>>(migrationTypes);

        }

        public Snapshot GetModelSnapshot(Assembly asm)
        {
            var snapshots = from t in asm.DefinedTypes
                            where t.IsAbstract == false
                                  && t.IsGenericTypeDefinition == false
                                  && t.IsSubclassOf(typeof(Snapshot))
                            select (Snapshot)Activator.CreateInstance(t.AsType());
            return snapshots.SingleOrDefault();
        }

        public Migration CreateMigration(TypeInfo migrationType)
        {
            var migration = (Migration)Activator.CreateInstance(migrationType.AsType());
            return migration;
        }
    }
}
