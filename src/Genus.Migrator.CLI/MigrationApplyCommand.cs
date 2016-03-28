using Genus.Migrator.Migrations.Design;
using Genus.Migrator.Model;
using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Migrator.CLI
{
    public class MigrationApplyCommand
    {
        public static void Configure(CommandLineApplication cla)
        {
            cla.Description = "Update database to specified migration";

            var migration = cla.Argument(
                 "[migration]",
                 "The target migration. If '0', all migrations will be reverted. If omitted, all pending migrations will be applied");
            var adapterName = cla.ormAdapterClassOption();
            var scriptOnly = cla.Option(
                "-s|--script <provider>",
                "Generate a SQL script from migrations", CommandOptionType.SingleValue
                );

            cla.OnExecute(() => Execute( migration.Value??"last", adapterName.Value(), scriptOnly.Value(), null));

        }

        private static int Execute(string migrationName, string adapterClassName, string scriptForProvider, string environment)
        {
            var adapter = OrmAdapterFactory.Create(adapterClassName, environment);

            ProviderName pn;
            bool scriptOnly = false;
            if (string.IsNullOrWhiteSpace(scriptForProvider))
                pn = adapter.CurrentProvider;
            else {
                scriptOnly = true;
                if (!Enum.TryParse(scriptForProvider, true, out pn))
                    throw new InvalidOperationException("Unknown provider");
            }
            var cmd = new MigrationCommands(adapter);
            var script = cmd.ApplyMigration(migrationName, pn, scriptOnly);
            Console.WriteLine(script);
            return 0;
        }
    }
}
