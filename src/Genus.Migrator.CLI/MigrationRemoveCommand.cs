using System;
using Microsoft.Extensions.CommandLineUtils;

namespace Genus.Migrator.CLI
{
    public class MigrationRemoveCommand
    {
        public static void Configure(CommandLineApplication cla)
        {
            cla.Description = "Remove last migration";
        }
    }
}