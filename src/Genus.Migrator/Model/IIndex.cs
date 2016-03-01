using System.Collections.Generic;

namespace Genus.Migrator.Model
{
    public interface IIndex
    {
        string DbName { get; }
        ITable Table { get; }
        IEnumerable<IIndexItem> Fields { get; }
        bool IsUnique { get; }
    }
}