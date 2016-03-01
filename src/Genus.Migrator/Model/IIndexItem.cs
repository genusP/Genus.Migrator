namespace Genus.Migrator.Model
{
    public interface IIndexItem
    {
        IField Field { get; }
        bool Decending { get; }
    }
}