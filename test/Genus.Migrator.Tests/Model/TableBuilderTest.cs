using Genus.Migrator.Model;
using Genus.Migrator.Model.Builder;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Genus.Migrator.Tests.Model
{
    public class TableBuilderTest
    {
        public TableBuilderTest()
        {
            Model = new Mock<IModel>().Object;
        }

        private IModel Model { get; }

        public static IEnumerable<object[]> GetItemsBuilders()
        {
            yield return new Func<TableBuilder, object>[] { tb => tb.Field("testField") };
            yield return new Func<TableBuilder, object>[] { tb => tb.Index("testIndex") };
            yield return new Func<TableBuilder, object>[] { tb => tb.Association("testA2", "asas", "pppp") };
        }

        [Theory]
        [MemberData(nameof(GetItemsBuilders))]
        public void register_item_and_get_registred(Func<TableBuilder, object> setup)
        {
            var target = new TableBuilder();

            var item = setup(target);
            var item2 = setup(target);

            Assert.Equal(item, item2);
        }

        public static IEnumerable<object[]> Building_TestData()
        {
            yield return new object[]
            {
                null,
                "test1",
                new[] { "f1", "f2"},
                new Dictionary<ProviderName, string>()
            };
            yield return new object[]
            {
                "sc",
                "test1",
                new []{"test1"},
                new Dictionary<ProviderName, string>()
            };
            yield return new object[]
            {
                null,
                null,
                new [] { "test22"},
                new Dictionary<ProviderName, string> {
                    { ProviderName.MicrosoftSqlServer, "Wthtest1"} }
            };
        }

        [Theory]
        [MemberData(nameof(Building_TestData))]
        public void build_table(string schema, string name, string[] fields,  IDictionary<ProviderName, string> with)
        {
            var target = new TableBuilder();
            if (schema != null)
                target.HasSchema(schema);
            if(name!=null)
                target.HasName(name);
            if (fields != null) {
                foreach (var t in fields)
                    target.Field(t).HasType(System.Data.DbType.Int32);
                target.Field(fields.First()).IsPrimaryKey();
            }

            target.Index("asd").OnColumn(fields.Last());

            target.Association("aassds", "iiiii", "id");

            if(with!=null)
                foreach (var item in with)
                    target.With(item.Key, item.Value);

            var res = target.Build("test", Model);

            Assert.NotNull(res);
            Assert.Equal(Model, res.Model);
            Assert.Equal("test", res.ClrName);
            Assert.Equal(name??"test", res.DbName);
            Assert.Equal(schema, res.Schema);
            Assert.Equal(fields.Length, res.Fields.Count());
            Assert.Equal(with, res.With);
        }

        [Fact]
        public void throw_build_if_fields_empty()
        {
            var target = new TableBuilder();
            Assert.Throws<InvalidOperationException>(() => target.Build("test", Model));
        }
    }
}
