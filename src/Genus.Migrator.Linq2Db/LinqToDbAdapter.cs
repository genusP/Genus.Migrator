using Genus.Migrator.Migrations.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Data;
using Microsoft.Extensions.DependencyInjection;
using Genus.Migrator.Migrations;

namespace Genus.Migrator.Linq2Db
{
    public abstract class LinqToDbAdapter : OrmAdapterBase
    {
        private readonly Lazy<Model.ProviderName> _currentProvider;
        private readonly Lazy<DataConnection> _dataContext;

        public LinqToDbAdapter()
        {
            _dataContext = new Lazy<DataConnection>(GetDataContext);
            _currentProvider = new Lazy<Model.ProviderName>(
                () =>ConvertLinq2DbProviderName(DataContext.DataProvider.Name));
        }

        public DataConnection DataContext
            => _dataContext.Value;

        public override Model.ProviderName CurrentProvider
            =>_currentProvider.Value;

        public override int ExecuteSql(string sqlText)
        {            
            return DataContext.Execute(sqlText);
        }

        protected override void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IMigrationLog, MigrationLog>();
            base.ConfigureServices(services);
        }

        protected virtual DataConnection GetDataContext()
        {
            return ServiceProvider.GetRequiredService<DataConnection>();
        }

        private Model.ProviderName ConvertLinq2DbProviderName(string name)
        {
            if (name.StartsWith("SqlServer", StringComparison.OrdinalIgnoreCase))
                return Model.ProviderName.MicrosoftSqlServer;
            switch (name)
            {
                case "MySql": return Model.ProviderName.MySql;
                case "PostgreSQL": return Model.ProviderName.PosgreSql;
                case "SQLite": return Model.ProviderName.SQLite;
            }
            throw new InvalidOperationException($"Unknown provider {name}");
        }
    }
}
