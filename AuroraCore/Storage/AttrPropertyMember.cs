using System;
using System.Threading.Tasks;

namespace AuroraCore.Storage {
    public interface IAttrPropertyMember : IEvent {
        Task<IAttr> GetAttr();
        Task<IIndividual> GetValue();
    }

    internal sealed class AttrPropertyValue : Event, IAttrPropertyMember {
        private int valueID;

        public AttrPropertyValue(IDataContext context, IEvent e) : base(context, e) {
            valueID = Int32.Parse(e.Value);
        }

        public Task<IAttr> GetAttr() {
            return Context.Storage.GetAttribute(BaseEventID);
        }

        public Task<IIndividual> GetValue() {
            return Context.Storage.GetIndividual(valueID);
        }
    }
}