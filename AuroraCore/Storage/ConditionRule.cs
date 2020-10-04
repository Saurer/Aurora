using System;
using System.Collections.Generic;

namespace AuroraCore.Storage {
    public abstract class ConditionRule {
        public class EventConditionRule : ConditionRule {
            public int EventID { get; private set; }

            public EventConditionRule(int eventID) {
                EventID = eventID;
            }
        }

        public abstract class ComplexConditionRule : ConditionRule {
            public IEnumerable<ConditionRule> Values { get; private set; }

            public ComplexConditionRule(IEnumerable<ConditionRule> values) {
                Values = values;
            }
        }

        public class ConjunctionRule : ComplexConditionRule {
            public ConjunctionRule(IEnumerable<ConditionRule> values) : base(values) {

            }
        }

        public class DisjunctionRule : ComplexConditionRule {
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
            public PropertyEqualityRule(int propertyID, string value) : base(propertyID, value) {

            }
        }

        public class PropertyInequalityRule : PropertyValueRule {
            public PropertyInequalityRule(int propertyID, string value) : base(propertyID, value) {

            }
        }
    }
}