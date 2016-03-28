using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Migrator.Migrations.Design
{
    public interface ICodeGeneratorFactory
    {
        IMigrationCodeGenerator MigrationCodeGenerator(string @namespace, string className, string baseClass=null);
        ISnapshotCodeGenerator SnapshotGenerator(string @namespace, string className, string baseClass=null);
    }
}
