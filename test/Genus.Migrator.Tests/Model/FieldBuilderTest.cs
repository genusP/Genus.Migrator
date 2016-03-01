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
    public class FieldBuilderTest
    {
        public static IEnumerable<object[]> BuildFiledArguments()
        {
            yield return new object[]
            {
                "name1",
                DbType.AnsiString,
                150,
                true,
                false,
                new Dictionary<ProviderName, string>(),
                new Dictionary<ProviderName, string>()
                {
                    {ProviderName.MicrosoftSqlServer, "SQL_Latin" }
                }
            };

            yield return new object[]
            {
                "name2",
                DbType.Int32,
                -1,
                false,
                true,
                new Dictionary<ProviderName, string>(),
                new Dictionary<ProviderName, string>()
            };

            yield return new object[]
            {
                "name3",
                DbType.Guid,
                -1,
                false,
                false,
                new Dictionary<ProviderName, string>
                {
                    {ProviderName.MicrosoftSqlServer, "newid()" }
                },
                new Dictionary<ProviderName, string>()
            };
        }

        [Theory]
        [MemberData(nameof(BuildFiledArguments))]
        public void build_field( string name, DbType type, int? len, bool nullable, bool identity, IDictionary<ProviderName, string> defaults, IDictionary<ProviderName, string> collation)
        {
            var tableBuilder = new TableBuilder();
            var table = new Mock<ITable>().Object;
            var target = new FieldBuilder(tableBuilder);
            if (name != null)
                target.HasName(name);
            target.HasType(type, len??-1);
            if (nullable)
                target.AsNullable();
            if (identity)
                target.AsIdentity();
            foreach (var d in defaults)
                target.HasDefault(d.Key, d.Value);
            foreach (var c in collation)
                target.HasCollation(c.Key, c.Value);

            var res = target.Build("test", table);

            Assert.NotNull(res);
            Assert.Equal("test", res.ClrName);
            Assert.Equal(collation, res.Collation);
            Assert.Equal(type, res.DataType);
            Assert.Equal(name, res.DbName);
            Assert.Equal(defaults, res.Default);
            Assert.Equal(identity, res.IsIdentity);
            Assert.Equal(nullable, res.IsNullable);
            Assert.Equal(len ?? -1, res.Length);
            Assert.Equal(table, res.Table);
        }
    }
}
