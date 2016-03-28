using Genus.Migrator.Migrations;
using Genus.Migrator.Migrations.Design;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.SchemaProvider;
using LinqToDB.SqlQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Migrator.Linq2Db
{
    public class MigrationLog: IMigrationLog
    {
        private readonly LinqToDbAdapter _ormAdapter;

        public MigrationLog(IOrmAdapter ormAdapter)
        {
            if (ormAdapter == null)
                throw new ArgumentNullException(nameof(ormAdapter));
            _ormAdapter = (LinqToDbAdapter)ormAdapter;
            _appliedMigrations = new Lazy<IEnumerable<string>>(GetAppliedMigrations);
        }

        Lazy<IEnumerable<string>> _appliedMigrations;

        public IEnumerable<string> AppliedMigrations
            => _appliedMigrations.Value;

        private IEnumerable<string> GetAppliedMigrations()
        {
            return IsExist()
                ?from m in _ormAdapter.DataContext.GetTable<_MigrationsLog>()
                 orderby m.Applied
                 select m.MigrationId
                : Enumerable.Empty<string>();
        }

        public string CurrentMigration
        {
            get
            {
                return IsExist() == false
                    ? null
                    : GetCurrentMigrationId();
            }
        }

        private string GetCurrentMigrationId()
        {
            return _ormAdapter.DataContext
                        .GetTable<_MigrationsLog>()
                        .OrderByDescending(_ => _.MigrationId)
                        .Select(_ => _.MigrationId)
                        .FirstOrDefault();
        }

        public string GetCreateSqlScript()
        {
            var query = new LinqToDB.SqlQuery.SelectQuery { QueryType = LinqToDB.SqlQuery.QueryType.CreateTable };
            var sqlTable = new LinqToDB.SqlQuery.SqlTable<_MigrationsLog>(_ormAdapter.DataContext.MappingSchema);
            query.CreateTable.Table = sqlTable;

            return SelectQueryToSql(query);
        }

        private string SelectQueryToSql(LinqToDB.SqlQuery.SelectQuery query)
        {
            var sqlBuilder = _ormAdapter.DataContext.DataProvider.CreateSqlBuilder();
            var sb = new System.Text.StringBuilder();
            sqlBuilder.BuildSql(0, query, sb);

            return sb.ToString();
        }

        public string GetDeleteScript(string migrationName)
        {
            var query = new LinqToDB.SqlQuery.SelectQuery { QueryType = LinqToDB.SqlQuery.QueryType.Delete };
            var sqlTable = new LinqToDB.SqlQuery.SqlTable<_MigrationsLog>(_ormAdapter.DataContext.MappingSchema);
            if (sqlTable.Alias == null)
                sqlTable.Alias = sqlTable.Name;
            query.From.Table(sqlTable);
            var pkField = sqlTable.GetKeys(true).Cast< LinqToDB.SqlQuery.SqlField>().First();
            query.Where.Field(pkField).Equal.Value(migrationName);
            
            return SelectQueryToSql(query);
        }

        public string GetInsertScript(string migrationName)
        {
         
            var query = new SelectQuery { QueryType = QueryType.Insert };
            var sqlTable = new SqlTable<_MigrationsLog>(_ormAdapter.DataContext.MappingSchema);
            query.Insert.Into = sqlTable;
            query.Insert.Items.Add(new SelectQuery.SetExpression(sqlTable.Fields[nameof(_MigrationsLog.MigrationId)], new SqlValue(migrationName)));
            var method = typeof(Sql).GetMethod(nameof(Sql.GetDate));
            Sql.FunctionAttribute getDateAtr =null;
            foreach (var atr in _ormAdapter.DataContext.MappingSchema.GetAttributes<Sql.FunctionAttribute>(method))
            {
                if (atr.Configuration == null)
                    getDateAtr = atr;
                if (atr.Configuration == _ormAdapter.DataContext.DataProvider.Name)
                {
                    getDateAtr = atr;
                    break;
                }
            }
            query.Insert.Items.Add(new SelectQuery.SetExpression(sqlTable.Fields[nameof(_MigrationsLog.Applied)], getDateAtr.GetExpression(method)));

            //foreach (var field in sqlTable.Fields)
            //{
            //    var sqlExp = new SqlValue(field.Value.ColumnDescriptor.GetValue(obj));
            //}
            return SelectQueryToSql(query);
        }

        public bool IsExist()
        {
            var entityDescription = _ormAdapter.DataContext.MappingSchema.GetEntityDescriptor(typeof(_MigrationsLog));
            var schemaProvider = _ormAdapter.DataContext.DataProvider.GetSchemaProvider();
            var tables = schemaProvider.GetSchema(_ormAdapter.DataContext, new GetSchemaOptions { GetTables=true}).Tables;
            return tables.Any(ts =>
                    (ts.IsDefaultSchema == string.IsNullOrWhiteSpace(entityDescription.SchemaName)
                        || ts.SchemaName.Equals(entityDescription.SchemaName, StringComparison.OrdinalIgnoreCase))
                    && ts.TableName.Equals(entityDescription.TableName, StringComparison.OrdinalIgnoreCase));
        }
    }
}
