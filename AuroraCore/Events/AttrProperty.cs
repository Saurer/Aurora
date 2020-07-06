using System.Collections.Generic;

namespace AuroraCore.Events {
    public interface IAttrProperty {
        int ID { get; }
        string Name { get; }
        IReadOnlyDictionary<int, string> Values { get; }
        bool ContainsValue(int id);
    }

    internal class AttrProperty : IAttrProperty {
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
}