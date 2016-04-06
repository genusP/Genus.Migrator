using Genus.Migrator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Migrator.Migrations.Design.Internal
{
    public class ModelCodeGenerator
    {
        public virtual void Generate(IModel model, IndentedStringBuilder builder)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            foreach (var table in model.Tables)
            {
                GenerateTable(table, builder);
                builder.AppendLine("");
            }

            foreach(var view in model.Views)
            {
                GenerateView(view, builder);
            }
        }

        protected virtual void GenerateTable(ITable table, IndentedStringBuilder builder)
        {
            var getTableCode = $"modelBuilder.Table(\"{table.ClrName}\"";
            builder.Append(getTableCode)
                .Append(", tb=>{");
            using (builder.Indenter())
            {
                GenerateTableFields(table, builder);

                GenerateObjectNames(builder, table.DbName, "tb", table.Schema);
                builder.Append(";");

                if (table.With.Any())
                {
                    builder.AppendNewLine("tb");
                    using (builder.Indenter())
                    {
                        foreach (var with in table.With)
                        {
                            GenerateWith(with, builder);
                            builder.AppendLine("");
                        }
                    }
                }

                if (table.Associations.Any())
                {
                    foreach (var association in table.Associations)
                    {
                        builder.AppendNewLine("tb");
                        GenerateAssociation(association, builder);
                        builder.Append(";");
                    }
                }

                if (table.PrimaryKey != null)
                {
                    GeneratePrimaryKey(table, builder, "tb");
                    builder.Append(";");
                }
            }
            builder.AppendNewLine("});");

            foreach (var index in table.Indexes)
            {
                builder.Append(getTableCode).Append(")");
                using (builder.Indenter())
                {
                    GenerateTableIndex(index, builder);
                }
            }
        }

        private static void GenerateObjectNames(IndentedStringBuilder builder, string dbName, string objName="", string schema=null)
        {
            builder.AppendNewLine($"{objName}.HasName(\"{dbName}\") ");
            if (string.IsNullOrEmpty(schema) == false)
                builder.AppendNewLine($".HasSchema(\"{schema}\")");

        }

        protected virtual void GenerateTableFields(ITable table, IndentedStringBuilder builder)
        {
            foreach (var field in table.Fields)
            {
                builder.AppendLine("");
                GenerateField(field, "tb", builder);
                builder.Append(";");
            }
        }

        private static void GeneratePrimaryKey(ITable table, IndentedStringBuilder builder, string objName="")
        {
            var @params = "\"" + string.Join("\", \"", table.PrimaryKey.Items.Select(i => i.ClrName)) + "\"";
            if (string.IsNullOrWhiteSpace(@params)==false)
            {
                builder.AppendNewLine($"{objName}.HasKey(");
                builder.Append(@params).Append(")");
                if (string.IsNullOrEmpty(table.PrimaryKey.DbName) == false)
                {
                    using (builder.Indenter())
                    {
                        GenerateObjectNames(builder, table.PrimaryKey.DbName);
                    }
                }
            }
        }

        protected virtual void GenerateField(IField field, string v, IndentedStringBuilder builder)
        {
            builder.Append($"{v}.Field(\"{field.ClrName}\", DbType.")
                .Append(field.DataType);
            if(field.Length>-1)
            {
                builder.Append(", ");
                builder.Append(field.Length);
            }
            builder.Append(")");
            using (builder.Indenter())
            {
                if (string.IsNullOrEmpty(field.DbName) == false && field.ClrName != field.DbName)
                    builder.AppendNewLine($".HasName(\"{field.DbName}\")");

                if (field.IsNullable)
                    builder.AppendNewLine(".AsNullable()");
                if (field.IsIdentity)
                    builder.AppendNewLine(".AsIdentity()");

                foreach (var collation in field.Collation)
                {
                    builder.AppendNewLine($".HasCollation(ProviderName.{collation.Key}, \"{collation.Value}\")");
                }

                foreach (var def in field.Default)
                {
                    builder.AppendNewLine($".HasDefault(ProviderName.{def.Key}, \"{def.Value}\")");
                }
            }
        }

        protected virtual void GenerateWith(KeyValuePair<ProviderName, string> with, IndentedStringBuilder builder)
        {
            builder.Append($".With(ProviderName.{with.Key}, @\"{with.Value.Replace("\"", "\\\"")}\")");
        }

        protected virtual void GenerateTableIndex(IIndex index, IndentedStringBuilder builder)
        {
            builder.AppendNewLine($".Index(\"{index.DbName}\")");
            using (builder.Indenter())
            {
                foreach (var field in index.Fields)
                {
                    builder.AppendNewLine($".OnColumn(\"{field.Field.ClrName}\", {field.Decending.ToString().ToLower()})");
                } 

                if(index.IsUnique)
                    builder.AppendNewLine(".IsUnique()");
                builder.Append(";");
            }
        }

        protected virtual void GenerateAssociation(IAssociation association, IndentedStringBuilder builder)
        {
            var dependField = association.Field.ClrName;
            var principalTable = association.ReferenceField.Table.ClrName;
            var principalKey = association.ReferenceField.ClrName;
            builder.Append($".Association(\"{dependField}\",\"{principalTable}\",\"{principalKey}\")");
            using (builder.Indenter())
            {
                var depNavStr = string.IsNullOrWhiteSpace(association.DependentNavigation)
                                ? "null"
                                : $"\"{association.DependentNavigation}\"";
                var princNavStr = string.IsNullOrWhiteSpace(association.PrincipalNavigation)
                                ? "null"
                                : $"\"{association.PrincipalNavigation}\"";
                if(depNavStr!="null"||princNavStr!="null")
                    builder.AppendNewLine($".WithNavigation({depNavStr}, {princNavStr})");

                if (string.IsNullOrWhiteSpace(association.ForeignKeyName) == false)
                    builder.AppendNewLine($".AsForeignKey(\"{ association.ForeignKeyName}\")");
                if (association.OnDeleteAction.HasValue)
                    builder.AppendNewLine($".OnDelete(ForeignKeyAction.{association.OnDeleteAction.Value})");
                if (association.OnUpdateAction.HasValue)
                    builder.AppendNewLine($".OnUpdate(ForeignKeyAction.{association.OnUpdateAction.Value})");
            }
        }

        protected virtual void GenerateView(IView view, IndentedStringBuilder builder)
        {
            builder.AppendNewLine($"modelBuilder.View(\"{view.ClrName}\")");
            using (builder.Indenter())
            {
                if (string.IsNullOrEmpty(view.DbName))
                    builder.AppendNewLine($".HasName(\"{view.DbName}\")");
                if (string.IsNullOrEmpty(view.Schema))
                    builder.AppendNewLine($".HasName(\"{view.Schema}\")");
                foreach (var with in view.With)
                    GenerateWith(with, builder);

                foreach (var item in view.SqlBody)
                {
                    builder.Append($".SetBodyScript(ProviderName.{item.Key},");
                    builder.AppendLine("");
                    builder.Append($"@\"{item.Value.Replace("\"", "\\\"")}\")", ignoreIndent:true);
                }

                builder.Append(";");
            }
        }

        protected virtual void GenerateFunction(IFunction function, IndentedStringBuilder builder)
        {
            builder.AppendNewLine($"modelBuilder.Function(\"{function.ClrName}\")");
            using (builder.Indenter())
            {
                foreach (var item in function.Scripts)
                {
                    builder.AppendNewLine($".SetScript(ProviderName.{item.Key},");
                    builder.AppendLine("");
                    builder.Append($"@\"{item.Value.Replace("\"", "\\\"")}\"", ignoreIndent: true);
                }
            }
            builder.Append(";");
        }
    }
}
