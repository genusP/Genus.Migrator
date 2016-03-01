using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genus.Migrator.Model.Internal
{
    internal class Function : IFunction
    {
        public Function(string name, IModel model)
        {
            ClrName = name;
            Model = model;
        }

        public IModel Model { get; }

        public string ClrName
        {
            get;
        }

        public IEnumerable<KeyValuePair<ProviderName, string>> Scripts
        {
            get;
            set;
        }

        public IReadOnlyDictionary<ProviderName, string> With
        {
            get;
        }
    }
}
