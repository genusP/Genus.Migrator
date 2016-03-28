using Genus.Migrator.Model;

namespace Genus.Migrator.Migrations.Design
{
    public interface ISnapshotCodeGenerator
    {
        string Extension { get; }
        void Generate(IModel model, IndentedStringBuilder builder);
    }
}