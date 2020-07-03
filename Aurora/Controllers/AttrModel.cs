using System;
using System.Collections.Generic;

namespace Aurora.Controllers {
    public sealed class AttrModel {
        private Dictionary<int, AttrProperty> properties = new Dictionary<int, AttrProperty>();

        public class AttrProperty {
            private Dictionary<int, string> values = new Dictionary<int, string>();

            public int ID { get; private set; }
            public string Name { get; private set; }

            public void RegisterValue(int id, string value) {
                values.Add(id, value);
            }

            public AttrProperty(int id, string name) {
                ID = id;
                Name = name;
            }
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

        public bool PropertyRegistered(int propertyID) {
            return properties.ContainsKey(propertyID);
        }
    }
}