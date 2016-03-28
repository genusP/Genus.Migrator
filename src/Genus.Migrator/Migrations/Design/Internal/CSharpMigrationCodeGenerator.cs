using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Genus.Migrator.Migrations.Operations;
using Genus.Migrator.Model;

namespace Genus.Migrator.Migrations.Design.Internal
{
    public class CSharpMigrationCodeGenerator : IMigrationCodeGenerator
    {
        public static IList<string> Usings = new List<string>
        {
            "Genus.Migrator.Migrations",
            "Genus.Migrator.Model",
            "Genus.Migrator.Model.Builder",
            "System",
            "System.Collections.Generic",
            "System.Data"
        };
        private readonly string _namespace;
        private readonly string _className;
        private readonly string _baseClass;

        private CSharpOperationGenerator _operationGenerator = new CSharpOperationGenerator();

        public CSharpMigrationCodeGenerator(string @namespace, string className, string baseClass)
        {
            this._namespace = @namespace;
            this._className = className;
            _baseClass = baseClass?? "Migration";
        }


        public string Extension => ".cs";

        public virtual void GenerateMigration(
            IEnumerable<MigrationOperation> upOperations, 
            IEnumerable<MigrationOperation> downOperations, 
            IndentedStringBuilder builder)
        {
            GenereateUsings(builder);
            GenerateClass(builder,b=>
            {
                GenerateOperationMethod(b, upOperations, "Up");
                GenerateOperationMethod(b, downOperations, "Down");
            });
        }

        private void GenerateOperationMethod(IndentedStringBuilder b, IEnumerable<MigrationOperation> operations, string methodName)
        {
            b.AppendNewLine("protected override void ")
                .Append(methodName)
                .Append("(MigrationBuilder migrationBuilder)")
                .AppendNewLine("{");
            using (b.Indenter())
            {
                _operationGenerator.Generate(operations, b);
            }
            b.AppendNewLine("}");
        }

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
            bool withBaseClass=true)
        {
            builder.Append("namespace ")
                   .AppendLine(_namespace)
                   .Append("{");
            using (builder.Indenter())
            {
                builder.AppendNewLine("public partial class ")
                       .Append(_className);
                if(withBaseClass)
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

        public virtual void GenerateTargetModel(IModel targetModel, IndentedStringBuilder builder)
        {
            GenereateUsings(builder);
            GenerateClass(builder, b => {
                b.AppendNewLine("public override void BuildTargetModel(ModelBuilder modelBuilder)")
                    .AppendNewLine("{");
                using (b.Indenter())
                {
                    var modelCodeGenerator = new ModelCodeGenerator();
                    modelCodeGenerator.Generate(targetModel, b);
                }
                b.AppendNewLine("}");
            }, withBaseClass: false);
        }
    }
}
