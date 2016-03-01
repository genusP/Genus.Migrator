namespace Genus.Migrator.Model
{
    public interface ITrigger:ISqlBody
    {
        string Schema { get; }
        string DbName { get; }
        IDbObject Target { get; }
        TriggerType TriggerType { get; }
        TriggerOpertaion Operations { get; }
    }
}