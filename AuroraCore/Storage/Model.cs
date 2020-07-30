using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuroraCore.Storage {
    public interface IModel : IEvent {
        Task<IEvent> GetBaseEvent();
        Task<IModel> GetParent();
        Task<IAttr> GetAttribute(int id);
        Task<IEnumerable<IAttr>> GetOwnAttributes();
        Task<IEnumerable<IAttr>> GetAllAttributes();
        Task<bool> Validate(IReadOnlyDictionary<int, string> values);
    }

    internal class Model : Event, IModel {
        public Model(IDataContext context, IEvent e) : base(context, e) {
        }

        public async Task<IEvent> GetBaseEvent() {
            return await Context.Storage.GetEvent(BaseEventID);
        }

        public async Task<IAttr> GetAttribute(int attrID) {
            var attribute = await Context.Storage.GetModelAttribute(ID, attrID);
            return attribute;
        }

        public Task<IEnumerable<IAttr>> GetOwnAttributes() {
            return Context.Storage.GetModelAttributes(ID);
        }

        public async Task<IEnumerable<IAttr>> GetAllAttributes() {
            var queue = new Queue<IModel>(new[] { this });
            var result = new List<IAttr>();

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

            if (attributes.Count() != values.Count) {
                return false;
            }

            foreach (var attr in attributes) {
                var value = values[attr.ID];
                var valid = await attr.Validate(value);

                if (!valid) {
                    return false;
                }
            }

            return true;
        }
    }
}