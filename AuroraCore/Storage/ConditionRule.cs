using System;
using System.Collections.Generic;

namespace AuroraCore.Storage {
    public abstract class ConditionRule {
        public class EventConditionRule : ConditionRule {
            public int EventID { get; private set; }

            public EventConditionRule() {

            }

            public EventConditionRule(int eventID) {
                EventID = eventID;
            }
        }

        public abstract class ComplexConditionRule : ConditionRule {
            public List<ConditionRule> Values { get; private set; }

            public ComplexConditionRule(IEnumerable<ConditionRule> values) {
                Values = new List<ConditionRule>(values);
            }
        }

        public class ConjunctionRule : ComplexConditionRule {
            public ConjunctionRule() : base(Array.Empty<ConditionRule>()) { }
            public ConjunctionRule(IEnumerable<ConditionRule> values) : base(values) {

            }
        }

        public class DisjunctionRule : ComplexConditionRule {
            public DisjunctionRule() : base(Array.Empty<ConditionRule>()) { }
            public DisjunctionRule(IEnumerable<ConditionRule> values) : base(values) {

            }
        }

        public abstract class PropertyValueRule : ConditionRule {
            public int PropertyID { get; private set; }
            public string Value { get; private set; }

            public PropertyValueRule(int propertyID, string value) {
                PropertyID = propertyID;
                Value = value;
            }
        }

        public class PropertyEqualityRule : PropertyValueRule {
            public PropertyEqualityRule() : base(default(int), null) { }
            public PropertyEqualityRule(int propertyID, string value) : base(propertyID, value) {

            }
        }

        public class PropertyInequalityRule : PropertyValueRule {
            public PropertyInequalityRule() : base(default(int), null) { }
            public PropertyInequalityRule(int propertyID, string value) : base(propertyID, value) {

            }
        }
    }
}