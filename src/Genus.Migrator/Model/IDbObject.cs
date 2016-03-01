using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genus.Migrator.Model
{
    public interface IDbObject
    {
        string Schema { get; }
        string DbName { get;}
        IReadOnlyDictionary<ProviderName, string> With { get; }
    }
}
