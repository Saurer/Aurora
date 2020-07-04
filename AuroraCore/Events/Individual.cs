using System;
using System.Collections.Generic;
using AuroraCore.Storage;

namespace AuroraCore.Events {
    public class Individual : Event {
        private Dictionary<int, string> attributes = new Dictionary<int, string>();

        public string Name { get; private set; }
        public Model Model { get; private set; }
        public IReadOnlyDictionary<int, string> Attributes {
            get {
                return attributes;
            }
        }

        public Individual(int id, string name, Model model) : base(id) {
            Name = name;
            Model = model;
        }

        public void SetAttribute(IEventData e) {
            Attr attribute = Model.GetAttribute(e.ValueID);
            if (null == attribute) {
                throw new Exception("Attribute " + e.ValueID + " does not exist");
            }
            else if (!attribute.Validate(e.Value)) {
                throw new Exception("Attribute " + attribute.Name + " is invalid");
            }
            else {
                attributes.Add(e.ValueID, e.Value);
            }

            bool valid = Model.Validate(attributes);
            Console.WriteLine("Individual [{0}]{1} is {2}", ID, Name, valid ? "valid" : "invalid");
        }
    }
}