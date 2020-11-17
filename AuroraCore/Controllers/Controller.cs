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

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AuroraCore.Storage;

namespace AuroraCore.Controllers {
    public abstract class Controller {
        protected IStorageAPI Storage { get; private set; }

        public static T Instantiate<T>(IStorageAPI storage) where T : Controller, new() {
            T value = new T();
            value.Storage = storage;
            return value;
        }

        public IEnumerable<ReactionBase> GetReactions() {
            var methods = this.GetType().GetMethods();
            foreach (var method in methods) {
                var attributes = method.GetCustomAttributes(typeof(EventReactionAttribute));
                if (attributes.Count() == 0) {
                    continue;
                }

                EventReactionAttribute reaction = (EventReactionAttribute)attributes.Single();
                yield return new ReactionBase(reaction.EventID, method);
            }

            yield break;
        }

        protected bool TryGetCondition<T>(IEventData e, out T value) where T : ConditionRule {
            if (e.Conditions is T) {
                value = e.Conditions as T;
                return true;
            }
            else {
                value = null;
                return false;
            }
        }

        protected IEnumerable<ConditionRule> TraverseConditions(ConditionRule stack) {
            if (stack is ConditionRule.ComplexConditionRule complex) {
                foreach (var rule in complex.Values) {
                    foreach (var subRule in TraverseConditions(rule)) {
                        yield return subRule;
                    }
                }
            }
            else {
                yield return stack;
            }
        }
    }
}