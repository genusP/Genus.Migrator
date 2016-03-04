using Genus.Migrator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Migrator.Migrations.Operations.Builders
{
    public class OperationBuilder<TOperation>
        where TOperation : MigrationOperation
    {
        public OperationBuilder(TOperation operation)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));
            Operation = operation;
        }

        public virtual TOperation Operation { get; }

        public OperationBuilder<TOperation> Annotation(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            if (Operation.Annotations.ContainsKey(key))
                Operation.Annotations[key] = value;
            else
                Operation.Annotations.Add(key, value);
            return this;
        }

        protected void AnnotateByProvider(ProviderName provider, string key, string value)
        {
            if (provider != ProviderName.All)
                key = provider.ToString() + ":" + key;
            Annotation(key, value);
        }
    }
}
