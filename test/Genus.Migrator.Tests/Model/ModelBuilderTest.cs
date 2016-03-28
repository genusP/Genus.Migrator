using Genus.Migrator.Model;
using Genus.Migrator.Model.Builder;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace Genus.Migrator.Tests.Model
{
    public class ModelBuilderTest
    {
        public static IEnumerable<object[]> GetItemsBuilders()
        {
            yield return new Func<ModelBuilder, object>[] { mb => mb.Table("testTable") };
            yield return new Func<ModelBuilder, object>[] { mb => mb.Table(typeof(object)) };
            yield return new Func<ModelBuilder, object>[] { mb => mb.Table<ModelBuilderTest>() };
            yield return new Func<ModelBuilder, object>[] { mb => mb.View("testView") };
            yield return new Func<ModelBuilder, object>[] { mb => mb.View<ModelBuilderTest>() };
            yield return new Func<ModelBuilder, object>[] { mb => mb.Function("testFunc") };
        }

        [Theory]
        [MemberData(nameof(GetItemsBuilders))]
        public void register_item_and_get_registred(Func<ModelBuilder,object> setup)
        {
            var target = new ModelBuilder();

            var item = setup(target);
            var item2 = setup(target);

            Assert.Equal(item, item2);
        }

        [Fact]
        public void block_register_view_and_table_wit_equal_name()
        {
            var target = new ModelBuilder();
            Action act1 = ()=>target.Table("table");
            Action act2 = ()=>target.View("table");

            act1();

            Assert.Throws<InvalidOperationException>(act2);
        }

        [Fact]
        public void build_model()
        {
            var target = new ModelBuilder();
            target.Table("table").Field("tf", DbType.AnsiString, 3);
            target.View("view").SetBodyScript( ProviderName.All, "SELECT 1 a");
            target.Function("func").SetScript(ProviderName.All, "CREATE Function a() as Select 1 a");

            var result = target.Build();

            Assert.NotNull(result);

            Assert.Equal(1, result.Tables.Count());
            Assert.Equal("table", result.Tables.First().ClrName);

            Assert.Equal(1, result.Views.Count());
            Assert.Equal("view", result.Views.First().ClrName);

            Assert.Equal(1, result.Functions.Count());
            Assert.Equal("func", result.Functions.First().ClrName);
        }

        [Fact]
        public void build_association()
        {
            var target = new ModelBuilder();
            target.Table("table1")
                .Fields(tb => tb.Field("id", DbType.Int32));
            target.Table("table2")
                .Fields(tb => tb.Field("fkid", DbType.Int32))
                .Association("fkid", "table1", "id");

            var res = target.Build();

            Assert.NotNull(res);
            var t2 = res.FindTable("table2");
            Assert.NotNull(t2);
            Assert.Equal(1, t2.Associations.Count());
        }
    }
}
