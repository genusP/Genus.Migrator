using Genus.Migrator.Migrations;
using Genus.Migrator.Migrations.Operations;
using Genus.Migrator.Migrations.Operations.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Genus.Migrator.Tests.Migrations
{
    public class CreateTableBuilderTest
    {
        class entity
        {
            public int id { get; set; }
            public string field { get; set; }
        }

        [Fact]
        public void primarykey_required_argument()
        {
            var creatateTableOperation = new CreateTable();
            var target = new CreateTableBuilder(creatateTableOperation);

            Assert.Throws<ArgumentException>("name", () => target.PrimaryKey(null));
            Assert.Throws<ArgumentException>("name", () => target.PrimaryKey(" "));
            Assert.Throws<ArgumentException>("fields", () => target.PrimaryKey("PK_test"));
        }

        [Fact]
        public void primarykey()
        {
            var creatateTableOperation = new CreateTable();
            var target = new CreateTableBuilder(creatateTableOperation);

            var res = target.PrimaryKey("PK_test", "id");

            Assert.NotNull(res);
            Assert.NotNull(res.Operation);
            Assert.Equal("PK_test", res.Operation.PKName);
            Assert.Contains(nameof(entity.id), res.Operation.Fields);
        }
    }
}
