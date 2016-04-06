using Genus.Migrator.Migrations.Operations;
using Genus.Migrator.Migrations.Operations.Builders;
using Genus.Migrator.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Genus.Migrator.Migrations
{
    public class MigrationBuilder
    {
        readonly Lazy<List<MigrationOperation>> _operations
            = new Lazy<List<MigrationOperation>>();

        public IEnumerable<MigrationOperation> Operations
            => _operations.IsValueCreated
                ? _operations.Value
                : Enumerable.Empty<MigrationOperation>();

        public CreateTableBuilder CreateTable(
            string name, 
            Action<FieldsBuilder> fields, 
            Func<CreateTableBuilder,OperationBuilder<AddPrimaryKey>> pk=null, 
            string schema=null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Need value", nameof(name));
            if (fields == null)
                throw new ArgumentNullException(nameof(fields));

            IList<FieldBuilder> fieldBuilders = new List<FieldBuilder>();
            fields(new FieldsBuilder(fieldBuilders));
            var operation = new CreateTable
            {
                Schema = schema,
                TableName = name,
                Fields = fieldBuilders.Select(_=>_.Operation).ToArray()

            };
            var builder = new CreateTableBuilder(operation);
            operation.PrimaryKey = pk?.Invoke(builder)?.Operation;
            _operations.Value.Add(operation);
            return builder;
        }

        public OperationBuilder<DropTable> DropTable(string name, string schema = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Need value", nameof(name));
            var operation = new DropTable
            {
                TableName = name,
                Schema = schema
            };
            _operations.Value.Add(operation);
            return new OperationBuilder<DropTable>(operation);
        }

        public OperationBuilder<CreateIndex> CreateIndex(string name, string table, string[] fields, bool unique =false, string tableSchema=null)
        {//TODO: Необходимы изменения для поддержки учета направления сортировки столбцов
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Need value", nameof(name));
            if (string.IsNullOrWhiteSpace(table))
                throw new ArgumentException("Need value", nameof(table));
            if (fields==null||fields.Length==0)
                throw new ArgumentException("Need value", nameof(fields));
            var operation = new CreateIndex
            {
                TableName = table,
                Fields = fields,
                IndexName = name,
                IsUnique = unique,
                Schema = tableSchema
            };
            _operations.Value.Add(operation);
            return new OperationBuilder<CreateIndex>(operation);
        }

        public FieldBuilder AddField(string table, string name, DbType dbType, int? length = null, bool nullable = true, bool identity = false, string schema=null)
        {
            var operation = new AddField
            {
                TableName = table,
                Name = name,
                Type = dbType,
                Length = length,
                IsNullable = nullable,
                IsIdentity = identity,
                Schema = schema
            };
            _operations.Value.Add(operation);
            return  new FieldBuilder(operation);
        }

        public OperationBuilder<AlterField> AlterField(string table, string name, DbType? dbType=null, int? length = null, bool? nullable = null, bool? identity = null, string schema = null)
        {
            var operation = new AlterField
            {
                TableName = table,
                Name = name,
                Type = dbType,
                Length = length,
                IsNullable = nullable,
                IsIdentity = identity,
                Schema = schema
            };
            _operations.Value.Add(operation);
            return new OperationBuilder<AlterField>(operation);
        }

        public OperationBuilder<AddForeignKey> AddForeignKey(
            string name, 
            string table, 
            string field, 
            string principalTable, 
            string principalField, 
            ForeignKeyAction? onDelete=null,
            ForeignKeyAction? onUpdate=null,
            string schema = null,
            string principalSchema = null
            )
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Need value", nameof(name));
            if (string.IsNullOrWhiteSpace(table))
                throw new ArgumentException("Need value", nameof(table));
            if (string.IsNullOrWhiteSpace(field))
                throw new ArgumentException("Need value", nameof(field));
            if (string.IsNullOrWhiteSpace(principalTable))
                throw new ArgumentException("Need value", nameof(principalTable));
            if (string.IsNullOrWhiteSpace(principalField))
                throw new ArgumentException("Need value", nameof(principalField));

            var operation = new AddForeignKey
            {
                FieldName = field,
                ForeignKeyName = name,
                OnDelete = onDelete,
                OnUpdate = onUpdate,
                PrincipalField = principalField,
                PrincipalSchema = principalSchema,
                PrincipalTable = principalTable,
                Schema = schema,
                TableName = table
            };
            _operations.Value.Add(operation);
            return new OperationBuilder<AddForeignKey>(operation);
        }

        public OperationBuilder<AddPrimaryKey> AddPrimaryKey(string name, string table, string[] fields, string schema = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            if (fields == null || fields.Length == 0)
                throw new ArgumentException("Need fields", nameof(fields));
            if (string.IsNullOrWhiteSpace(table))
                throw new ArgumentException("Need table", nameof(table));
            var operation = new AddPrimaryKey
            {
                PKName = name,
                Fields = fields,
                TableName = table,
                Schema = schema
            };
            _operations.Value.Add(operation);
            return new OperationBuilder<AddPrimaryKey>(operation);
        }

        public OperationBuilder<ExecuteSql> Sql(string sql, ProviderName provider)
        {
            if (string.IsNullOrWhiteSpace(sql))
                throw new ArgumentException("Need value", nameof(sql));
            var operation = new ExecuteSql
            {
                Sql = sql,
                Provider = provider
            };
            _operations.Value.Add(operation);
            return new OperationBuilder<ExecuteSql>(operation);
        }

        public OperationBuilder<DropForeignKey> DropForeignKey(string name, string table, string schema=null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Need value", nameof(name));
            if (string.IsNullOrWhiteSpace(table))
                throw new ArgumentException("Need value", nameof(table));
            var operation = new DropForeignKey
            {
                Schema = schema,
                ForeignKeyName = name,
                TableName = table
            };
            _operations.Value.Add(operation);
            return new OperationBuilder<DropForeignKey>(operation);
        }

        public OperationBuilder<DropIndex> DropIndex(string name, string table, string schema=null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Need value", nameof(name));
            if (string.IsNullOrWhiteSpace(table))
                throw new ArgumentException("Need value", nameof(table));
            var operation = new DropIndex
            {
                IndexName = name,
                Schema = schema,
                TableName = table
            };
            _operations.Value.Add(operation);
            return new OperationBuilder<DropIndex>(operation);
        }

        public OperationBuilder<DropPrimaryKey> DropPrimaryKey(string name, string table, string schema=null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Need value", nameof(name));
            if (string.IsNullOrWhiteSpace(table))
                throw new ArgumentException("Need value", nameof(table));
            var operation = new DropPrimaryKey
            {
                TableName = table,
                Schema = schema,
                PrimaryKeyName = name
            };
            _operations.Value.Add(operation);
            return new OperationBuilder<DropPrimaryKey>(operation);
        }

        public OperationBuilder<DropField> DropField(string field, string table, string schema=null)
        {
            if (string.IsNullOrWhiteSpace(field))
                throw new ArgumentException("Need value", nameof(field));
            if (string.IsNullOrWhiteSpace(table))
                throw new ArgumentException("Need value", nameof(table));
            var operation = new DropField
            {
                FieldName = field,
                Schema = schema,
                TableName = table
            };
            _operations.Value.Add(operation);
            return new OperationBuilder<DropField>(operation);
        }

        public OperationBuilder<DropSchema> DropSchema(string schema)
        {
            if (string.IsNullOrWhiteSpace(schema))
                throw new ArgumentException("Need value", nameof(schema));

            var operation = new DropSchema
            {
                Schema = schema
            };
            _operations.Value.Add(operation);
            return new OperationBuilder<DropSchema>(operation);
        }

        public OperationBuilder<EnsureSchema> EnsureSchema(string schema)
        {
            if (string.IsNullOrWhiteSpace(schema))
                throw new ArgumentException("Need value", nameof(schema));

            var operation = new EnsureSchema
            {
                Schema = schema
            };
            _operations.Value.Add(operation);
            return new OperationBuilder<EnsureSchema>(operation);
        }

        public OperationBuilder<RenameTable> RenameTable(string name, string newName)
            => RenameTable(name, newName, null, null);

        public OperationBuilder<RenameTable> RenameTable(string name, string newName, string schema, string newSchema)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Need value", nameof(name));
            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("Need value", nameof(newName));
            bool isSchema = string.IsNullOrWhiteSpace(schema),
                 isNewSchema = string.IsNullOrWhiteSpace(newSchema);
            if (!isNewSchema && isSchema)
                throw new ArgumentException("Need value", nameof(schema));
            if(isNewSchema && !isSchema)
                throw new ArgumentException("Need value", nameof(newSchema));
            var operation = new RenameTable
            {
                Schema = schema,
                NewSchema = newSchema,
                NewTableName = newName,
                TableName = name
            };
            _operations.Value.Add(operation);
            return new OperationBuilder<RenameTable>(operation);
        }

        public OperationBuilder<RenameField> RenameField(string name, string newName, string table, string schema = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Need value", nameof(name));
            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("Need value", nameof(newName));
            if (string.IsNullOrWhiteSpace(table))
                throw new ArgumentException("Need value", nameof(table));

            var operation = new RenameField
            {
                Schema = schema,
                FieldName = name,
                NewFieldName = newName,
                TableName = table
            };
            _operations.Value.Add(operation);
            return new OperationBuilder<RenameField>(operation);
        }

        public OperationBuilder<RenameIndex> RenameIndex(string name, string newName, string table, string schema = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Need value", nameof(name));
            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("Need value", nameof(newName));
            if (string.IsNullOrWhiteSpace(table))
                throw new ArgumentException("Need value", nameof(table));

            var operation = new RenameIndex
            {
                Schema = schema,
                TableName = table,
                IndexName = name,
                NewIndexName=newName
            };
            _operations.Value.Add(operation);
            return new OperationBuilder<RenameIndex>(operation);
        }

        public ViewBuilder CreateView(string name, string schema = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Need value", nameof(name));

            var operation = new CreateView {
                ViewName=name,
                Schema=schema
            };
            _operations.Value.Add(operation);
            return new ViewBuilder(operation);
        }

        public OperationBuilder<DropView> DropView(string name, string schema = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Need value", nameof(name));

            var operation = new DropView{
                Schema =schema,
                ViewName = name
            };
            _operations.Value.Add(operation);
            return new OperationBuilder<DropView>(operation);
        }

        public OperationBuilder<RenameView> RenameView(string name, string newName)
            => RenameView(name, newName, null, null);

        public OperationBuilder<RenameView> RenameView(string name, string newName, string schema, string newSchema)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Need value", nameof(name));
            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("Need value", nameof(newName));
            bool isSchema = string.IsNullOrWhiteSpace(schema),
                  isNewSchema = string.IsNullOrWhiteSpace(newSchema);
            if (!isNewSchema && isSchema)
                throw new ArgumentException("Need value", nameof(schema));
            if (isNewSchema && !isSchema)
                throw new ArgumentException("Need value", nameof(newSchema));

            var operation = new RenameView {
                Schema = schema,
                ViewName = name,
                NewSchema = newSchema,
                NewViewName = newName
            };
            _operations.Value.Add(operation);
            return new OperationBuilder<RenameView>(operation);
        }

        public CreateFunctionBuilder CreateFunction(string name, string schema = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Need value", nameof(name));

            var operation = new CreateFunction {
                FunctionName = name,
                Schema = schema
            };
            _operations.Value.Add(operation);
            return new CreateFunctionBuilder(operation);
        }

        public OperationBuilder<DropFunction> DropFunction(string name, string schema = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Need value", nameof(name));

            var operation = new DropFunction {
                Schema = schema,
                FunctionName = name
            };
            _operations.Value.Add(operation);
            return new OperationBuilder<DropFunction>(operation);
        }

        public OperationBuilder<RenameFunction> RenameFunction(string name, string newName)
            => RenameFunction(name, newName, null, null);

        public OperationBuilder<RenameFunction> RenameFunction(string name, string newName, string schema, string newSchema)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Need value", nameof(name));
            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("Need value", nameof(newName));
            bool isSchema = string.IsNullOrWhiteSpace(schema),
                 isNewSchema = string.IsNullOrWhiteSpace(newSchema);
            if (!isNewSchema && isSchema)
                throw new ArgumentException("Need value", nameof(schema));
            if (isNewSchema && !isSchema)
                throw new ArgumentException("Need value", nameof(newSchema));

            var operation = new RenameFunction {
                FunctionName = name,
                Schema = schema,
                NewFunctionName = newName,
                NewSchema = newSchema
            };
            _operations.Value.Add(operation);
            return new OperationBuilder<RenameFunction>(operation);
        }
    }
}
