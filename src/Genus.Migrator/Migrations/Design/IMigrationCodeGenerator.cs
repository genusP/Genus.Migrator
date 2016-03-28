using Genus.Migrator.Migrations.Operations;
using Genus.Migrator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Migrator.Migrations.Design
{
    public interface IMigrationCodeGenerator
    {
        string Extension { get; }
        void GenerateMigration(IEnumerable<MigrationOperation> upOperations, IEnumerable<MigrationOperation> downOperations, IndentedStringBuilder builder);
        void GenerateTargetModel(IModel model, IndentedStringBuilder builder);
    }
}
