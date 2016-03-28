using System.Collections.Generic;

namespace Genus.Migrator.Migrations
{
    public interface IMigrationLog
    {
        IEnumerable<string> AppliedMigrations { get; }

        string GetCreateSqlScript();

        string GetDeleteScript(string migrationName);

        string GetInsertScript(string migrationName);

        bool IsExist();
    }
}