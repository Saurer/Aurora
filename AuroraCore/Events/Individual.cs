using System;
using System.Collections.Generic;
using AuroraCore.Storage;

namespace AuroraCore.Events {
    public interface IIndividual : IEvent {
        IModel Model { get; }
        IReadOnlyDictionary<int, string> Attributes { get; }
    }

    internal class Individual : Event, IIndividual {
        private Dictionary<int, string> attributes = new Dictionary<int, string>();

        public IModel Model { get; private set; }
        public IReadOnlyDictionary<int, string> Attributes {
            get {
                return attributes;
            }
        }

        public Individual(IEventData e, IModel model) : base(e) {
            Model = model;
        }

        public void SetAttribute(int attributeID, string value) {
            var attribute = Model.GetAttribute(attributeID);
            if (null == attribute) {
                throw new Exception("Attribute " + attributeID + " does not exist");
            }
            else if (!attribute.Validate(value)) {
                throw new Exception("Attribute " + attribute.Value + " is invalid");
            }
            else {
                attributes.Add(attributeID, value);
            }

            bool valid = Model.Validate(attributes);
        }
    }
}