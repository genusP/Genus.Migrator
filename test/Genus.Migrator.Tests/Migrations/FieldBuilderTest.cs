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
    public class FieldBuilderTest
    {
        [Fact]
        public void required_arguments()
        {
            var addFieldOperation = new AddField();
            var fieldBuilder = new FieldBuilder(addFieldOperation);

            Assert.Throws<ArgumentException>("collation", () => fieldBuilder.Collation(ProviderName.All, null));
            Assert.Throws<ArgumentException>("collation", () => fieldBuilder.Collation(ProviderName.All, " "));
            Assert.Throws<ArgumentException>("expression", () => fieldBuilder.Default(ProviderName.All, null));
            Assert.Throws<ArgumentException>("expression", () => fieldBuilder.Default(ProviderName.All, " "));
        }

        [Fact]
        public void collation()
        {
            var addFieldOperation = new AddField();
            var fieldBuilder = new FieldBuilder(addFieldOperation);

            var res = fieldBuilder.Collation(ProviderName.All, "c1")
                                  .Collation(ProviderName.MicrosoftSqlServer, "c2");

            Assert.NotNull(res);
            Assert.Equal("c1", res.Operation.Annotations["collation"]);
            Assert.Equal("c2", res.Operation.Annotations[ProviderName.MicrosoftSqlServer.ToString()+":collation"]);
        }
        
        [Fact]
        public void defaults()
        {
            var addFieldOperation = new AddField();
            var fieldBuilder = new FieldBuilder(addFieldOperation);

            var res = fieldBuilder.Default(ProviderName.All, "d1")
                                  .Default(ProviderName.MicrosoftSqlServer, "d2");

            Assert.NotNull(res);
            Assert.Equal("d1", res.Operation.Annotations["default"]);
            Assert.Equal("d2", res.Operation.Annotations[ProviderName.MicrosoftSqlServer.ToString() + ":default"]);
        }
    }
}
