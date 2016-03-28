using System;
using Microsoft.Extensions.CommandLineUtils;

namespace Genus.Migrator.CLI
{
    public class MigrationVersionCommand
    {
        public static void Configure(CommandLineApplication obj)
        {
            obj.Description = "Show current applyed migration ID";
        }
    }
}