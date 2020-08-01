using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuroraCore.Storage {
    public interface IModel : IEvent {
        Task<IEvent> GetBaseEvent();
        Task<IModel> GetParent();
        Task<IModelAttr> GetAttribute(int id);
        Task<IEnumerable<IModelAttr>> GetOwnAttributes();
        Task<IEnumerable<IModelAttr>> GetAllAttributes();
        Task<bool> Validate(IReadOnlyDictionary<int, string> values);
    }

    internal class Model : Event, IModel {
        public Model(IDataContext context, IEvent e) : base(context, e) {
        }

        public async Task<IEvent> GetBaseEvent() {
            return await Context.Storage.GetEvent(BaseEventID);
        }

        public async Task<IModelAttr> GetAttribute(int attrID) {
            var attribute = await Context.Storage.GetModelAttribute(ID, attrID);
            return attribute;
        }

        public Task<IEnumerable<IModelAttr>> GetOwnAttributes() {
            return Context.Storage.GetModelAttributes(ID);
        }

        public async Task<IEnumerable<IModelAttr>> GetAllAttributes() {
            var queue = new Queue<IModel>(new[] { this });
            var result = new List<IModelAttr>();

            while (queue.Count > 0) {
                var model = queue.Dequeue();
                var attributes = await model.GetOwnAttributes();
                result.AddRange(attributes);

                var parent = await model.GetParent();
                if (null != parent) {
                    queue.Enqueue(parent);
                }
            }

            return result;
        }

        public async Task<IModel> GetParent() {
            if (ConditionEventID == StaticEvent.Event) {
                return null;
            }
            else {
                return await Context.Storage.GetModel(ConditionEventID);
            }
        }

        public async Task<bool> Validate(IReadOnlyDictionary<int, string> values) {
            var attributes = await GetAllAttributes();

            foreach (var modelAttr in attributes) {
                var attrProperties = await modelAttr.GetValueProperties();
                var requiredValue = attrProperties.Where(prop => prop.ValueID == StaticEvent.Required).SingleOrDefault()?.Value;
                var attr = await modelAttr.GetAttribute();

                // Value is not required
                if (null == requiredValue ? Const.DefaultRequired == 0 : requiredValue == "0") {
                    continue;
                }

                if (values.TryGetValue(attr.ID, out var value)) {
                    var valid = await attr.Validate(value);
                    if (!valid) {
                        return false;
                    }
                }
                else {
                    return false;    
                }
            }

            return true;
        }
    }
}