using System.Linq;
using System.Collections.Generic;
using AuroraCore.Storage;
using System.Threading.Tasks;

namespace Aurora.Models {
    public class ModelData {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public int Parent { get; private set; }
        public IEnumerable<AttrData> Attributes { get; private set; }

        private ModelData() { }

        public static async Task<ModelData> Instantiate(IModel model) {
            var parent = await model.GetParent();
            var plainAttributes = await model.GetAllAttributes();
            var attributes = await Task.WhenAll(plainAttributes.Select(attr => AttrData.Instantiate(attr)));

            return new ModelData() {
                ID = model.ID,
                Name = model.Value,
                Parent = parent?.ID ?? 0,
                Attributes = attributes
            };
        }
    }
}