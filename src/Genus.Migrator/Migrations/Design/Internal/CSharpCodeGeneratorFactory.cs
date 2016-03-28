using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Migrator.Migrations.Design.Internal
{
    public class CSharpCodeGeneratorFactory : ICodeGeneratorFactory
    {
        public IMigrationCodeGenerator MigrationCodeGenerator(string @namespace, string className, string baseClass)
        {
            return  new CSharpMigrationCodeGenerator(@namespace, className, baseClass);
        }

        public ISnapshotCodeGenerator SnapshotGenerator(string @namespace, string className, string baseClass)
        {
            return new CSharpSnapshotCodeGenerator(@namespace, className, baseClass);
        }
    }
}
