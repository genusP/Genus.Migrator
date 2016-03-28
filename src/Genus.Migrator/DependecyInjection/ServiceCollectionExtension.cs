using Genus.Migrator.Migrations;
using Genus.Migrator.Migrations.Design;
using Genus.Migrator.Migrations.Design.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Migrator.DependecyInjection
{
    public static class ServiceCollectionExtension
    {
        public static void AddMigrator(this IServiceCollection services)
        {
            services.TryAddTransient<IMigrationsSource, MigrationsSource>();
            services.TryAddTransient<IModelComparer, ModelComparer>();
            services.TryAddTransient<ICodeGeneratorFactory, CSharpCodeGeneratorFactory>();
            services.Configure<MigratorOptions>(o=>
            {
                o.SqlGenerators.Add(Model.ProviderName.MicrosoftSqlServer, new Lazy<SqlGenerator>(() => new MsSqlServerSqlGenerator()));
            });
        }
    }
}
