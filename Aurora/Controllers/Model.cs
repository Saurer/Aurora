using System;
using System.Collections.Generic;
using System.Linq;

namespace Aurora.Controllers {
    public class Model {
        private Dictionary<int, Attr> attributes = new Dictionary<int, Attr>();

        public int ID { get; private set; }
        public Model Parent { get; private set; }
        public string Name { get; private set; }

        public Model(int id, string name, Model parent = null) {
            ID = id;
            Name = name;
            Parent = parent;
        }

        public void AddAttribute(Attr attr) {
            attributes.Add(attr.ID, attr);
        }

        public Attr GetAttribute(int id) {
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

        public IEnumerable<Attr> GetAttributes() {
            foreach (var attrValue in attributes) {
                yield return attrValue.Value;
            }

            if (null != Parent) {
                foreach (var attrValue in Parent.attributes) {
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
                if (String.IsNullOrEmpty(attr.DefaultValue) && String.IsNullOrEmpty(item.Value)) {
                    return false;
                }
                // TODO: Type validation
            }

            return true;
        }
    }
}