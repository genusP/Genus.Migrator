using System;
using System.Linq;
using Microsoft.Extensions.CommandLineUtils;
using Genus.Migrator.Migrations.Design;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Genus.Migrator.Migrations;
using Microsoft.Extensions.PlatformAbstractions;

namespace Genus.Migrator.CLI
{
    public class MigrationAddCommand
    {
        public static void Configure(CommandLineApplication cla)
        {
            cla.Description = "Add new migration";

            cla.HelpOption("-h|--help");
            var outputDir = cla.Option("-o <path>","", CommandOptionType.SingleValue);
            var skipEmpty = cla.Option("-e|--skip-empty", "Skip empty migrations", CommandOptionType.NoValue);
            var adapterName = cla.ormAdapterClassOption();


            cla.OnExecute(() =>Execute(outputDir.Value(), skipEmpty.HasValue(), adapterName.Value(), null));
        }

        private static int Execute( string outDir, bool skipEmpty, string adapterClassName, string environment)
        {
            var adapter = OrmAdapterFactory.Create(adapterClassName, environment);
            var cmd = new MigrationCommands(adapter);
            var model = cmd.ServiceProvider.GetRequiredService<IModelSource>().Model;
            var snapshot = cmd.ServiceProvider.GetRequiredService<IMigrationsSource>().GetModelSnapshot(adapter.StartupAssembly);
            var currentModel = snapshot?.Model;
            cmd.AddMigration(
                PlatformServices.Default.Application.ApplicationName,
                currentModel,
                model,
                outDir ?? System.IO.Directory.GetCurrentDirectory(),
                skipEmpty
                );
            return 0;
        }

        
    }
}