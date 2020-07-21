using System.Threading.Tasks;
using AuroraCore.Storage;

namespace Aurora.Models {
    public class IndividualData {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public IndividualModelData Model { get; private set; }

        private IndividualData() {

        }

        public static async Task<IndividualData> Instantiate(IIndividual individual) {
            var plainModel = await individual.GetModel();
            var attributes = await individual.GetAttributes();
            var model = await IndividualModelData.Instantiate(plainModel, attributes);

            return new IndividualData {
                ID = individual.ID,
                Name = individual.Value,
                Model = model
            };
        }
    }
}