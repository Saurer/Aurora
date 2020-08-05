using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuroraCore.Storage {
    public interface IModel : IEvent {
        Task<IEvent> GetBaseEvent();
        Task<IModel> GetParent();
        Task<IModelProperty<IAttr>> GetAttribute(int id);
        Task<IEnumerable<IModelProperty<IAttr>>> GetOwnAttributes();
        Task<IEnumerable<IModelProperty<IAttr>>> GetAllAttributes();
        Task<IEnumerable<IModelProperty<IRelation>>> GetOwnRelations();
        Task<IEnumerable<IModelProperty<IRelation>>> GetAllRelations();
        Task<bool> Validate(IReadOnlyDictionary<int, IEnumerable<string>> attributeValues, IReadOnlyDictionary<int, IEnumerable<string>> relationValues);
    }

    internal class Model : Event, IModel {
        public Model(IDataContext context, IEvent e) : base(context, e) {
        }

        public async Task<IEvent> GetBaseEvent() {
            return await Context.Storage.GetEvent(BaseEventID);
        }

        public async Task<IModelProperty<IAttr>> GetAttribute(int attrID) {
            var attribute = await Context.Storage.GetModelAttribute(ID, attrID);
            return attribute;
        }

        public async Task<IEnumerable<IModelProperty<IAttr>>> GetOwnAttributes() {
            return await Context.Storage.GetModelAttributes(ID);
        }

        public async Task<IEnumerable<IModelProperty<IAttr>>> GetAllAttributes() {
            var queue = new Queue<IModel>(new[] { this });
            var result = new List<IModelProperty<IAttr>>();

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

        public async Task<IEnumerable<IModelProperty<IRelation>>> GetOwnRelations() {
            return await Context.Storage.GetModelRelations(ID);
        }

        public async Task<IEnumerable<IModelProperty<IRelation>>> GetAllRelations() {
            var queue = new Queue<IModel>(new[] { this });
            var result = new List<IModelProperty<IRelation>>();

            while (queue.Count > 0) {
                var model = queue.Dequeue();
                var relations = await model.GetOwnRelations();
                result.AddRange(relations);

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

        public async Task<bool> Validate(IReadOnlyDictionary<int, IEnumerable<string>> attributeValues, IReadOnlyDictionary<int, IEnumerable<string>> relationValues) {
            var attributes = await GetAllAttributes();
            var relations = await GetAllRelations();

            foreach (var modelAttr in attributes) {
                var attrProperties = await modelAttr.GetValueProperties();
                var requiredValue = attrProperties.Where(prop => prop.ValueID == StaticEvent.Required).SingleOrDefault()?.Value;
                var attr = await modelAttr.GetProperty();

                // Value is not required
                if (null == requiredValue ? Const.DefaultRequired == 0 : requiredValue == "0") {
                    continue;
                }

                if (attributeValues.TryGetValue(attr.ID, out var valuesCollection)) {
                    foreach (var value in valuesCollection) {
                        var valid = await attr.Validate(value);
                        if (!valid) {
                            return false;
                        }
                    }
                }
                else {
                    return false;
                }
            }

            foreach (var modelRelation in relations) {
                var required = await modelRelation.IsRequired();
                var relation = await modelRelation.GetProperty();

                if (required) {
                    if (relationValues.TryGetValue(relation.ID, out var values)) {
                        if (!values.Any()) {
                            return false;
                        }
                    }
                    else {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}