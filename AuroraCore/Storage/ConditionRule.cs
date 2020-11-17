// Aurora 
// Copyright (C) 2020  Frank Horrigan <https://github.com/saurer>

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;

namespace AuroraCore.Storage {
    public abstract class ConditionRule {
        public class EventConditionRule : ConditionRule {
            public int EventID { get; set; }

            public EventConditionRule() {

            }

            public EventConditionRule(int eventID) {
                EventID = eventID;
            }
        }

        public abstract class ComplexConditionRule : ConditionRule {
            public List<ConditionRule> Values { get; set; }

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
            public int PropertyID { get; set; }
            public string Value { get; set; }

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