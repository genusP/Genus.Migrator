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
    public class AssociationBuilderTest
    {

        [Theory]
        [InlineData(null, null, null, null, null)]
        [InlineData("depNav", null, "fk_table1", ForeignKeyAction.CASCADE, null)]
        [InlineData(null, "principalNav", "fk_table2", ForeignKeyAction.SET_NULL, null)]
        public void build_association(string depNav, string prinNav, string fkName, ForeignKeyAction? ondel, ForeignKeyAction? onupd)
        {
            var target = new AssociationBuilder();
            var f1 = new Mock<IField>().Object;
            var f2 = new Mock<IField>().Object;

            if (depNav != null || prinNav != null)
                target.WithNavigation(depNav, prinNav);
            if (fkName != null)
                target.AsForeignKey(fkName);
            if (ondel.HasValue)
                target.OnDelete(ondel.Value);
            if (onupd.HasValue)
                target.OnUpdate(onupd.Value);

            var res = target.Build(f1, f2);

            Assert.NotNull(res);
            Assert.Equal(depNav, res.DependentNavigation);
            Assert.Equal(f1, res.Field);
            Assert.Equal(fkName, res.ForeignKeyName);
            Assert.Equal(ondel, res.OnDeleteAction);
            Assert.Equal(onupd, res.OnUpdateAction);
            Assert.Equal(f2, res.ReferenceField);
        }
    }
}
