using Genus.Migrator.Model.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Migrator.Model.Builder
{
    public class TriggerBuilder: DbObjectWithScriptsBuildercs<TriggerBuilder>
    {
        private TriggerOperation _operations = TriggerOperation.ALL;
        private TriggerType _triggerType = TriggerType.AFTER;

        public TriggerBuilder ForOperations(TriggerOperation operations)
        {
            _operations = operations;
            return this;
        }

        public TriggerBuilder AsAfter()
        {
            _triggerType = TriggerType.AFTER;
            return this;
        }

        public TriggerBuilder AsInsteadOf()
        {
            _triggerType = TriggerType.INSTEAD_OF;
            return this;
        }

        internal ITrigger Build(string schema, string name, IDbObject target)
        {
            var trigger = new Trigger(schema, name, target)
            {
                SqlBody = Scripts,
                With = Withs,
                Operations = _operations,
                TriggerType = _triggerType
            };
            return trigger;
        }
    }
}
