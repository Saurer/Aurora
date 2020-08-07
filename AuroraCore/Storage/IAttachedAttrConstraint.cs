using System.Threading.Tasks;

namespace AuroraCore.Storage {
    public interface IAttachedAttrConstraint : IEvent {
        int AttachmentID { get; }
        int ConstraintID { get; }
        int AttributeID { get; }
        int ValueID { get; }
        Task<IAttr> GetAttribute();
        Task<IIndividual> GetValue();
    }
}