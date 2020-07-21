using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuroraCore.Storage {
    public interface IAttrModel : IModel {
        Task<IAttrProperty> GetProperty(int id);
        Task<IEnumerable<IAttrProperty>> GetProperties();
    }

    internal sealed class AttrModel : Model, IAttrModel {
        public AttrModel(IDataContext context, IEvent e) : base(context, e) {
        }

        public Task<IEnumerable<IAttrProperty>> GetProperties() {
            return Context.Storage.GetAttrProperties();
        }

        public Task<IAttrProperty> GetProperty(int id) {
            return Context.Storage.GetAttrProperty(id);
        }

        public new async Task<bool> Validate(IReadOnlyDictionary<int, string> values) {
            var modelValid = await base.Validate(values);

            if (!modelValid) {
                return false;
            }

            var properties = await GetProperties();
            foreach (var prop in properties) {
                if (!values.ContainsKey(prop.ID)) {
                    return false;
                }
            }

            return true;
        }
    }
}