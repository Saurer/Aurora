using System;
using System.Collections.Generic;
using AuroraCore.Types;

namespace AuroraCore.Events {
    public sealed class Attr : Event {
        private Dictionary<int, int> properties = new Dictionary<int, int>();

        public string Name { get; private set; }
        public AttrModel Model { get; private set; }
        public DataType Type { get; private set; }

        public Attr(int id, string name, AttrModel model) : base(id) {
            Name = name;
            Model = model;
        }

        public void SetProperty(int id, int valueID) {
            var property = Model.GetProperty(id);
            if (property.ContainsValue(valueID)) {
                properties.Add(id, valueID);
            }
            else {
                throw new Exception("Value " + valueID + " does not exist");
            }
        }

        public void SetDataType(int id, int valueID, DataType type) {
            SetProperty(id, valueID);
            Type = type;
        }

        public int GetProperty(int id) {
            if (properties.TryGetValue(id, out var value)) {
                return value;
            }
            else {
                throw new Exception("Property" + id + " does not exist");
            }
        }

        public bool Validate(string value) {
            if (properties.Count != Model.PropertyCount) {
                return false;
            }

            return Type.Validate(value);
        }
    }
}