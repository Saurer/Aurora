using System.Collections.Generic;

namespace Aurora.Controllers {
    public sealed class AttrProperty {
        private List<string> values = new List<string>();

        public int ID { get; private set; }
        public string Name { get; private set; }
        public IEnumerable<string> Values {
            get {
                return values;
            }
        }

        public void AddValue(string value) {
            values.Add(value);
        }

        public AttrProperty(int id, string name) {
            ID = id;
            Name = name;
        }
    }
}