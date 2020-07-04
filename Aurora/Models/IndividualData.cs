using AuroraCore.Events;

namespace Aurora.Models {
    public class IndividualData {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public IndividualModelData Model { get; private set; }

        public IndividualData(Individual individual) {
            ID = individual.ID;
            Name = individual.Name;
            Model = new IndividualModelData(individual.Model, individual.Attributes);
        }
    }
}