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
    public class IndexBuilderTest
    {
        [Theory]
        [InlineData("a", new[] { "field"}, false)]
        [InlineData("a", new[] { "field", "field2"}, false)]
        public void build_index(string name, string[] fields, bool unique)
        {
            var tablebuilder = new TableBuilder();
            var target = new IndexBuilder();
            bool desc = false;
            foreach (var f in fields)
            {
                tablebuilder.Field(f).HasType(DbType.Int32);
                target.OnColumn(f, desc);
                desc = !desc;
            }
            if (unique)
                target.IsUnique();

            var res = target.Build(name, tablebuilder.Build("none", null));

            Assert.NotNull(res);
            Assert.Equal(name, res.DbName);
            Assert.Equal(fields.Length, res.Fields.Count());
            Assert.False(res.Fields.Select(_ => _.Field.ClrName).Except(fields).Any());
            Assert.Equal(unique, res.IsUnique);
        }

        [Fact]
        public void throw_if_fields_not_set()
        {
            var table = new Mock<ITable>().Object;
            var target = new IndexBuilder();

            Assert.Throws<InvalidOperationException>(()=>target.Build("name", table));
        }
    }
}
