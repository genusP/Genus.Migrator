using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genus.Migrator.Model.Internal
{
    [System.Diagnostics.DebuggerDisplay("Schema = {Schema}, DbName = {DbName}")]
    internal class View : IView
    {
        public View(string name, IModel model)
        {
            ClrName = name;
            Model = model;
            DbName = ClrName.Split('.').Last();
        }
        public string ClrName
        {
            get;
        }

        public string DbName
        {
            get;
            set;
        }

        public IModel Model
        {
            get;
        }

        public string Schema
        {
            get;
            set;
        }

        public IEnumerable<KeyValuePair<ProviderName, string>> SqlBody
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
