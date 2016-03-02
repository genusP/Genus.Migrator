using Genus.Migrator.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Genus.Migrator.Migrations.Operations.Builders
{
    public class CreateTableBuilder<T>:OperationBuilder<CreateTable>
    {
        public CreateTableBuilder(CreateTable operation)
            :base(operation)
        {
        }
        public OperationBuilder<AddPrimaryKey> PrimaryKey(string name, params Expression<Func<T, object>>[] fields)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is empty", nameof(name));
            if (fields.Length == 0)
                throw new ArgumentException("Data need", nameof(fields));
            var o = new AddPrimaryKey
            {
                PKName = name,
                Fields = fields.Select(_ => ExpressionHelper.GetPropertyName(_)).ToArray()
            };
            return new OperationBuilder<AddPrimaryKey>(o);
        }
    }
}
