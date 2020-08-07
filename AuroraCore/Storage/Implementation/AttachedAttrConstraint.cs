using System;
using System.Threading.Tasks;

namespace AuroraCore.Storage.Implementation {
    internal sealed class AttachedAttrConstraint : Event, IAttachedAttrConstraint {
        private int valueID;

        public int AttachmentID => EventValue.ID;
        public int ConstraintID => EventValue.ValueID;
        public int AttributeID => EventValue.BaseEventID;
        public int ValueID => valueID;

        public AttachedAttrConstraint(IDataContext context, IEventData e) : base(context, e) {
            valueID = Int32.Parse(e.Value);
        }

        public async Task<IAttr> GetAttribute() =>
            await Context.Storage.GetAttribute(AttributeID);

        public async Task<IIndividual> GetValue() =>
            await Context.Storage.GetIndividual(ValueID);
    }
}