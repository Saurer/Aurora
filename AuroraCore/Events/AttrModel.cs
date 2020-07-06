using System;
using System.Collections.Generic;
using AuroraCore.Storage;

namespace AuroraCore.Events {
    public interface IAttrModel : IModel {
        IReadOnlyDictionary<int, IAttrProperty> Properties { get; }
        IAttrProperty GetProperty(int id);
    }

    internal sealed class AttrModel : Model, IAttrModel {
        private Dictionary<int, IAttrProperty> properties = new Dictionary<int, IAttrProperty>();

        public IReadOnlyDictionary<int, IAttrProperty> Properties {
            get {
                return properties;
            }
        }

        public AttrModel(IEventData e, IModel parent = null) : base(e, parent) {
        }

        public void RegisterProperty(IAttrProperty property) {
            properties.Add(property.ID, property);
        }

        public void RegisterValue(int propertyID, int valueID, string value) {
            if (properties.TryGetValue(propertyID, out var property)) {
                var prop = (AttrProperty)property;
                prop.RegisterValue(valueID, value);
            }
            else {
                throw new Exception("Property " + propertyID + " is not defined");
            }
        }

        public IAttrProperty GetProperty(int id) {
            if (properties.TryGetValue(id, out var property)) {
                return property;
            }
            else {
                throw new Exception("Property " + id + " is not defined");
            }
        }
    }
}