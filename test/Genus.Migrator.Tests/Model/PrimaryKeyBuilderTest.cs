using Genus.Migrator.Model;
using Genus.Migrator.Model.Builder;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Genus.Migrator.Tests.Model
{
    public class PrimaryKeyBuilderTest
    {
        [Theory]
        [InlineData(null)]
        [InlineData("pk_teest")]
        public void build_pk(string name)
        {
            var tablename = "test";
            var tableBuilder = new TableBuilder();
            tableBuilder.HasName(tablename);
            var fb = tableBuilder.Field("id").HasType(DbType.Int32);
            var target = new PrimaryKeyBuilder(fb);
            if (name != null)
                target.HasName(name);
            var table = tableBuilder.Build("test", null);

            var res = target.Build(table, _=>fb==_?table.Fields.First():null);

            Assert.NotNull(res);
            Assert.Equal(name??$"PK_{tablename}", res.DbName);
            Assert.Equal(1, res.Items.Count());
            Assert.Equal("id", res.Items.First().ClrName);
        }

        public void build_null_if_fields_empty()
        {
            var target = new PrimaryKeyBuilder();

            var res = target.Build(new Mock<ITable>().Object, fb => new Mock<IField>().Object);

            Assert.Null(res);
        }
    }
}
