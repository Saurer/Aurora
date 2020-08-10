using System;
using System.Threading.Tasks;

namespace AuroraCore.Storage.Implementation {
    internal sealed class AttachedProperty<T> : Event, IAttachedProperty<T> where T : IProperty {
        private int propertyID;

        public int AttachmentID => EventValue.ID;
        public int ProviderID => EventValue.BaseEventID;
        public int PropertyID => propertyID;
        public IPropertyProvider Properties { get; private set; }

        public AttachedProperty(IDataContext context, IEventData e) : base(context, e) {
            propertyID = Int32.Parse(e.Value);
            Properties = new PropertyProvider(context, AttachmentID);
        }

        public async Task<T> GetProperty() {
            if (typeof(T) == typeof(IAttr)) {
                var attr = await Context.Storage.GetAttribute(PropertyID);
                return (T)attr;
            }
            else if (typeof(T) == typeof(IRelation)) {
                var relation = await Context.Storage.GetRelation(PropertyID);
                return (T)relation;
            }
            else {
                throw new NotImplementedException();
            }
        }

        public async Task<bool> IsRequired() {
            var constraint = await Context.Storage.GetPropertyProviderValueConstraint(ProviderID, PropertyID, StaticEvent.Required);
            if (null == constraint) {
                return Const.DefaultRequired;
            }
            else {
                return constraint.EventValue.Value == "1";
            }
        }

        public async Task<int> GetCardinality() {
            var constraint = await Context.Storage.GetPropertyProviderValueConstraint(ProviderID, PropertyID, StaticEvent.Cardinality);
            if (null == constraint) {
                return Const.DefaultCardinality;
            }
            else {
                return Int32.Parse(constraint.EventValue.Value);
            }
        }
    }
}