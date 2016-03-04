using Genus.Migrator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Migrator.Migrations.Operations.Builders
{
    public class CreateViewBuilder:WithScriptBuilder<CreateViewBuilder, CreateView>
    {
        public CreateViewBuilder(CreateView operation)
            :base(operation)
        {

        }

        public CreateViewBuilder With(ProviderName provider, string with)
        {
            if (string.IsNullOrWhiteSpace(with))
                throw new ArgumentException("Need value", nameof(with));
            AnnotateByProvider(provider, "with", with);
            return this;
        }
    }
}
