using System.Collections.Generic;

namespace Genus.Migrator.Model
{
    public interface IFunction:/*IDbObject,*/ IModelNamedObject
    {
        IModel Model { get; }
        IEnumerable<KeyValuePair<ProviderName, string>> Scripts { get; }
    }
}