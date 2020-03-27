using System;

namespace Genus.Migrator.Model
{
    [Flags]
    public enum TriggerOperation
    {
        INSERT=1,
        UPDATE=2,
        DELETE=4,
        ALL=7
    }
}