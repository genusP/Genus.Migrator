using Genus.Migrator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;

namespace Genus.Migrator.Model.Internal
{
    [System.Diagnostics.DebuggerDisplay("DbName = {DbName}")]
    internal class Field : IField
    {
        public Field(
            string name, 
            ITable table, 
            DbType dbType, 
            int length,
            IReadOnlyDictionary<ProviderName, string> collation,
            IReadOnlyDictionary<ProviderName, string> @default
            )
        {
            ClrName = name;
            Table = table;
            DbName = name.Split('.').Last();
            DataType = dbType;
            Length = length;
            Collation = collation;
            Default = @default;
        }

        public ITable Table { get; }

        public string ClrName { get; }

        public IReadOnlyDictionary<ProviderName, string> Collation { get; }

        public DbType DataType { get; }

        public string DbName { get; internal set; }

        public IReadOnlyDictionary<ProviderName, string> Default { get; }

        public IEnumerable<IAssociation> ForeignKeys
        {
            get
            {
                return Table.Associations.Where(fk => fk.Field == this || fk.ReferenceField == this);
            }
        }

        public bool IsIdentity{ get; internal set; }

        public bool IsNullable { get; internal set; }

        public int Length { get; }
    }
}
