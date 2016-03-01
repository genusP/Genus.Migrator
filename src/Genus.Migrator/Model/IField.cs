using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace Genus.Migrator.Model
{
    public interface IField:IModelNamedObject
    {
        ITable Table { get; }
        string DbName { get; }
        DbType DataType { get;}
        bool IsNullable { get;}
        int Length { get;}
        bool IsIdentity { get; }
        IReadOnlyDictionary<ProviderName, string> Collation { get;  }
        IReadOnlyDictionary<ProviderName, string> Default { get; }
        IEnumerable<IAssociation> ForeignKeys { get; }
    }
}