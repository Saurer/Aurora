using System.Linq;
using System.Collections.Generic;
using AuroraCore.Events;

namespace Aurora.Models {
    public class IndividualModelData {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public int Parent { get; private set; }
        public IEnumerable<IndividualAttrData> Attributes { get; private set; }

        public IndividualModelData(Model model, IReadOnlyDictionary<int, string> values) {
            ID = model.ID;
            Name = model.Name;
            Parent = model?.Parent.ID ?? 0;
            Attributes =
                from a in model.GetAttributes()
                select new IndividualAttrData(a, values.ContainsKey(a.ID) ? values[a.ID] : null);
        }
    }
}