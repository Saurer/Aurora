using System;
using System.Collections.Generic;
using AuroraCore.Storage;

namespace Aurora.Controllers {
    public class Individual {
        private Dictionary<int, string> attributes = new Dictionary<int, string>();

        public int ID { get; private set; }
        public string Name { get; private set; }
        public Model Model { get; private set; }

        public Individual(IEventData e, Model model) {
            ID = e.ID;
            Name = e.Value;
            Model = model;
        }

        public void SetAttribute(IEventData e) {
            Attr attribute = Model.GetAttribute(e.ValueID);
            if (null == attribute) {
                throw new Exception("Attribute " + e.ValueID + " does not exist");
            }
            else {
                attributes.Add(e.ValueID, e.Value);
            }

            bool valid = Model.Validate(attributes);
            Console.WriteLine("Individual [{0}]{1} is {2}", ID, Name, valid ? "valid" : "invalid");
        }
    }
}