using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Migrator.CLI
{
    public static class ComandLineApplicationExtensions
    {
        public static CommandOption ormAdapterClassOption(this CommandLineApplication cla)
        {
            return cla.Option("-a <ormAdapterClassName>", "", CommandOptionType.SingleValue);
        }
    }
}
