using System.Linq;
using System.Collections.Generic;
using AuroraCore.Events;

namespace Aurora.Models {
    public class AttrData {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public string Type { get; private set; }
        public IEnumerable<AttrPropertyData> Properties { get; private set; }

        public AttrData(Attr attr) {
            ID = attr.ID;
            Name = attr.Name;
            Type = attr.Type.Name;
            Properties = from p in attr.Properties select new AttrPropertyData(p.Key, p.Value);
        }
    }
}