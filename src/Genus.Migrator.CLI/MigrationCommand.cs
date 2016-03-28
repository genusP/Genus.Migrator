using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Migrator.CLI
{
    public class MigrationCommand
    {
        public static void Configure(CommandLineApplication cla)
        {
            cla.Description = "Commands for manage your migrations";

            cla.HelpOption("-h|--help");

            cla.Command("apply", MigrationApplyCommand.Configure);
            cla.Command("remove", MigrationRemoveCommand.Configure);
            cla.Command("version", MigrationVersionCommand.Configure);
            cla.Command("add", MigrationAddCommand.Configure);

            cla.OnExecute(() => {
                cla.ShowHelp();
                return 0;
                });
        }
    }
}
