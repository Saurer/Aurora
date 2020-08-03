using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuroraCore.Storage {
    public interface IModelAttr : IEvent {
        Task<IAttr> GetAttribute();
        Task<IEnumerable<IEvent>> GetValueProperties();
        Task<bool> IsRequired();
        Task<int> GetCardinality();
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

        public async Task<bool> IsRequired() {
            var dbValue = await Context.Storage.GetModelAttributeValueProperty(BaseEventID, Int32.Parse(Value), StaticEvent.Required);
            if (null == dbValue) {
                return Const.DefaultRequired == 1;
            }
            else {
                return dbValue.Value == "1";
            }
        }

        public async Task<int> GetCardinality() {
            var dbValue = await Context.Storage.GetModelAttributeValueProperty(BaseEventID, Int32.Parse(Value), StaticEvent.Cardinality);
            if (null == dbValue) {
                return Const.DefaultCardinality;
            }
            else {
                return Int32.Parse(dbValue.Value);
            }
        }
    }
}