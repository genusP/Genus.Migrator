namespace Genus.Migrator.Model
{
    public interface IAssociation
    {
        IField Field { get; }
        string ForeignKeyName { get; }
        IField ReferenceField { get; }
        string DependentNavigation { get; }
        string PrincipalNavigation { get; }
        ForeignKeyAction? OnDeleteAction { get;}
        ForeignKeyAction? OnUpdateAction { get;}
    }
}