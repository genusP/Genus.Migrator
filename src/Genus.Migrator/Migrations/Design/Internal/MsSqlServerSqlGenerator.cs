using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Genus.Migrator.Migrations.Operations;
using Genus.Migrator.Model;

namespace Genus.Migrator.Migrations.Design.Internal
{
    public class MsSqlServerSqlGenerator : SqlGenerator
    {
        public override string BatchTerminator => "GO";

        public override ProviderName ProviderName => ProviderName.MicrosoftSqlServer;

        protected override char BeginQuote => '[';

        protected override char EndQuote => ']';

        protected override string Generate(RenameField operation)
        {
            //TODO: Add support schema
            return $"Exec sp_rename '{operation.TableName}.{operation.FieldName}', '{operation.NewFieldName}', 'COLUMN'";
        }

        protected override string Generate(RenameIndex operation)
        {
            return $"Exec sp_rename '{operation.TableName}.{operation.IndexName}', '{operation.NewIndexName}', 'INDEX'";
        }

        protected override string Generate(RenameView operation)
        {
            return $"Exec sp_rename '{operation.ViewName}', '{operation.NewViewName}'";
        }

        protected override string Generate(RenameTable operation)
        {
            return $"Exec sp_rename '{operation.TableName}', '{operation.NewTableName}'";
        }

        protected override string Generate(RenameFunction operation)
        {
            return $"Exec sp_rename '{operation.FunctionName}', '{operation.NewFunctionName}'";
        }

        protected override string Generate(EnsureSchema operation)
        {
            return $"CREATE SCHEMA {Quote(operation.Schema)}";
        }

        protected override string Generate(DropIndex operation)
        {
            builder.Clear();
            builder.Append($"DROP INDEX {Quote(operation.IndexName)} ON ");
            if (!string.IsNullOrWhiteSpace(operation.Schema))
                builder.Append(Quote(operation.Schema))
                    .Append(".");
            builder.Append(Quote(operation.TableName));
            return builder.ToString();
            
        }
    }
}
