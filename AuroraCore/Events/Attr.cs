using System;
using System.Collections.Generic;
using AuroraCore.Storage;
using AuroraCore.Types;

namespace AuroraCore.Events {
    public interface IAttr : IEvent {
        IAttrModel Model { get; }
        DataType Type { get; }
        IReadOnlyDictionary<int, int> Properties { get; }
        int GetProperty(int id);
        bool Validate(string value);
    }

    internal sealed class Attr : Event, IAttr {
        private Dictionary<int, int> properties = new Dictionary<int, int>();

        public IAttrModel Model { get; private set; }
        public DataType Type { get; private set; }
        public IReadOnlyDictionary<int, int> Properties {
            get {
                return properties;
            }
        }

        public Attr(IEventData e, IAttrModel model) : base(e) {
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
            if (properties.Count != Model.Properties.Count) {
                return false;
            }

            return Type.Validate(value);
        }
    }
}