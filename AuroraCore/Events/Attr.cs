using System;
using System.Collections.Generic;

namespace AuroraCore.Events {
    public sealed class Attr : Event {
        private Dictionary<int, int> properties = new Dictionary<int, int>();

        public string Name { get; private set; }

        public AttrModel Model { get; private set; }

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
    }
}