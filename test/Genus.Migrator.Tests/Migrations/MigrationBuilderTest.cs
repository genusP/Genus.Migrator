using Genus.Migrator.Migrations;
using Genus.Migrator.Migrations.Operations;
using Genus.Migrator.Migrations.Operations.Builders;
using Genus.Migrator.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Genus.Migrator.Tests.Migrations
{
    public class MigrationBuilderTest
    {
        [Fact]
        public void required_arg()
        {
            var migrationBuilder = new MigrationBuilder();

            Assert.Throws<ArgumentException>("name", ()=> migrationBuilder.AddForeignKey(null, "table", "column", "principalTable", "principalField"));
            Assert.Throws<ArgumentException>("name", ()=> migrationBuilder.AddForeignKey(" ", "table", "column", "principalTable", "principalField"));
            Assert.Throws<ArgumentException>("table", ()=> migrationBuilder.AddForeignKey("name", null, "column", "principalTable", "principalField"));
            Assert.Throws<ArgumentException>("table", ()=> migrationBuilder.AddForeignKey("name", " ", "column", "principalTable", "principalField"));
            Assert.Throws<ArgumentException>("field", ()=> migrationBuilder.AddForeignKey("name", "table", null, "principalTable", "principalField"));
            Assert.Throws<ArgumentException>("field", ()=> migrationBuilder.AddForeignKey("name", "table", " ", "principalTable", "principalField"));
            Assert.Throws<ArgumentException>("principalTable", ()=> migrationBuilder.AddForeignKey("name", "table", "column", null, "principalField"));
            Assert.Throws<ArgumentException>("principalTable", ()=> migrationBuilder.AddForeignKey("name", "table", "column", " ", "principalField"));
            Assert.Throws<ArgumentException>("principalField", ()=> migrationBuilder.AddForeignKey("name", "table", "column", "principalTable", null));
            Assert.Throws<ArgumentException>("principalField", ()=> migrationBuilder.AddForeignKey("name", "table", "column", "principalTable", " "));

            Assert.Throws<ArgumentException>("name", ()=> migrationBuilder.CreateIndex(null, "table", new []{ "fields" }));
            Assert.Throws<ArgumentException>("name", ()=> migrationBuilder.CreateIndex(" ", "table", new []{ "fields" }));
            Assert.Throws<ArgumentException>("table", ()=> migrationBuilder.CreateIndex("name", null, new []{ "fields" }));
            Assert.Throws<ArgumentException>("table", ()=> migrationBuilder.CreateIndex("name", " ", new []{ "fields" }));
            Assert.Throws<ArgumentException>("fields", ()=> migrationBuilder.CreateIndex("name", "table", null));
            Assert.Throws<ArgumentException>("fields", ()=> migrationBuilder.CreateIndex("name", "table", new string[0]));
            Assert.Throws<ArgumentException>("name", ()=> migrationBuilder.CreateTable<object>(null, o=>o));
            Assert.Throws<ArgumentException>("name", () => migrationBuilder.CreateTable<object>(" ", o=>o));
            Assert.Throws<ArgumentNullException>("fields", () => migrationBuilder.CreateTable<object>("name",null));

            Assert.Throws<ArgumentException>("field", () => migrationBuilder.DropField(null, "table"));
            Assert.Throws<ArgumentException>("field", () => migrationBuilder.DropField(" ", "table"));
            Assert.Throws<ArgumentException>("table", () => migrationBuilder.DropField("name", null));
            Assert.Throws<ArgumentException>("table", () => migrationBuilder.DropField("name", " "));

            Assert.Throws<ArgumentException>("name", ()=> migrationBuilder.DropForeignKey(null, "table"));
            Assert.Throws<ArgumentException>("name", ()=> migrationBuilder.DropForeignKey(" ", "table"));
            Assert.Throws<ArgumentException>("table", ()=> migrationBuilder.DropForeignKey("name", null));
            Assert.Throws<ArgumentException>("table", ()=> migrationBuilder.DropForeignKey("name", " "));

            Assert.Throws<ArgumentException>("name", () => migrationBuilder.DropIndex(null, "table"));
            Assert.Throws<ArgumentException>("name", () => migrationBuilder.DropIndex(" ", "table"));
            Assert.Throws<ArgumentException>("table", () => migrationBuilder.DropIndex("name", null));
            Assert.Throws<ArgumentException>("table", () => migrationBuilder.DropIndex("name", " "));

            Assert.Throws<ArgumentException>("name", () => migrationBuilder.DropPrimaryKey(null, "table"));
            Assert.Throws<ArgumentException>("name", () => migrationBuilder.DropPrimaryKey(" ", "table"));
            Assert.Throws<ArgumentException>("table", () => migrationBuilder.DropPrimaryKey("name", null));
            Assert.Throws<ArgumentException>("table", () => migrationBuilder.DropPrimaryKey("name", " "));

            Assert.Throws<ArgumentException>("schema", () => migrationBuilder.DropSchema(null));
            Assert.Throws<ArgumentException>("schema", () => migrationBuilder.DropSchema(" "));

            Assert.Throws<ArgumentException>("name", () => migrationBuilder.DropTable(null));
            Assert.Throws<ArgumentException>("name", () => migrationBuilder.DropTable(" "));

            Assert.Throws<ArgumentException>("schema", () => migrationBuilder.EnsureSchema(null));
            Assert.Throws<ArgumentException>("schema", () => migrationBuilder.EnsureSchema(" "));

            Assert.Throws<ArgumentException>("sql", () => migrationBuilder.Sql(null));
            Assert.Throws<ArgumentException>("sql", () => migrationBuilder.Sql(" "));

            Assert.Throws<ArgumentException>("name", () => migrationBuilder.RenameTable(null, "newname"));
            Assert.Throws<ArgumentException>("name", () => migrationBuilder.RenameTable(" ", "newname"));
            Assert.Throws<ArgumentException>("newName", () => migrationBuilder.RenameTable("name", null));
            Assert.Throws<ArgumentException>("newName", () => migrationBuilder.RenameTable("name", " "));
            Assert.Throws<ArgumentException>("newSchema", () => migrationBuilder.RenameTable("name", "newname", "schema", null));
            Assert.Throws<ArgumentException>("schema", () => migrationBuilder.RenameTable("name", "newname", null, "newSchema"));

            Assert.Throws<ArgumentException>("name", () => migrationBuilder.RenameField(null, "newname", "table"));
            Assert.Throws<ArgumentException>("name", () => migrationBuilder.RenameField(" ", "newname", "table"));
            Assert.Throws<ArgumentException>("newName", () => migrationBuilder.RenameField("name", null, "table"));
            Assert.Throws<ArgumentException>("newName", () => migrationBuilder.RenameField("name", " ", "table"));
            Assert.Throws<ArgumentException>("table", () => migrationBuilder.RenameField("name", "newname", null));
            Assert.Throws<ArgumentException>("table", () => migrationBuilder.RenameField("name", "newname", " "));

            Assert.Throws<ArgumentException>("name", () => migrationBuilder.RenameIndex(null, "newname","table"));
            Assert.Throws<ArgumentException>("name", () => migrationBuilder.RenameIndex(" ", "newname", "table"));
            Assert.Throws<ArgumentException>("newName", () => migrationBuilder.RenameIndex("name", null, "table"));
            Assert.Throws<ArgumentException>("newName", () => migrationBuilder.RenameIndex("name", " ", "table"));
            Assert.Throws<ArgumentException>("table", () => migrationBuilder.RenameIndex("name", "newname", null));
            Assert.Throws<ArgumentException>("table", () => migrationBuilder.RenameIndex("name", "newname", " "));

            Assert.Throws<ArgumentException>("name", () => migrationBuilder.CreateView(null));
            Assert.Throws<ArgumentException>("name", () => migrationBuilder.CreateView(" "));

            Assert.Throws<ArgumentException>("name", () => migrationBuilder.CreateFunction(null));
            Assert.Throws<ArgumentException>("name", () => migrationBuilder.CreateFunction(" "));

            Assert.Throws<ArgumentException>("name", () => migrationBuilder.DropView(null));
            Assert.Throws<ArgumentException>("name", () => migrationBuilder.DropView(" "));

            Assert.Throws<ArgumentException>("name", () => migrationBuilder.DropFunction(null));
            Assert.Throws<ArgumentException>("name", () => migrationBuilder.DropFunction(" "));

            Assert.Throws<ArgumentException>("name", () => migrationBuilder.RenameFunction(null, "newname"));
            Assert.Throws<ArgumentException>("name", () => migrationBuilder.RenameFunction(" ", "newname"));
            Assert.Throws<ArgumentException>("newName", () => migrationBuilder.RenameFunction("name", null));
            Assert.Throws<ArgumentException>("newName", () => migrationBuilder.RenameFunction("name", " "));
            Assert.Throws<ArgumentException>("newSchema", () => migrationBuilder.RenameFunction("name", "newname", "schema", null));
            Assert.Throws<ArgumentException>("schema", () => migrationBuilder.RenameFunction("name", "newname", null, "newSchema"));

            Assert.Throws<ArgumentException>("name", () => migrationBuilder.RenameView(null, "newname"));
            Assert.Throws<ArgumentException>("name", () => migrationBuilder.RenameView(" ", "newname"));
            Assert.Throws<ArgumentException>("newName", () => migrationBuilder.RenameView("name", null));
            Assert.Throws<ArgumentException>("newName", () => migrationBuilder.RenameView("name", " "));
            Assert.Throws<ArgumentException>("newSchema", () => migrationBuilder.RenameView("name", "newname", "schema", null));
            Assert.Throws<ArgumentException>("schema", () => migrationBuilder.RenameView("name", "newname", null, "newSchema"));

        }

        [Fact]
        public void create_table()
        {
            var migrationBuilder = new MigrationBuilder();
            var field = new AddField();
            var pk = new AddPrimaryKey();

            var res = migrationBuilder.CreateTable(
                schema:"dbo",
                name: "test_table",
                fields: table => new{
                    Id = new FieldBuilder(field)},
                pk: t=>new OperationBuilder<AddPrimaryKey>(pk)
                );

            Assert.NotNull(res);
            Assert.NotNull(res.Operation);
            Assert.Equal("dbo", res.Operation.Schema);
            Assert.Equal("test_table", res.Operation.TableName);
            Assert.Equal(1, res.Operation.Fields.Count());
            Assert.Equal(field, res.Operation.Fields.First());
            Assert.Equal(pk, res.Operation.PrimaryKey);
            Assert.Empty(res.Operation.Annotations);
            Assert.Contains(res.Operation, migrationBuilder.Operations);
        }

        [Fact]
        public void drop_table()
        {
            var migrationBuilder = new MigrationBuilder();

            var res = migrationBuilder.DropTable("test_table", "dbo");

            Assert.NotNull(res);
            Assert.NotNull(res.Operation);
            Assert.Equal("test_table", res.Operation.TableName);
            Assert.Equal("dbo", res.Operation.Schema);
            Assert.Contains(res.Operation, migrationBuilder.Operations);
        }

        [Fact]
        public void add_foreign_key()
        {
            var migrationBuilder = new MigrationBuilder();

            var res = migrationBuilder.AddForeignKey(
                "name",
                "table",
                "field",
                "principalTable",
                "principalField",
                onDelete: ForeignKeyAction.SET_NULL,
                onUpdate: ForeignKeyAction.CASCADE,
                schema: "schema",
                principalSchema: "principalSchema"
                );
            Assert.NotNull(res);
            Assert.NotNull(res.Operation);
            Assert.Empty(res.Operation.Annotations);
            Assert.Equal("name", res.Operation.ForeignKeyName);
            Assert.Equal("table", res.Operation.TableName);
            Assert.Equal("field", res.Operation.FieldName);
            Assert.Equal("principalTable", res.Operation.PrincipalTable);
            Assert.Equal("principalField", res.Operation.PrincipalField);
            Assert.Equal(ForeignKeyAction.SET_NULL, res.Operation.OnDelete);
            Assert.Equal(ForeignKeyAction.CASCADE, res.Operation.OnUpdate);
            Assert.Equal("schema", res.Operation.Schema);
            Assert.Equal("principalSchema", res.Operation.PrincipalSchema);
            Assert.Contains(res.Operation, migrationBuilder.Operations);
        }

        [Fact]
        public void create_index()
        {
            var migrationBuilder = new MigrationBuilder();
            var fields = new[] { "field1", "-field2" };

            var res = migrationBuilder.CreateIndex(name: "name", table: "table", fields: fields, unique: true, tableSchema: "dbo");

            Assert.NotNull(res);
            Assert.NotNull(res.Operation);
            Assert.Equal("name", res.Operation.IndexName);
            Assert.Equal("table", res.Operation.TableName);
            Assert.Equal(fields, res.Operation.Fields);
            Assert.True(res.Operation.IsUnique);
            Assert.Equal("dbo", res.Operation.Schema);
            Assert.Contains(res.Operation, migrationBuilder.Operations);
        }

        [Fact]
        public void drop_index()
        {
            var migrationBuilder = new MigrationBuilder();

            var res = migrationBuilder.DropIndex(name: "name", table: "table", schema: "dbo");

            Assert.NotNull(res);
            Assert.NotNull(res.Operation);
            Assert.Empty(res.Operation.Annotations);
            Assert.Equal("name", res.Operation.IndexName);
            Assert.Equal("table", res.Operation.TableName);
            Assert.Equal("dbo", res.Operation.Schema);
            Assert.Contains(res.Operation, migrationBuilder.Operations);
        }
        

        [Fact]
        public void drop_primarykey()
        {
            var migrationBuilder = new MigrationBuilder();

            var res = migrationBuilder.DropPrimaryKey(name: "name", table: "table", schema: "dbo");

            Assert.NotNull(res);
            Assert.NotNull(res.Operation);
            Assert.Empty(res.Operation.Annotations);
            Assert.Equal("name", res.Operation.PrimaryKeyName);
            Assert.Equal("table", res.Operation.TableName);
            Assert.Equal("dbo", res.Operation.Schema);
            Assert.Contains(res.Operation, migrationBuilder.Operations);
        }


        [Fact]
        public void drop_field()
        {
            var migrationBuilder = new MigrationBuilder();

            var res = migrationBuilder.DropField(field: "field", table: "table", schema: "dbo");

            Assert.NotNull(res);
            Assert.NotNull(res.Operation);
            Assert.Empty(res.Operation.Annotations);
            Assert.Equal("field", res.Operation.FieldName);
            Assert.Equal("table", res.Operation.TableName);
            Assert.Equal("dbo", res.Operation.Schema);
            Assert.Contains(res.Operation, migrationBuilder.Operations);
        }


        [Fact]
        public void drop_schema()
        {
            var migrationBuilder = new MigrationBuilder();

            var res = migrationBuilder.DropSchema(schema: "dbo");

            Assert.NotNull(res);
            Assert.NotNull(res.Operation);
            Assert.Empty(res.Operation.Annotations);
            Assert.Equal("dbo", res.Operation.Schema);
            Assert.Contains(res.Operation, migrationBuilder.Operations);
        }


        [Fact]
        public void ensure_schema()
        {
            var migrationBuilder = new MigrationBuilder();

            var res = migrationBuilder.EnsureSchema(schema: "dbo");

            Assert.NotNull(res);
            Assert.NotNull(res.Operation);
            Assert.Empty(res.Operation.Annotations);
            Assert.Equal("dbo", res.Operation.Schema);
            Assert.Contains(res.Operation, migrationBuilder.Operations);
        }


        [Fact]
        public void rename_table()
        {
            var migrationBuilder = new MigrationBuilder();

            var res = migrationBuilder.RenameTable(name:"old_name", newName:"new_name", schema: "dbo", newSchema:"ndbo");

            Assert.NotNull(res);
            Assert.NotNull(res.Operation);
            Assert.Empty(res.Operation.Annotations);
            Assert.Equal("old_name", res.Operation.TableName);
            Assert.Equal("dbo", res.Operation.Schema);
            Assert.Equal("new_name", res.Operation.NewTableName);
            Assert.Equal("ndbo", res.Operation.NewSchema);
            Assert.Contains(res.Operation, migrationBuilder.Operations);
        }



        [Fact]
        public void rename_table_no_schema()
        {
            var migrationBuilder = new MigrationBuilder();

            var res = migrationBuilder.RenameTable(name: "old_name", newName: "new_name");

            Assert.NotNull(res);
            Assert.NotNull(res.Operation);
            Assert.Empty(res.Operation.Annotations);
            Assert.Equal("old_name", res.Operation.TableName);
            Assert.Null(res.Operation.Schema);
            Assert.Equal("new_name", res.Operation.NewTableName);
            Assert.Null(res.Operation.NewSchema);
            Assert.Contains(res.Operation, migrationBuilder.Operations);
        }


        [Fact]
        public void rename_field()
        {
            var migrationBuilder = new MigrationBuilder();

            var res = migrationBuilder.RenameField(name: "old_name", newName: "new_name", table:"table", schema: "dbo");

            Assert.NotNull(res);
            Assert.NotNull(res.Operation);
            Assert.Empty(res.Operation.Annotations);
            Assert.Equal("old_name", res.Operation.FieldName);
            Assert.Equal("new_name", res.Operation.NewFieldName);
            Assert.Equal("table", res.Operation.TableName);
            Assert.Equal("dbo", res.Operation.Schema);
            Assert.Contains(res.Operation, migrationBuilder.Operations);
        }


        [Fact]
        public void rename_index()
        {
            var migrationBuilder = new MigrationBuilder();

            var res = migrationBuilder.RenameIndex(name: "old_name", newName: "new_name", table:"table", schema: "dbo");

            Assert.NotNull(res);
            Assert.NotNull(res.Operation);
            Assert.Empty(res.Operation.Annotations);
            Assert.Equal("old_name", res.Operation.IndexName);
            Assert.Equal("new_name", res.Operation.NewIndexName);
            Assert.Equal("table", res.Operation.TableName);
            Assert.Equal("dbo", res.Operation.Schema);
            Assert.Contains(res.Operation, migrationBuilder.Operations);
        }



        [Fact]
        public void sql()
        {
            var migrationBuilder = new MigrationBuilder();

            var res = migrationBuilder.Sql("Select 1 a");

            Assert.NotNull(res);
            Assert.NotNull(res.Operation);
            Assert.Empty(res.Operation.Annotations);
            Assert.Equal("Select 1 a", res.Operation.Sql);
            Assert.Contains(res.Operation, migrationBuilder.Operations);
        }

        [Fact]
        public void create_view()
        {
            var migrationBuilder = new MigrationBuilder();

            var res = migrationBuilder.CreateView(name: "name", schema: "dbo");

            Assert.NotNull(res);
            Assert.NotNull(res.Operation);
            Assert.Equal("name", res.Operation.ViewName);
            Assert.Equal("dbo", res.Operation.Schema);
            Assert.Contains(res.Operation, migrationBuilder.Operations);
        }

        [Fact]
        public void drop_view()
        {
            var migrationBuilder = new MigrationBuilder();

            var res = migrationBuilder.DropView(name: "name", schema: "dbo");

            Assert.NotNull(res);
            Assert.NotNull(res.Operation);
            Assert.Equal("name", res.Operation.ViewName);
            Assert.Equal("dbo", res.Operation.Schema);
            Assert.Contains(res.Operation, migrationBuilder.Operations);
        }

        [Fact]
        public void rename_view()
        {
            var migrationBuilder = new MigrationBuilder();

            var res = migrationBuilder.RenameView(name: "old_name", newName: "new_name", schema: "dbo", newSchema: "ndbo");

            Assert.NotNull(res);
            Assert.NotNull(res.Operation);
            Assert.Empty(res.Operation.Annotations);
            Assert.Equal("old_name", res.Operation.ViewName);
            Assert.Equal("dbo", res.Operation.Schema);
            Assert.Equal("new_name", res.Operation.NewViewName);
            Assert.Equal("ndbo", res.Operation.NewSchema);
            Assert.Contains(res.Operation, migrationBuilder.Operations);
        }

        [Fact]
        public void create_function()
        {
            var migrationBuilder = new MigrationBuilder();

            var res = migrationBuilder.CreateFunction(name: "name", schema: "dbo");

            Assert.NotNull(res);
            Assert.NotNull(res.Operation);
            Assert.Equal("name", res.Operation.FunctionName);
            Assert.Equal("dbo", res.Operation.Schema);
            Assert.Contains(res.Operation, migrationBuilder.Operations);
        }

        [Fact]
        public void drop_function()
        {
            var migrationBuilder = new MigrationBuilder();

            var res = migrationBuilder.DropFunction(name: "name", schema: "dbo");

            Assert.NotNull(res);
            Assert.NotNull(res.Operation);
            Assert.Equal("name", res.Operation.FunctionName);
            Assert.Equal("dbo", res.Operation.Schema);
            Assert.Contains(res.Operation, migrationBuilder.Operations);
        }

        [Fact]
        public void rename_function()
        {
            var migrationBuilder = new MigrationBuilder();

            var res = migrationBuilder.RenameFunction(name: "old_name", newName: "new_name", schema: "dbo", newSchema: "ndbo");

            Assert.NotNull(res);
            Assert.NotNull(res.Operation);
            Assert.Empty(res.Operation.Annotations);
            Assert.Equal("old_name", res.Operation.FunctionName);
            Assert.Equal("dbo", res.Operation.Schema);
            Assert.Equal("new_name", res.Operation.NewFunctionName);
            Assert.Equal("ndbo", res.Operation.NewSchema);
            Assert.Contains(res.Operation, migrationBuilder.Operations);
        }
    }
}
