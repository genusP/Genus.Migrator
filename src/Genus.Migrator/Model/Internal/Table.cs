using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genus.Migrator.Model.Internal
{
    [System.Diagnostics.DebuggerDisplay("Schema = {Schema}, DbName = {DbName}")]
    internal class Table : ITable
    {
        public Table(string clrName, IModel model)
        {
            ClrName = clrName;
            Model = model;
            DbName = ClrName.Split('.').Last();
        }

        public IModel Model { get; }

        public string ClrName { get; }

        public string DbName
        {
            get;
            set;
        }

        public IEnumerable<IField> Fields
        {
            get;
            internal set;
        }

        public IEnumerable<IAssociation> Associations
        {
            get;
            set;
        }

        public IPrimaryKey PrimaryKey
        {
            get;
            set;
        }

        public string Schema
        {
            get;
            set;
        }

        public IReadOnlyDictionary<ProviderName, string> With { get; set; }

        public IEnumerable<IIndex> Indexes
        {
            get;
            set;
        }

        public IEnumerable<ITrigger> Triggers
        {
            get;
            set;
        }

        public IField FindField(string name)
        {
            return Fields.FirstOrDefault(f=>f.ClrName == name);
        }
    }
}
