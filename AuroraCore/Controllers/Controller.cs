using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AuroraCore.Storage;

namespace AuroraCore.Controllers {
    public abstract class Controller {
        protected IStorageAdapter Storage { get; private set; }

        public static T Instantiate<T>(IStorageAdapter storage) where T : Controller, new() {
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
    }
}