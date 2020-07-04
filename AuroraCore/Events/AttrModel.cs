using System;
using System.Collections.Generic;

namespace AuroraCore.Events {
    public sealed class AttrModel : Model {
        private Dictionary<int, AttrProperty> properties = new Dictionary<int, AttrProperty>();

        public class AttrProperty {
            private Dictionary<int, string> values = new Dictionary<int, string>();

            public int ID { get; private set; }
            public string Name { get; private set; }
            public IReadOnlyDictionary<int, string> Values {
                get {
                    return values;
                }
            }

            public void RegisterValue(int id, string value) {
                values.Add(id, value);
            }

            public bool ContainsValue(int id) {
                return values.ContainsKey(id);
            }

            public AttrProperty(int id, string name) {
                ID = id;
                Name = name;
            }
        }

        public AttrModel(int id, string name, Model parent = null) : base(id, name, parent) {
        }

        public void RegisterProperty(int id, string name) {
            properties.Add(id, new AttrProperty(id, name));
        }

        public void RegisterValue(int propertyID, int valueID, string value) {
            if (properties.TryGetValue(propertyID, out var property)) {
                property.RegisterValue(valueID, value);
            }
            else {
                throw new Exception("Property " + propertyID + " is not defined");
            }
        }

        public AttrProperty GetProperty(int id) {
            if (properties.TryGetValue(id, out var property)) {
                return property;
            }
            else {
                throw new Exception("Property " + id + " is not defined");
            }
        }
    }
}