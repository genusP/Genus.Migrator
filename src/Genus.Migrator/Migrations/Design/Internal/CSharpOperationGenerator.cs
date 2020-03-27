using Genus.Migrator.Migrations.Operations;
using Genus.Migrator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Migrator.Migrations.Design.Internal
{
    public class CSharpOperationGenerator
    {
        private readonly IDictionary<Type, Action<MigrationOperation, IndentedStringBuilder>> _operationGenerators;

        public CSharpOperationGenerator()
        {
            _operationGenerators = GetOperationGenerators();
        }

        protected virtual IDictionary<Type, Action<MigrationOperation, IndentedStringBuilder>> GetOperationGenerators()
        {
            return new Dictionary<Type, Action<MigrationOperation, IndentedStringBuilder>>
            {
                {typeof(AddField), (o, b)=>Generate((AddField)o, b) },
                {typeof(AddForeignKey), (o, b)=>Generate((AddForeignKey)o, b) },
                {typeof(AddPrimaryKey), (o, b)=>Generate((AddPrimaryKey)o, b) },
                {typeof(AlterField), (o, b)=>Generate((AlterField)o, b) },
                {typeof(AlterView), (o, b)=>Generate((AlterView)o, b) },
                {typeof(CreateFunction), (o, b)=>Generate((CreateFunction)o, b) },
                {typeof(CreateIndex), (o, b)=>Generate((CreateIndex)o, b) },
                {typeof(CreateTable), (o, b)=>Generate((CreateTable)o, b) },
                {typeof(CreateView), (o, b)=>Generate((CreateView)o, b) },
                {typeof(DropField), (o, b)=>Generate((DropField)o, b) },
                {typeof(DropForeignKey), (o, b)=>Generate((DropForeignKey)o, b) },
                {typeof(DropFunction), (o, b)=>Generate((DropFunction)o, b) },
                {typeof(DropIndex), (o, b)=>Generate((DropIndex)o, b) },
                {typeof(DropPrimaryKey), (o, b)=>Generate((DropPrimaryKey)o, b) },
                {typeof(DropSchema), (o, b)=>Generate((DropSchema)o, b) },
                {typeof(DropTable), (o, b)=>Generate((DropTable)o, b) },
                {typeof(DropView), (o, b)=>Generate((DropView)o, b) },
                {typeof(EnsureSchema), (o, b)=>Generate((EnsureSchema)o, b) },
                {typeof(ExecuteSql), (o, b)=>Generate((ExecuteSql)o, b) },
                {typeof(RenameField), (o, b)=>Generate((RenameField)o, b) },
                {typeof(RenameFunction), (o, b)=>Generate((RenameFunction)o, b) },
                {typeof(RenameIndex), (o, b)=>Generate((RenameIndex)o, b) },
                {typeof(RenameTable), (o, b)=>Generate((RenameTable)o, b) },
                {typeof(RenameView), (o, b)=>Generate((RenameView)o, b) },
                {typeof(CreateTrigger), (o, b)=>Generate((CreateTrigger)o, b) },
                {typeof(DropTrigger), (o, b)=>Generate((DropTrigger)o, b) },
            };
        }

        protected string EscapeString(string str)
            => str.Replace("\"", "\\\"");

        private void Generate<T>(IndentedStringBuilder builder,T operation,params Func<T, string>[] arguments)
            where T :MigrationOperation
        => Generate<T>(builder, operation, null, false, arguments);


        private void Generate<T>(
            IndentedStringBuilder builder,
            T operation, 
            string migrationBuilderFunction = null,
            bool noSemicolon =false,
            params Func<T, string>[] arguments
            )
            where T :MigrationOperation
        {
            if (migrationBuilderFunction == null)
                migrationBuilderFunction = operation.GetType().Name;
            bool first = true;
            builder.AppendNewLine("migrationBuilder.")
                .Append(migrationBuilderFunction)
                .Append("(");
            foreach (var arg in arguments)
            {
                var value = arg(operation);
                if (!string.IsNullOrWhiteSpace(value))
                {
                    if (first)
                        first = false;
                    else
                        builder.Append(", ");
                    builder.Append("@\"")
                        .Append(EscapeString(value))
                        .Append("\"");
                }
            }
            builder.Append(")");
            if (!noSemicolon)
                builder.Append(";");
        }

        protected virtual void Generate(RenameView operation, IndentedStringBuilder  builder)
            =>Generate(builder, operation, o => o.ViewName, o => o.NewViewName, o => o.Schema, o => o.NewSchema);

        protected virtual void Generate(RenameTable operation, IndentedStringBuilder  builder)
            =>Generate(builder, operation, o => o.TableName, o => o.NewTableName, o => o.Schema, o => o.NewSchema);

        protected virtual void Generate(RenameIndex operation, IndentedStringBuilder  builder)
            =>Generate(builder, operation, o => o.IndexName, o => o.NewIndexName, o=>o.TableName, o => o.Schema);

        protected virtual void Generate(RenameFunction operation, IndentedStringBuilder  builder)
            =>Generate(builder, operation, o => o.FunctionName, o => o.NewFunctionName, o => o.Schema, o => o.NewSchema);

        protected virtual void Generate(RenameField operation, IndentedStringBuilder  builder)
            =>Generate(builder, operation, o => o.FieldName, o => o.NewFieldName, o => o.TableName, o => o.Schema);

        protected virtual void Generate(ExecuteSql operation, IndentedStringBuilder  builder)
            =>Generate(builder, operation, "Sql", true, o => o.Sql, o => o.Provider.ToString());

        protected virtual void Generate(EnsureSchema operation, IndentedStringBuilder  builder)
            =>Generate(builder, operation, o => o.Schema);

        protected virtual void Generate(DropView operation, IndentedStringBuilder  builder)
            =>Generate(builder, operation, o => o.ViewName,o => o.Schema);

        protected virtual void Generate(DropTable operation, IndentedStringBuilder  builder)
            =>Generate(builder, operation, o => o.TableName, o => o.Schema);

        protected virtual void Generate(DropSchema operation, IndentedStringBuilder  builder)
            =>Generate(builder, operation, o => o.Schema);

        protected virtual void Generate(DropPrimaryKey operation, IndentedStringBuilder builder)
            => Generate(builder, operation, o => o.PrimaryKeyName, o => o.TableName, o => o.Schema);

        protected virtual void Generate(DropIndex operation, IndentedStringBuilder builder)
            => Generate(builder, operation, o => o.IndexName, o => o.TableName, o => o.Schema);

        protected virtual void Generate(DropFunction operation, IndentedStringBuilder  builder)
            => Generate(builder, operation, o => o.FunctionName, o => o.Schema);

        protected virtual void Generate(DropForeignKey operation, IndentedStringBuilder  builder)
            => Generate(builder, operation, o => o.ForeignKeyName, o => o.TableName, o => o.Schema);

        protected virtual void Generate(DropField operation, IndentedStringBuilder  builder)
            => Generate(builder, operation, o => o.FieldName, o => o.TableName, o => o.Schema);

        protected virtual void Generate(CreateView operation, IndentedStringBuilder  builder)
        {
            Generate(builder, operation, null, true, o => o.ViewName, o => o.Schema);
            using (builder.Indenter())
            {
                foreach (var item in operation.Annotations)
                {
                    var keyParts = item.Key.Split(':');
                    ProviderName provider = ProviderName.All;
                    if (keyParts.Length == 2)
                        Enum.TryParse(keyParts[0], true, out provider);
                    string funcName;
                    if (string.Equals(keyParts.Last(), "sql", StringComparison.OrdinalIgnoreCase))
                        funcName = "SetScript";
                    else if (string.Equals(keyParts.Last(), "with", StringComparison.OrdinalIgnoreCase))
                        funcName = "With";
                    else funcName = "Annotation";
                    
                    builder.AppendNewLine(".")
                        .Append(funcName)
                        .Append("(@\"")
                        .Append(EscapeString(item.Value))
                        .Append(")");
                    
                }
                builder.Append(";");
            }
        }

        protected virtual void Generate(CreateTable operation, IndentedStringBuilder  builder)
        {
            builder.AppendNewLine("migrationBuilder.CreateTable(");
            using (builder.Indenter())
            {
                builder.AppendNewLine("name: ")
                    .Append("@\"")
                    .Append(EscapeString(operation.TableName))
                    .Append("\",")
                    .AppendNewLine("fields: table =>")
                    .AppendNewLine("{");
                using (builder.Indenter())
                {
                    foreach (var item in operation.Fields)
                    {
                        builder.AppendNewLine("table.Field(@\"")
                            .Append(EscapeString(item.Name))
                            .Append("\", DbType.")
                            .Append(item.Type);
                        if (item.Length.HasValue)
                            builder.Append(", length: ")
                                .Append(item.Length.Value);
                        builder.Append(", nullable: ")
                            .Append(item.IsNullable.ToString().ToLower());
                        if (item.IsIdentity)
                            builder.Append(", identity: ")
                                .Append(item.IsIdentity.ToString().ToLower());
                        builder.Append(")");
                        if(item.Annotations.Any())
                        {
                            using (builder.Indenter())
                            {
                                foreach (var a in item.Annotations)
                                {
                                    var keyParts = a.Key.Split(':');
                                    ProviderName provider = ProviderName.All;
                                    if (keyParts.Length == 2)
                                        Enum.TryParse(keyParts[0], true, out provider);
                                    string funcName;
                                    if (string.Equals(keyParts.Last(), "default", StringComparison.OrdinalIgnoreCase))
                                        funcName = "Default";
                                    else if (string.Equals(keyParts.Last(), "collation", StringComparison.OrdinalIgnoreCase))
                                        funcName = "Collation";
                                    else funcName = "Annotation";

                                    builder.AppendNewLine(".")
                                        .Append(funcName)
                                        .Append("( ProviderName.")
                                        .Append(provider)
                                        .Append(", @\"")
                                        .Append(EscapeString(a.Value))
                                        .Append("\")");
                                }
                            }
                        }
                        builder.Append(";");
                    }
                }
                builder.AppendNewLine("}");

                if (operation.PrimaryKey != null)
                {
                    builder.Append(",")
                        .AppendNewLine("pk: table => table.PrimaryKey(@\"")
                        .Append(EscapeString(operation.PrimaryKey.PKName))
                        .Append("\"");
                    foreach (var field in operation.PrimaryKey.Fields)
                    {
                        builder.Append(", @\"")
                            .Append(EscapeString(field))
                            .Append("\"");
                    }
                    builder.Append(")");
                }
                builder.AppendNewLine(");");
            }
        }

        protected virtual void Generate(CreateIndex operation, IndentedStringBuilder  builder)
        {
            builder.AppendNewLine("migrationBuilder.CreateIndex(");
            using (builder.Indenter())
            {
                builder.AppendNewLine("name: @\"")
                    .Append(EscapeString(operation.IndexName))
                    .Append("\",")
                    .AppendNewLine("table: @\"")
                    .Append(EscapeString( operation.TableName))
                    .Append("\",")
                    .AppendNewLine("fields: new []{@\"")
                    .Append(string.Join("\", @\"", operation.Fields.Select(_ => EscapeString(_))))
                    .Append("\"}");
                if (operation.IsUnique)
                    builder.Append(",")
                        .AppendNewLine("unique: true");
                if (!string.IsNullOrWhiteSpace(operation.Schema))
                    builder.Append(",")
                        .AppendNewLine("schema: @\"")
                        .Append(EscapeString(operation.Schema))
                        .Append("\"");
            }
            builder.AppendNewLine(");");
        }

        protected virtual void Generate(CreateFunction operation, IndentedStringBuilder  builder)
        {
            Generate(builder, operation, null, true, o => o.FunctionName, o => o.Schema);
            using (builder.Indenter())
            {
                GenerateSetScript(operation, builder);
                builder.Append(";");
            }
        }

        protected void GenerateSetScript(MigrationOperation operation, IndentedStringBuilder builder)
        {
            foreach (var item in operation.Annotations)
            {
                var keyParts = item.Key.Split(':');
                ProviderName provider = ProviderName.All;
                if (keyParts.Length == 2)
                    Enum.TryParse(keyParts[0], true, out provider);
                string funcName;
                if (string.Equals(keyParts.Last(), "sql", StringComparison.OrdinalIgnoreCase))
                    funcName = "SetScript";
                else funcName = "Annotation";

                builder.AppendNewLine(".")
                    .Append(funcName)
                    .Append("(@\"")
                    .Append(EscapeString(item.Value))
                    .Append(")");
            }
        }

        protected virtual void Generate(AlterField operation, IndentedStringBuilder builder)
        {
            builder.AppendNewLine("migrationBuilder.AlterField(");
            using (builder.Indenter())
            {
                if (!string.IsNullOrEmpty(operation.Schema))
                    builder.AppendNewLine("schema: @\"")
                        .Append(operation.Schema)
                        .Append("\",");
                builder.AppendNewLine("table: @\"")
                    .Append(operation.TableName)
                    .Append("\",")
                    .AppendNewLine("name: @\"")
                    .Append(operation.Name)
                    .Append("\"");
                if (operation.Type != null)
                    builder.Append(",")
                        .AppendNewLine("dbType: DbType.")
                        .Append(operation.Type.Value.ToString());
                if (operation.Length != null && operation.Length.Value>-1)
                    builder.Append(",")
                        .AppendNewLine("length: ")
                        .Append(operation.Length.Value.ToString());
                if (operation.IsNullable!=null)
                    builder.Append(",")
                        .AppendNewLine("nullable: ")
                        .Append(operation.IsNullable.ToString().ToLower());
                if (operation.IsIdentity!=null) 
                    builder.Append(",")
                        .AppendNewLine("identity: ")
                        .Append(operation.IsIdentity.ToString().ToLower());
            }
            builder.AppendNewLine(");");
        }
        protected virtual void Generate(AlterView operation, IndentedStringBuilder builder)
        {
            throw new NotImplementedException();
        }

        protected virtual void Generate(AddPrimaryKey operation, IndentedStringBuilder  builder)
        {
            builder.AppendNewLine("migrationBuilder.AddPrimaryKey(");
            using (builder.Indenter())
            {
                builder.AppendNewLine("name: @\"")
                    .Append(EscapeString(operation.PKName))
                    .Append("\",")
                    .AppendNewLine("table: @\"")
                    .Append(EscapeString(operation.TableName))
                    .Append("\",")
                    .AppendNewLine("fields: new []{@\"")
                    .Append(string.Join("\", @\"", operation.Fields.Select(_ => EscapeString(_))))
                    .Append("\"}");
                if (!string.IsNullOrWhiteSpace(operation.Schema))
                    builder.Append(",")
                        .AppendNewLine("schema: @\"")
                        .Append(EscapeString(operation.Schema))
                        .Append("\"");
            }
            builder.AppendNewLine(");");
        }

        protected virtual void Generate(AddForeignKey operation, IndentedStringBuilder  builder)
        {
            builder.AppendNewLine("migrationBuilder.AddForeignKey(");
            using (builder.Indenter())
            {
                builder.AppendNewLine("name: @\"")
                    .Append(EscapeString(operation.ForeignKeyName))
                    .Append("\",");
                if (!string.IsNullOrWhiteSpace(operation.Schema))
                    builder.AppendNewLine("schema: @\"")
                        .Append(EscapeString(operation.Schema))
                        .Append("\",");
                builder.AppendNewLine("table: @\"")
                    .Append(EscapeString(operation.TableName))
                    .Append("\",")
                    .AppendNewLine("field: @\"")
                    .Append(EscapeString(operation.FieldName))
                    .Append("\",");
                if (!string.IsNullOrWhiteSpace(operation.PrincipalSchema))
                    builder.AppendNewLine("principalSchema: @\"")
                        .Append(EscapeString(operation.PrincipalSchema))
                        .Append("\",");
                builder.AppendNewLine("principalTable: @\"")
                    .Append(EscapeString(operation.PrincipalTable))
                    .Append("\",")
                    .AppendNewLine("principalField: @\"")
                    .Append(EscapeString(operation.PrincipalField))
                    .Append("\"");
                if (operation.OnUpdate.HasValue)
                    builder.Append(",")
                        .AppendNewLine("onUpdate: ForeignKeyAction.")
                        .Append(operation.OnUpdate.Value);
                if (operation.OnDelete.HasValue)
                    builder.Append(",")
                        .AppendNewLine("onDelete: ForeignKeyAction.")
                        .Append(operation.OnDelete.Value);
            }
            builder.AppendNewLine(");");
        }

        protected virtual void Generate(AddField operation, IndentedStringBuilder builder)
        {
            builder.AppendNewLine("migrationBuilder.AddField(");
            using (builder.Indenter())
            {
                builder.AppendNewLine("name: @\"")
                    .Append(EscapeString(operation.Name))
                    .Append("\",");
                if (!string.IsNullOrWhiteSpace(operation.Schema))
                    builder.AppendNewLine("schema: @\"")
                        .Append(EscapeString(operation.Schema))
                        .Append("\",");
                builder.AppendNewLine("table: @\"")
                    .Append(EscapeString(operation.TableName))
                    .Append("\",")
                    .AppendNewLine("dbType: DbType.")
                    .Append(operation.Type);
                if (operation.Length.HasValue && operation.Length>0)
                    builder.Append(",")
                        .AppendNewLine("length: ")
                        .Append(operation.Length.Value);
                builder.Append(",")
                    .AppendNewLine("nullable: ")
                    .Append(operation.IsNullable.ToString().ToLower());
                if (operation.IsIdentity)
                    builder.Append(",")
                        .AppendNewLine("identity: ")
                        .Append(operation.IsIdentity.ToString().ToLower());
                builder.Append(")");
                if (operation.Annotations.Any())
                {
                    using (builder.Indenter())
                    {
                        foreach (var a in operation.Annotations)
                        {
                            var keyParts = a.Key.Split(':');
                            ProviderName provider = ProviderName.All;
                            if (keyParts.Length == 2)
                                Enum.TryParse(keyParts[0], true, out provider);
                            string funcName;
                            if (string.Equals(keyParts.Last(), "default", StringComparison.OrdinalIgnoreCase))
                                funcName = "Default";
                            else if (string.Equals(keyParts.Last(), "collation", StringComparison.OrdinalIgnoreCase))
                                funcName = "Collation";
                            else funcName = "Annotation";

                            builder.AppendNewLine(".")
                                .Append(funcName)
                                .Append("(@\"")
                                .Append(EscapeString(a.Value))
                                .Append(")");
                        }
                    }
                }
            }
            builder.Append(";");
        }

        protected virtual void Generate(CreateTrigger operation, IndentedStringBuilder builder)
        {
            builder.AppendNewLine("migrationBuilder.AddTrigger(");
            using (builder.Indenter())
            {
                if (!string.IsNullOrWhiteSpace(operation.TriggerSchema))
                    builder.AppendNewLine($"triggerSchema: @\"{operation.TriggerSchema}\",");
                builder.AppendNewLine($"triggerName: @\"{operation.TriggerName}\",");
                if (!string.IsNullOrWhiteSpace(operation.Schema))
                    builder.AppendNewLine($"tableSchema: @\"{operation.Schema}\",");
                builder.AppendNewLine($"tableName: @\"{operation.TableName}\",")
                    .AppendNewLine($"triggerType: TriggerType.{operation.TriggerType},")
                    .AppendNewLine($"triggerOperations: {operation.TriggerOperation}")
                    .Append(")");
                GenerateSetScript(operation, builder);
                builder.Append(";");
            }
        }

        protected virtual void Generate(DropTrigger operation, IndentedStringBuilder builder)
        {
            builder.AppendNewLine("migrationBuilder.DropTrigger(")
                .Append($"@\"{operation.TriggerName}\"");
            if (!string.IsNullOrWhiteSpace(operation.TriggerSchema))
                builder.Append($", @\"{operation.TriggerSchema}\", ");
            builder.Append(");");
        }

        public void Generate(IEnumerable<MigrationOperation> operations, IndentedStringBuilder builder)
        {
            builder.AppendLine("");
            foreach (var operation in operations)
            {
                Action<MigrationOperation, IndentedStringBuilder> generator;
                if (_operationGenerators.TryGetValue(operation.GetType(), out generator))
                    generator(operation, builder);
                else
                    throw new InvalidOperationException($"Unknown operation: {operation.GetType().FullName}");
            }
        }
    }
}
