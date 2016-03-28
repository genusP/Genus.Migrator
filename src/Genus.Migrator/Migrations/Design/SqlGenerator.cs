using Genus.Migrator.Migrations.Design;
using Genus.Migrator.Migrations.Operations;
using Genus.Migrator.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Migrator.Migrations.Design
{
    public abstract class SqlGenerator
    {
        private readonly IDictionary<Type, Func<MigrationOperation, string>> _operationGenerators;

        protected readonly IndentedStringBuilder builder = new IndentedStringBuilder();

        public SqlGenerator()
        {
            _operationGenerators = GetOperationGenerators();
        }

        private IDictionary<Type, Func<MigrationOperation, string>> GetOperationGenerators()
        {
            return new Dictionary<Type, Func<MigrationOperation, string>>
            {
                {typeof(AddField), o=>Generate((AddField)o) },
                {typeof(AddForeignKey), o=>Generate((AddForeignKey)o) },
                {typeof(AddPrimaryKey), o=>Generate((AddPrimaryKey)o) },
                {typeof(AlterView), o=>Generate((AlterView)o) },
                {typeof(CreateFunction), o=>Generate((CreateFunction)o) },
                {typeof(CreateIndex), o=>Generate((CreateIndex)o) },
                {typeof(CreateTable), o=>Generate((CreateTable)o) },
                {typeof(CreateView), o=>Generate((CreateView)o) },
                {typeof(DropField), o=>Generate((DropField)o) },
                {typeof(DropForeignKey), o=>Generate((DropForeignKey)o) },
                {typeof(DropFunction), o=>Generate((DropFunction)o) },
                {typeof(DropIndex), o=>Generate((DropIndex)o) },
                {typeof(DropPrimaryKey), o=>Generate((DropPrimaryKey)o) },
                {typeof(DropSchema), o=>Generate((DropSchema)o) },
                {typeof(DropTable), o=>Generate((DropTable)o) },
                {typeof(DropView), o=>Generate((DropView)o) },
                {typeof(EnsureSchema), o=>Generate((EnsureSchema)o) },
                {typeof(ExecuteSql), o=>Generate((ExecuteSql)o) },
                {typeof(RenameField), o=>Generate((RenameField)o) },
                {typeof(RenameFunction), o=>Generate((RenameFunction)o) },
                {typeof(RenameIndex), o=>Generate((RenameIndex)o) },
                {typeof(RenameTable), o=>Generate((RenameTable)o) },
                {typeof(RenameView), o=>Generate((RenameView)o) }
            };
        }

        protected abstract char BeginQuote { get; }

        protected abstract char EndQuote { get; }

        public abstract string BatchTerminator { get; }

        public abstract ProviderName ProviderName { get; }

        public IEnumerable<string> Generate(IEnumerable<MigrationOperation> operations)
        {
            foreach (var operation in operations)
            {
                var res = Generate(operation);
                if (res != null)
                    yield return res;
            }
        }

        protected virtual string Quote(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;
            if (value.StartsWith(BeginQuote.ToString())
                && value.EndsWith(EndQuote.ToString()))
                return value;
            return BeginQuote + value + EndQuote;
        }

        protected virtual string GetType(DbType dbType, int length)
        {
            switch (dbType)
            {
                case DbType.StringFixedLength:
                case DbType.AnsiStringFixedLength: return $"char({length})";
                case DbType.String:
                case DbType.AnsiString: return $"varchar({(length==-1?50:length)})";
                case DbType.Binary:     return $"binary({length})";
                case DbType.Byte:       return "tinyint";
                case DbType.Boolean:    return "bit";
                case DbType.Date:       return "date";
                case DbType.DateTime2:
                case DbType.DateTime:   return "datetime";
                case DbType.Decimal:    return "money";
                case DbType.Double:     return "float";
                case DbType.Guid:       return "uniqueidentifier";
                case DbType.UInt16: 
                case DbType.Int16:      return "shortint";
                case DbType.UInt32:
                case DbType.Int32:      return "int";
                case DbType.UInt64:
                case DbType.Int64:      return "bigint";
                case DbType.Time:       return "time";
                default:
                    throw new NotSupportedException($"Not supported DbType: {dbType}");
            }
        }

        protected virtual string Generate(MigrationOperation operation)
        {
            Func<MigrationOperation, string> action;
            if(this._operationGenerators.TryGetValue(operation.GetType(), out action))
            {
                return action(operation);
            }
            throw new InvalidOperationException($"Unknown operation: {operation.GetType().FullName}");
        }

        protected virtual string Generate(AddField operation)
        {
            builder.Clear();
            builder.Append("ALTER TABLE");
            FullTableName(operation.Schema, operation.TableName, builder);
            builder.Append(" ADD ");
            AddColumnDefenititon(operation, builder);
            return builder.ToString();
        }

        private void FullTableName(string schema, string table, IndentedStringBuilder builder)
        {
            if (!string.IsNullOrWhiteSpace(schema))
            {
                builder.Append(Quote(schema));
                builder.Append(".");
            }
            builder.Append(Quote(table));
        }

        protected virtual void AddColumnDefenititon(AddField operation, IndentedStringBuilder builder)
        {
            builder.Append(Quote(operation.Name))
                .Append(" ")
                .Append(GetType(operation.Type, operation.Length ?? -1))
                .Append(" ");
            if (!operation.IsNullable)
                builder.Append("NOT ");
            builder.Append("NULL");
            if (operation.IsIdentity)
                builder.Append(" IDENTITY");
        }

        protected virtual string Generate(AddForeignKey operation)
        {
            builder.Clear();
            builder.Append("ALTER TABLE");
            FullTableName(operation.Schema, operation.TableName, builder);
            builder.Append(" ADD CONSTRAINT ")
                .Append(Quote(operation.ForeignKeyName))
                .Append(" FOREIGN KEY (")
                .Append(Quote(operation.FieldName))
                .Append(") REFERENCES ");
            FullTableName(operation.PrincipalSchema, operation.PrincipalTable, builder);
            builder.Append("(")
                .Append(Quote(operation.PrincipalField))
                .Append(")");
            if (operation.OnUpdate.HasValue)
                builder.Append(" ON UPDATE ")
                    .Append(operation.OnUpdate.Value.ToString().Replace('_',' '));
            if (operation.OnDelete.HasValue)
                builder.Append(" ON DELETE ")
                    .Append(operation.OnDelete.Value.ToString().Replace('_', ' '));
            return builder.ToString();
        }

        protected virtual string Generate(AddPrimaryKey operation)
        {
            builder.Clear();
            builder.Append("ALTER TABLE");
            FullTableName(operation.Schema, operation.TableName, builder);
            builder.Append(" ADD CONSTRAINT ")
                .Append(Quote(operation.PKName))
                .Append(" PRIMARY KEY (")
                .Append(string.Join(", ", operation.Fields.Select(f=>Quote(f))))
                .Append(")");
            return builder.ToString();
        }

        protected virtual string Generate(AlterView operation)
        {
            builder.Clear();
            builder.Append("ALTER VIEW");
            FullTableName(operation.Schema, operation.ViewName, builder);
            builder.Append(" AS ");
            using (builder.Indenter())
            {
                string sql = GetAnotation(operation, "sql");
                if (sql != null)
                    builder.AppendNewLine(sql);
                else
                    throw new InvalidOperationException($"Script for view {operation.Schema}.{operation.ViewName} and provider '{ProviderName}' not found.");
            }
            return builder.ToString();
        }

        protected virtual string Generate(CreateFunction operation)
        {
            builder.Clear();
            string sql = GetAnotation(operation, "sql");
            if (sql != null)
                builder.AppendNewLine(sql);
            else
                throw new InvalidOperationException($"Script for function {operation.Schema}.{operation.FunctionName} and provider '{ProviderName}' not found.");
            return builder.ToString();
        }

        protected virtual string Generate(CreateIndex operation)
        {
            builder.Clear();
            builder.Append("CREATE");
            if (operation.IsUnique)
                builder.Append(" UNIQUE");
            builder.Append(" INDEX ")
                .Append(Quote(operation.IndexName))
                .Append(" ON ");
            FullTableName(operation.Schema, operation.TableName, builder);
            builder.Append("(")
                //TODO: Необходимы изменения для поддержки учета направления сортировки столбцов
                .Append(string.Join(", ", operation.Fields.Select(f => Quote(f))))
                .Append(")");
            return builder.ToString();
        }

        protected virtual string Generate(CreateTable operation)
        {
            builder.Clear();
            builder.Append("CREATE TABLE ");
            FullTableName(operation.Schema, operation.TableName, builder);
            builder.AppendNewLine("(");
            using (builder.Indenter())
            {
                var first = true;
                foreach (var field in operation.Fields)
                {
                    if (first)
                        first = false;
                    else
                        builder.AppendLine(",");
                    AddColumnDefenititon(field, builder);
                }
                if(operation.PrimaryKey!=null)
                {
                    builder.AppendLine(",");
                    builder.Append("CONSTRAINT ")
                        .Append(Quote(operation.PrimaryKey.PKName))
                        .Append(" PRIMARY KEY (")
                        .Append(string.Join(", ", operation.PrimaryKey.Fields.Select(f => Quote(f))))
                        .Append(")");
                }
            }
            builder.AppendNewLine(")");
            string with = GetAnotation(operation, "with");
            if (with!=null)
                builder.AppendNewLine("WITH ").Append(with);

            return builder.ToString();
        }

        protected string GetAnotation(MigrationOperation operation, string key)
        {
            string value;
            if (operation.Annotations.TryGetValue($"{ProviderName}:{key}", out value))
                return value;
            else if (operation.Annotations.TryGetValue(key, out value))
                return value;
            return null;
        }

        protected virtual string Generate(CreateView operation)
        {
            builder.Clear();
            builder.Append("CREATE VIEW ");
            FullTableName(operation.Schema, operation.ViewName, builder);
            builder.Append(" AS ");
            using (builder.Indenter())
            {
                string sql = GetAnotation(operation, "sql");
                if (sql != null)
                    builder.AppendNewLine(sql);
                else
                    throw new InvalidOperationException($"Script for view {operation.Schema}.{operation.ViewName} and provider '{ProviderName}' not found.");
            }
            return builder.ToString();
        }

        protected virtual string Generate(DropField operation)
        {
            builder.Clear();
            builder.Append("ALTER TABLE ");
            FullTableName(operation.Schema, operation.TableName, builder);
            builder.Append(" DROP COLUMN ")
                .Append(Quote(operation.FieldName));
            
            return builder.ToString();
        }

        protected virtual string Generate(DropForeignKey operation)
        {
            builder.Clear();
            builder.Append("ALTER TABLE ");
            FullTableName(operation.Schema, operation.TableName, builder);
            builder.Append(" DROP CONSTARINS ")
                .Append(Quote(operation.ForeignKeyName));

            return builder.ToString();
        }

        protected virtual string Generate(DropFunction operation)
        {
            builder.Clear();
            builder.Append("DROP FUNCTION ");
            FullTableName(operation.Schema, operation.FunctionName, builder);

            return builder.ToString();
        }

        protected abstract string Generate(DropIndex operation);

        protected virtual string Generate(DropPrimaryKey operation)
        {
            builder.Clear();
            builder.Append("ALTER TABLE ");
            FullTableName(operation.Schema, operation.TableName, builder);
            builder.Append(" DROP CONSTARINS ")
                .Append(Quote(operation.PrimaryKeyName));

            return builder.ToString();
        }

        protected virtual string Generate(DropSchema operation)
        {
            builder.Clear();
            builder.Append("DROP SCHEMA ")
                .Append(Quote(operation.Schema));

            return builder.ToString();
        }

        protected virtual string Generate(DropTable operation)
        {
            builder.Clear();
            builder.Append("DROP TABLE ");
            FullTableName(operation.Schema, operation.TableName, builder);

            return builder.ToString();
        }

        protected virtual string Generate(DropView operation)
        {
            builder.Clear();
            builder.Append("DROP VIEW ");
            FullTableName(operation.Schema, operation.ViewName, builder);

            return builder.ToString();
        }

        protected abstract string Generate(EnsureSchema operation);


        protected virtual string Generate(ExecuteSql operation)
        {
            if(ProviderName == operation.Provider)
                return operation.Sql;
            return null;
        }

        protected abstract string Generate(RenameField operation);

        protected abstract string Generate(RenameFunction operation);

        protected abstract string Generate(RenameIndex operation);

        protected abstract string Generate(RenameTable operation);

        protected abstract string Generate(RenameView operation);
    }
}
