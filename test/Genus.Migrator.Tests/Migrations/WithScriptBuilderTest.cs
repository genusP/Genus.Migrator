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
    public class WithScriptBuilderTest
    {
        class builder: WithScriptBuilder<builder, CreateView>
        {
            public builder(CreateView o):base(o)
            {

            }
        }

        [Fact]
        public void required_arg()
        {
            var opertion = new CreateView();
            var target = new builder(opertion);

            Assert.Throws<ArgumentException>(() => target.SetScript(ProviderName.MicrosoftSqlServer, null));
            Assert.Throws<ArgumentException>(() => target.SetScript(ProviderName.MicrosoftSqlServer, " "));
        }

        [Fact]
        public void set_script()
        {
            var opertion = new CreateView();
            var target = new builder(opertion);

            var res = target.SetScript(ProviderName.All, "script");

            Assert.NotNull(res);
            Assert.NotNull(res.Operation);
            Assert.Equal(1, res.Operation.Annotations.Count);
            Assert.True(res.Operation.Annotations.ContainsKey("sql"));
            Assert.Equal("script", res.Operation.Annotations["sql"]);
        }
    }
}
