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
    public class ViewBuilderTest
    {
        public ViewBuilderTest()
        {
            Model = new Mock<IModel>().Object;
        }

        private IModel Model { get; }

        public static IEnumerable<object[]> Building_TestData()
        {
            yield return new object[]
            {
                null,
                "test1",
                new Dictionary<ProviderName, string> {
                    { ProviderName.MicrosoftSqlServer, "test1"},
                    { ProviderName.All, "test22"}
                },
                new Dictionary<ProviderName, string>()
            };
            yield return new object[]
            {
                "sc",
                "test1",
                new Dictionary<ProviderName, string> {
                    { ProviderName.MicrosoftSqlServer, "test1"},
                    { ProviderName.All, "test22"}
                },
                new Dictionary<ProviderName, string>()
            };
            yield return new object[]
            {
                null,
                null,
                new Dictionary<ProviderName, string> {
                    { ProviderName.All, "test22"}
                },
                new Dictionary<ProviderName, string> {
                    { ProviderName.MicrosoftSqlServer, "Wthtest1"} }
            };
        }

        [Theory]
        [MemberData(nameof(Building_TestData))]
        public void build_view(string schema, string name, IDictionary<ProviderName, string> script, IDictionary<ProviderName, string> with)
        {
            var target = new ViewBuilder();
            if (schema != null)
                target.HasSchema(schema);
            if(name!=null)
                target.HasName(name);
            if(script!=null)
                foreach (var item in script)
                    target.SetBodyScript(item.Key, item.Value);

            if(with!=null)
                foreach (var item in with)
                    target.With(item.Key, item.Value);

            var res = target.Build("test", Model);

            Assert.NotNull(res);
            Assert.Equal(Model, res.Model);
            Assert.Equal("test", res.ClrName);
            Assert.Equal(name??"test", res.DbName);
            Assert.Equal(schema, res.Schema);
            Assert.Equal(script, res.SqlBody);
            Assert.Equal(with, res.With);
        }

        [Fact]
        public void throw_build_if_script_empty()
        {
            var target = new ViewBuilder();
            target.HasName("adsasd");

            Assert.Throws<InvalidOperationException>(() => target.Build("test", Model));
        }
    }
}
