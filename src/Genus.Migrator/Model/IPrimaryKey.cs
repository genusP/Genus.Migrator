using System.Collections.Generic;

namespace Genus.Migrator.Model
{
    public interface IPrimaryKey
    {
        string DbName { get; }
        IEnumerable<IField> Items { get; }
    }
}