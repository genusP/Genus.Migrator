using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Migrator.Model.Builder
{
    public abstract class DbObjectWithScriptsBuildercs<TBuilder>:DbObjectBuider<TBuilder>
        where TBuilder: DbObjectWithScriptsBuildercs<TBuilder>
    {
        readonly Dictionary<ProviderName, string> _bodyScripts = new Dictionary<ProviderName, string>();

        public TBuilder SetBodyScript(ProviderName provider, string selectScript)
        {
            _bodyScripts[provider]= selectScript;
            return (TBuilder)this;
        }

        protected IReadOnlyDictionary<ProviderName, string> Scripts
        {
            get
            {
                return new ReadOnlyDictionary<ProviderName, string>(_bodyScripts);
            }
        }
    }
}
