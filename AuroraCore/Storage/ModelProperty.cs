using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuroraCore.Storage {
    public interface IModelProperty<T> : IEvent {
        Task<T> GetProperty();
        Task<IEnumerable<IEvent>> GetValueProperties();
        Task<bool> IsRequired();
        Task<int> GetCardinality();
    }

    internal sealed class ModelProperty<T> : Event, IModelProperty<T> {
        public ModelProperty(IDataContext context, IEvent e) : base(context, e) {
        }

        public async Task<T> GetProperty() {
            if (typeof(T) == typeof(IAttr)) {
                var attr = await Context.Storage.GetAttribute(Int32.Parse(Value));
                return (T)attr;
            }
            else if (typeof(T) == typeof(IRelation)) {
                var relation = await Context.Storage.GetRelation(Int32.Parse(Value));
                return (T)relation;
            }
            else {
                return default(T);
            }
        }

        public async Task<IEnumerable<IEvent>> GetValueProperties() {
            return await Context.Storage.GetModelPropertyValueProperties(BaseEventID, Int32.Parse(Value));
        }

        public async Task<bool> IsRequired() {
            var dbValue = await Context.Storage.GetModelPropertyValueProperty(BaseEventID, Int32.Parse(Value), StaticEvent.Required);
            if (null == dbValue) {
                return Const.DefaultRequired == 1;
            }
            else {
                return dbValue.Value == "1";
            }
        }

        public async Task<int> GetCardinality() {
            var dbValue = await Context.Storage.GetModelPropertyValueProperty(BaseEventID, Int32.Parse(Value), StaticEvent.Cardinality);
            if (null == dbValue) {
                return Const.DefaultCardinality;
            }
            else {
                return Int32.Parse(dbValue.Value);
            }
        }
    }
}