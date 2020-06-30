using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuroraCore.Storage {
    public interface IStorageAdapter {
        Task<IEventData> GetEvent(long id);
        Task<IEnumerable<IEventData>> GetEvents();
        Task<bool> AddEvent(IEventData value);
        Task<bool> UpdateEvent(IEventData value);
    }
}