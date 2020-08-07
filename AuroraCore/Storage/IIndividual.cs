using System.Threading.Tasks;

namespace AuroraCore.Storage {
    public interface IIndividual : IEvent {
        int IndividualID { get; }
        int EventBase { get; }
        int ModelID { get; }
        string Label { get; }
        IPropertyContainer Properties { get; }
        Task<IModel> GetModel();
    }
}