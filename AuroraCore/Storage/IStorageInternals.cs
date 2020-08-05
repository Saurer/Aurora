using System.Threading.Tasks;

namespace AuroraCore.Storage {
    public interface IStorageInternals {
        Task AddEvent(IEvent value);
        Task Rollback(int positionID);
        Task Prune();
    }
}