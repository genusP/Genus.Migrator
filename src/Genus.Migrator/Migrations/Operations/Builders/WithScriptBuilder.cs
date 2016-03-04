using Genus.Migrator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Migrator.Migrations.Operations.Builders
{
    public abstract class WithScriptBuilder<T, TOperation>:OperationBuilder<TOperation>
        where T :WithScriptBuilder<T,TOperation>
        where TOperation : MigrationOperation
    {
        public WithScriptBuilder(TOperation operation)
            :base(operation)
        {
        }

        public T SetScript(ProviderName provider, string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
                throw new ArgumentException("Need value", nameof(sql));
            AnnotateByProvider(provider, "sql", sql);
            return (T)this;
        }
    }
}
