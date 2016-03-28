using System.Collections.Generic;
using Genus.Migrator.Migrations.Operations;
using Genus.Migrator.Model;

namespace Genus.Migrator.Migrations.Design
{
    public interface IModelComparer
    {
        IEnumerable<MigrationOperation> CompareModel(IModel sourceModel, IModel targetModel);
    }
}