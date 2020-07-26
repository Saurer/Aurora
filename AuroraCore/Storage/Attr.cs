using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AuroraCore.Types;

namespace AuroraCore.Storage {
    public interface IAttr : IEvent {
        Task<bool> IsBoxed();
        Task<DataType> GetDataType();
        Task<IAttrPropertyMember> GetProperty(int id);
        Task<IEnumerable<IAttrPropertyMember>> GetProperties();
        Task<IAttrModel> GetAttrModel();
        Task<IEvent> GetValue(int valueID);
        Task<IEnumerable<IEvent>> GetValues();
        Task<bool> Validate(string value);
    }

    internal sealed class Attr : Event, IAttr {
        public Attr(IDataContext context, IEvent e) : base(context, e) {
        }

        public async Task<bool> IsBoxed() {
            var dt = await GetDataType();
            return dt.IsBoxed;
        }

        public async Task<DataType> GetDataType() {
            var prop = await Context.Storage.GetAttrPropertyMember(ID, StaticEvent.DataType);
            int valueID;

            if (null == prop) {
                return null;
            }

            if (!Int32.TryParse(prop.Value, out valueID)) {
                return null;
            }

            var propValue = await Context.Storage.GetIndividual(valueID);

            if (null == propValue) {
                return null;
            }

            return Context.Types.Get(propValue.Value);
        }

        public Task<IAttrModel> GetAttrModel() {
            return Context.Storage.GetAttrModel();
        }

        public Task<IEnumerable<IAttrPropertyMember>> GetProperties() {
            return Context.Storage.GetAttrPropertyMembers(ID);
        }

        public Task<IAttrPropertyMember> GetProperty(int id) {
            return Context.Storage.GetAttrPropertyMember(ID, id);
        }

        public Task<IEvent> GetValue(int valueID) {
            return Context.Storage.GetAttrValue(ID, valueID);
        }

        public Task<IEnumerable<IEvent>> GetValues() {
            return Context.Storage.GetAttrValues(ID);
        }

        public async Task<bool> Validate(string value) {
            var dataType = await GetDataType();
            return dataType.Validate(value);
        }
    }
}