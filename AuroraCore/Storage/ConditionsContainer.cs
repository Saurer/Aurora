using System;
using System.Collections.Generic;
using System.Linq;

namespace AuroraCore.Storage {
    [Serializable]
    public class ConditionsContainer {
        public IEnumerable<ConditionRule> Values { get; private set; }

        public ConditionsContainer(IEnumerable<ConditionRule> values) {
            Values = values;
        }

        public T TryGet<T>() where T : ConditionRule {
            var value = Values.Where(c => c is T).SingleOrDefault();
            return value as T;
        }

        public ConditionRule.EventConditionRule TryGetEvent() {
            return TryGet<ConditionRule.EventConditionRule>();
        }
    }
}