using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Genus.Migrator.Model;

namespace Genus.Migrator.Migrations.Design.Internal
{
    public class CSharpSnapshotCodeGenerator : ISnapshotCodeGenerator
    {
        private readonly string _baseClass;
        private readonly string _className;
        private readonly string _namespace;

        public static IList<string> Usings = new List<string>
        {
            "Genus.Migrator.Migrations",
            "Genus.Migrator.Model",
            "Genus.Migrator.Model.Builder",
            "System",
            "System.Collections.Generic",
            "System.Data"
        };

        public CSharpSnapshotCodeGenerator(string @namespace, string className, string baseClass)
        {
            this._namespace = @namespace;
            this._className = className;
            this._baseClass = baseClass??"Snapshot";
        }

        public string Extension => ".cs";

        public virtual void Generate(IModel model, IndentedStringBuilder builder)
        {
            GenereateUsings(builder);
            GenerateClass(builder, b =>
            {
                b.AppendNewLine(BuildModelDeclaration)
                    .AppendNewLine("{");
                using (b.Indenter())
                {
                    b.AppendLine("");
                    var mcg = new ModelCodeGenerator();
                    mcg.Generate(model, b);
                }
                b.AppendNewLine("}");
            });
        }

        protected virtual string BuildModelDeclaration
            =>"protected override void BuildModel(ModelBuilder modelBuilder)";

        protected virtual void GenereateUsings(IndentedStringBuilder builder)
        {
            foreach (var item in Usings)
            {
                builder.Append("using ");
                builder.Append(item);
                builder.AppendLine(";");
            }
        }

        protected virtual void GenerateClass(
            IndentedStringBuilder builder,
            Action<IndentedStringBuilder> generateBody,
            bool withBaseClass = true)
        {
            builder.Append("namespace ")
                   .AppendLine(_namespace)
                   .Append("{");
            using (builder.Indenter())
            {
                builder.AppendNewLine("public partial class ")
                       .Append(_className);
                if (withBaseClass)
                    builder.Append(": ")
                       .Append(_baseClass);
                builder.AppendNewLine("{");
                using (builder.Indenter())
                {
                    generateBody(builder);
                }
                builder.AppendNewLine("}");
            }
            builder.AppendNewLine("}");
        }
    }
}
