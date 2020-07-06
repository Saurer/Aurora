using AuroraCore.Events;

namespace Aurora.Models {
    public class IndividualData {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public IndividualModelData Model { get; private set; }

        public IndividualData(IIndividual individual) {
            ID = individual.ID;
            Name = individual.Value;
            Model = new IndividualModelData(individual.Model, individual.Attributes);
        }
    }
}