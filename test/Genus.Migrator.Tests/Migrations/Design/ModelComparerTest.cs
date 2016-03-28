using Genus.Migrator.Migrations.Design.Internal;
using Genus.Migrator.Migrations.Operations;
using Genus.Migrator.Model;
using Genus.Migrator.Model.Builder;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Genus.Migrator.Tests.Migrations.Design
{
    public class ModelComparerTest
    {
        ModelComparer target = new ModelComparer();
        
        private IModel BuildTestModel(Action<ModelBuilder> setup =null)
        {
            var modelbuilder = new ModelBuilder();
            modelbuilder.Table("Blog", tb=> {
                tb.Field("Id", DbType.Int32).AsIdentity();
                tb.Field("Name", DbType.StringFixedLength, 50);
                tb.HasKey("Id").HasName("PK_Blog");
                });
            modelbuilder.Table("Posts", tb =>
            {
                tb.Field("Id", DbType.Int32).AsIdentity();
                tb.Field("Blog_Id", DbType.Int32);
                tb.Field("Text", DbType.String);
                tb.Field("CreateDate", DbType.DateTime).HasDefault(ProviderName.MicrosoftSqlServer, "getdate()");
                tb.HasKey("Id");
                tb.Association("Blog_Id", "Blog", "Id").AsForeignKey("FK_Post__Blog").OnDelete(ForeignKeyAction.CASCADE);
            }).HasSchema("dbo");
            if (setup != null)
                setup(modelbuilder);
            var model = modelbuilder.Build();
            return model;
        }

        [Fact]
        public void compare_without_source_model()
        {
            var result = target.CompareModel(null, BuildTestModel());

            Assert.NotEmpty(result);
        }

        [Fact]
        public void compare_without_target_model()
        {
            var result = target.CompareModel(BuildTestModel(), null);

            Assert.NotEmpty(result);
        }

        [Fact]
        public void create_table()
        {
            var modelbuilder = new ModelBuilder();
            modelbuilder.Table("test").Field("testField", DbType.Int32).IsPrimaryKey();
            var model = modelbuilder.Build();
            var model2 = BuildTestModel(mb => mb.Table("test").Field("testField", DbType.Int32).IsPrimaryKey());

            var res = target.CompareModel(null, model).ToArray();
            var res2 = target.CompareModel(BuildTestModel(), model2).ToArray();

            foreach (var result in new[] { res, res2})
            {
                Assert.NotNull(result);
                Assert.Equal(1, result.Count());
                var oper = Assert.IsType<CreateTable>(result.First());
                Assert.NotNull(oper);
                Assert.Null(oper.Schema);
                Assert.Equal("test", oper.TableName);
            }
        }

        [Fact]
        public void drop_table()
        {
            var modelbuilder = new ModelBuilder();
            modelbuilder.Table("test").HasSchema("dbo").Field("test", DbType.Int32);
            var model1 = modelbuilder.Build();
            var model2 = BuildTestModel(mb => mb.Table("test").HasSchema("dbo").Field("test"));

            var res = target.CompareModel(model1, null);
            var res2 = target.CompareModel(model2, BuildTestModel());

            foreach (var result in new[] { res, res2 })
            {
                Assert.NotNull(result);
                Assert.Equal(1, result.Count());
                var oper = Assert.IsType<DropTable>(result.First());
                Assert.NotNull(oper);
                Assert.Equal("dbo", oper.Schema);
                Assert.Equal("test", oper.TableName);
            }
        }
    }
}
