using Genus.Migrator.Migrations.Operations;
using Genus.Migrator.Migrations.Operations.Builders;
using Genus.Migrator.Model;
using System;
using System.Collections.Generic;
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

        public CreateTableBuilder<T> CreateTable<T>(
            string name, 
            Func<FieldsBuilder, T> fields, 
            AddPrimaryKey pk=null, 
            string schema=null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Need value", nameof(name));
            if (fields == null)
                throw new ArgumentNullException(nameof(fields));

            var builder = new FieldsBuilder();
            var fieldsObj = fields(builder);
            var operation = new CreateTable
            {
                Schema = schema,
                TableName = name,
                Fields = fieldsObj.GetType().GetProperties()
                            .Select(p =>
                            {
                                var op = (OperationBuilder<AddField>)p.GetValue(fieldsObj);
                                op.Operation.Name = p.Name;
                                return op.Operation;
                            }),
                PrimaryKey = pk

            };
            _operations.Value.Add(operation);
            return new CreateTableBuilder<T>(operation);
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
        {
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

        public OperationBuilder<ExecuteSql> Sql(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
                throw new ArgumentException("Need value", nameof(sql));
            var operation = new ExecuteSql
            {
                Sql= sql
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
    }
}
