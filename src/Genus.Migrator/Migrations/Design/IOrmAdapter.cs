using Genus.Migrator.Model;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Genus.Migrator.Migrations.Design
{
    public interface IOrmAdapter
    {
        Assembly StartupAssembly { get; set; }
        string Environment { get; set; }
        IServiceCollection Services { get; set; }
        IServiceProvider ServiceProvider { get; }
        ProviderName CurrentProvider { get; }
        int ExecuteSql(string sqlText);
    }
}
