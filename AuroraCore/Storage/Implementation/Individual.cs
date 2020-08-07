using System.Threading.Tasks;

namespace AuroraCore.Storage.Implementation {
    internal class Individual : Event, IIndividual {
        public int IndividualID => EventValue.ID;
        public int EventBase => EventValue.BaseEventID;
        public int ModelID => EventValue.ConditionEventID;
        public string Label => EventValue.Value;
        public IPropertyContainer Properties { get; private set; }

        public Individual(IDataContext context, IEventData e) : base(context, e) {
            Properties = new PropertyContainer(context, ModelID, IndividualID);
        }

        public async Task<IModel> GetModel() =>
            await Context.Storage.GetModel(ModelID);
    }
}