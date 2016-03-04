using Genus.Migrator.Migrations.Operations;
using Genus.Migrator.Migrations.Operations.Builders;
using Genus.Migrator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Genus.Migrator.Tests.Migrations
{
    public class CreateViewBuilderTest
    {
        [Fact]
        public void required_arg()
        {
            var operation = new CreateView();
            var target = new CreateViewBuilder(operation);

            Assert.Throws<ArgumentException>(() => target.With(ProviderName.MicrosoftSqlServer, null));
            Assert.Throws<ArgumentException>(() => target.With(ProviderName.MicrosoftSqlServer, " "));
        }

        [Fact]
        public void set_with()
        {
            var operation = new CreateView();
            var target = new CreateViewBuilder(operation);

            var res = target.With(ProviderName.All, "script");

            Assert.NotNull(res);
            Assert.NotNull(res.Operation);
            Assert.Equal(1, res.Operation.Annotations.Count);
            Assert.True(res.Operation.Annotations.ContainsKey("with"));
            Assert.Equal("script", res.Operation.Annotations["with"]);
        }
    }
}
