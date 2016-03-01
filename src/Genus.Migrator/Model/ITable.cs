using System.Collections.Generic;

namespace Genus.Migrator.Model
{
    public interface ITable : IDbObject, IModelNamedObject
    {
        IModel Model { get; }
        IEnumerable<IField> Fields { get; }
        IPrimaryKey PrimaryKey { get; }
        IEnumerable<IAssociation> Associations { get; }
        IEnumerable<IIndex> Indexes { get; }

        IField FindField(string name);
    }
}