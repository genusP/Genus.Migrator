using System.Collections.Generic;

namespace Genus.Migrator.Model
{
    public interface ISqlBody
    {
        IEnumerable<KeyValuePair<ProviderName, string>> SqlBody { get; }
    }
}