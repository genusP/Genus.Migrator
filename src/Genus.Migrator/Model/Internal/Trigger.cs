using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Migrator.Model.Internal
{
    public class Trigger : ITrigger
    {
        public Trigger(string schema, string name, IDbObject target)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is empty", nameof(name));
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            Schema = schema;
            DbName = name;
            Target = target;
        }
        public string DbName
        {
            get;
        }

        public TriggerOperation Operations
        {
            get;
            set;
        }

        public string Schema
        {
            get;
        }

        public IEnumerable<KeyValuePair<ProviderName, string>> SqlBody
        {
            get;
            set;
        }

        public IDbObject Target
        {
            get;
        }

        public TriggerType TriggerType
        {
            get;
            set;
        }

        public IReadOnlyDictionary<ProviderName, string> With
        {
            get;
            set;
        }
    }
}
