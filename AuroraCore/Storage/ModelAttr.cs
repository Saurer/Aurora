using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuroraCore.Storage {
    public interface IModelAttr : IEvent {
        Task<IAttr> GetAttribute();
        Task<IEnumerable<IEvent>> GetValueProperties();
    }

    internal sealed class ModelAttr : Event, IModelAttr {
        public ModelAttr(IDataContext context, IEvent e) : base(context, e) {
        }

        public async Task<IAttr> GetAttribute() {
            return await Context.Storage.GetAttribute(Int32.Parse(Value));
        }

        public async Task<IEnumerable<IEvent>> GetValueProperties() {
            return await Context.Storage.GetModelAttributeValueProperties(BaseEventID, Int32.Parse(Value));
        }
    }
}