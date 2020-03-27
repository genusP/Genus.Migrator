namespace Genus.Migrator.Model
{
    public interface ITrigger:ISqlBody, IDbObject
    {
        IDbObject Target { get; }
        TriggerType TriggerType { get; }
        TriggerOperation Operations { get; }
    }
}