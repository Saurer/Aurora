using System.Threading.Tasks;

namespace AuroraCore.Storage {
    public interface IAttachedProperty<T> : IEvent where T : IProperty {
        int AttachmentID { get; }
        int ProviderID { get; }
        int PropertyID { get; }
        IPropertyProvider Properties { get; }

        Task<T> GetProperty();
        Task<bool> IsRequired();
        Task<int> GetCardinality();
    }
}