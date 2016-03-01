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
    public class FunctionBuilderTest
    {
        public FunctionBuilderTest()
        {
            Model = new Mock<IModel>().Object;
        }

        private IModel Model { get; }

        public static IEnumerable<object[]> Building_TestData()
        {
            yield return new object[]
            {
                "testF1",
                new Dictionary<ProviderName, string> {
                    { ProviderName.MicrosoftSqlServer, "testF1"},
                    { ProviderName.All, "testF22"}
                }
            };
            yield return new object[]
            {
                "testF31",
                new Dictionary<ProviderName, string> {
                    { ProviderName.MicrosoftSqlServer, "test1"}
                }
            };
        }

        [Theory]
        [MemberData(nameof(Building_TestData))]
        public void build_function(string name, IDictionary<ProviderName, string> script)
        {
            var target = new FunctionBuilder();
            foreach (var item in script)
                target.SetScript(item.Key, item.Value);

            
            var res = target.Build(name, Model);

            Assert.NotNull(res);
            Assert.Equal(Model, res.Model);
            Assert.Equal(name, res.ClrName);
            Assert.Equal(script, res.Scripts);
        }

        [Fact]
        public void throw_build_if_script_empty()
        {
            var target = new FunctionBuilder();

            Assert.Throws<InvalidOperationException>(() => target.Build("test", Model));
        }
    }
}
