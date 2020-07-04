using System.Linq;
using System.Collections.Generic;
using AuroraCore.Events;

namespace Aurora.Models {
    public class ModelData {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public int Parent { get; private set; }
        public IEnumerable<AttrData> Attributes { get; private set; }

        public ModelData(Model model) {
            ID = model.ID;
            Name = model.Name;
            Parent = model.Parent?.ID ?? 0;
            Attributes = from a in model.GetAttributes() select new AttrData(a);
        }
    }
}