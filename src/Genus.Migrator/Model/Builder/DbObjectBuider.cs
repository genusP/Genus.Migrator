using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genus.Migrator.Model.Builder
{
    public abstract class DbObjectBuider<T>
        where T :DbObjectBuider<T>
    {
        public DbObjectBuider()
        {

        }
        public DbObjectBuider(string name, string schema)
        {
            HasName(name);
            HasSchema(schema);
        }

        public T HasName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is empty", nameof(name));
            Name = name;
            return (T)this;
        }

        public T HasSchema(string schema)
        {
            Schema = schema;
            return (T)this;
        }

        protected virtual void Validate(string clrName) {
        }

        protected string Name { get; private set; }
        protected string Schema { get; private set; }
    }
}
