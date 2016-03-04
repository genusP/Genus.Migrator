using Genus.Migrator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Migrator.Migrations.Operations.Builders
{
    public class FieldBuilder:OperationBuilder<AddField>
    {
        public FieldBuilder(AddField operation):
            base(operation)
        {
        }

        public FieldBuilder Default(ProviderName provider, string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
                throw new ArgumentException("Need not empty string", nameof(expression));
            AnnotateByProvider(provider, "Default", expression);
            return this;
        }

        public FieldBuilder Collation(ProviderName provider, string collation)
        {
            if (string.IsNullOrWhiteSpace(collation))
                throw new ArgumentException("Need not empty string", nameof(collation));
            AnnotateByProvider(provider, "Collation", collation);
            return this;
        }
    }
}
