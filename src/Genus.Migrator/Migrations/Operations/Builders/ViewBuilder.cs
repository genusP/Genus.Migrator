using Genus.Migrator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Migrator.Migrations.Operations.Builders
{
    public class ViewBuilder:WithScriptBuilder<ViewBuilder, ViewOperation>
    {
        public ViewBuilder(ViewOperation operation)
            :base(operation)
        {

        }

        public ViewBuilder With(ProviderName provider, string with)
        {
            if (string.IsNullOrWhiteSpace(with))
                throw new ArgumentException("Need value", nameof(with));
            AnnotateByProvider(provider, "with", with);
            return this;
        }
    }
}
