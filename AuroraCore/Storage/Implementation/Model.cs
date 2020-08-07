using System.Threading.Tasks;

namespace AuroraCore.Storage.Implementation {
    internal class Model : Event, IModel {
        public int ModelID => EventValue.ID;
        public int BaseEventID => EventValue.BaseEventID;
        public int ParentModelID => EventValue.ConditionEventID;
        public string Label => EventValue.Value;
        public IPropertyProvider Properties { get; private set; }

        public Model(IDataContext context, IEventData e) : base(context, e) {
            Properties = new ModelPropertyProvider(context, this);
        }

        public async Task<IModel> GetParentModel() =>
            StaticEvent.Event == ParentModelID ?
                null :
                await Context.Storage.GetModel(ParentModelID);
    }
}