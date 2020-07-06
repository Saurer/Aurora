using System.Collections.Generic;
using System.Linq;
using AuroraCore.Storage;

namespace AuroraCore.Events {
    public interface IModel : IEvent {
        IModel Parent { get; }
        IAttr GetAttribute(int id);
        IEnumerable<IAttr> GetAttributes();
        bool Validate(IReadOnlyDictionary<int, string> values);
    }

    internal class Model : Event, IModel {
        private Dictionary<int, IAttr> attributes = new Dictionary<int, IAttr>();

        public IModel Parent { get; private set; }

        public Model(IEventData e, IModel parent = null) : base(e) {
            Parent = parent;
        }

        public void AddAttribute(IAttr attr) {
            attributes.Add(attr.ID, attr);
        }

        public IAttr GetAttribute(int id) {
            if (attributes.TryGetValue(id, out var value)) {
                return value;
            }
            else if (null != Parent) {
                return Parent.GetAttribute(id);
            }
            else {
                return null;
            }
        }

        public IEnumerable<IAttr> GetAttributes() {
            foreach (var attrValue in attributes) {
                yield return attrValue.Value;
            }

            if (null != Parent) {
                var parent = (Model)Parent;
                foreach (var attrValue in parent.attributes) {
                    yield return attrValue.Value;
                }
            }
        }

        public bool Validate(IReadOnlyDictionary<int, string> values) {
            if (GetAttributes().Count() != values.Count) {
                return false;
            }

            foreach (var item in values) {
                var attr = GetAttribute(item.Key);
                if (!attr.Validate(item.Value)) {
                    return false;
                }
            }

            return true;
        }
    }
}