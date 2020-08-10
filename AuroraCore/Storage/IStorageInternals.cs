using System.Threading.Tasks;

namespace AuroraCore.Storage {
    public interface IStorageInternals {
        Task AddEvent(IEventData value);
        Task Rollback(int positionID);
        Task Prune();

        #region Effect
        Task<bool> IsEventAncestor(int ancestorID, int childID);
        Task AddSubEvent(int ancestorID, int childID);
        Task AddContainer(int containerID, int providerID);
        #endregion
    }
}