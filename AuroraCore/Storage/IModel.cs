using System.Threading.Tasks;

namespace AuroraCore.Storage {
    public interface IModel : IEvent {
        int ModelID { get; }
        int BaseEventID { get; }
        int ParentModelID { get; }
        string Label { get; }
        IPropertyProvider Properties { get; }

        Task<IModel> GetParentModel();
    }
}