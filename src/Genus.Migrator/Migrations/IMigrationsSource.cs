using System;
using System.Collections.Generic;
using System.Reflection;

namespace Genus.Migrator.Migrations
{
    public interface IMigrationsSource
    {
        LinkedList<KeyValuePair<string, TypeInfo>> GetMigrations(Assembly assembly);
        Migration CreateMigration(TypeInfo migrationType);
        Snapshot GetModelSnapshot(Assembly asm);
    }
}