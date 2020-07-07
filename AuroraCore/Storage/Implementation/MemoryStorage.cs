using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuroraCore.Types;

namespace AuroraCore.Storage.Implementation {
    public class MemoryStorage : IStorageAdapter {
        private Dictionary<int, IEvent> events = new Dictionary<int, IEvent>();
        private IDataContext context;

        public MemoryStorage(ITypeManager typeManager) {
            context = new DataContext(this, typeManager);
        }

        public async Task AddEvent(IEvent e) {
            await Task.Yield();
            events.Add(e.ID, e);
        }

        public async Task<IEvent> GetEvent(int id) {
            await Task.Yield();
            if (events.TryGetValue(id, out var value)) {
                return value;
            }
            else {
                return null;
            }
        }

        public async Task<IAttr> GetAttribute(int id) {
            await Task.Yield();

            var attrDef = await GetEvent(id);

            if (attrDef.BaseEventID != StaticEvent.Attribute) {
                return null;
            }

            return new Attr(context, attrDef);
        }

        public async Task<IAttr> GetModelAttribute(int modelID, int attrID) {
            await Task.Yield();

            var attr = (
                from e in events
                where e.Value.BaseEventID == modelID && e.Value.ValueID == attrID
                select e.Value
            ).SingleOrDefault();

            if (null == attr) {
                return null;
            }

            return new Attr(context, attr);
        }

        public async Task<IEnumerable<IAttr>> GetModelAttributes(int modelID) {
            var attrIDs =
                from e in events
                where e.Value.ValueID == StaticEvent.Attribute
                select Int32.Parse(e.Value.Value);

            var attributes = new List<IAttr>();
            foreach (var attrID in attrIDs) {
                var attribute = await GetAttribute(attrID);
                attributes.Add(attribute);
            }

            return attributes;
        }

        public async Task<IAttrModel> GetAttrModel() {
            await Task.Yield();

            var modelDef = (
                from e in events
                where e.Value.BaseEventID == StaticEvent.Attribute && e.Value.ValueID == StaticEvent.Model
                select e.Value
            ).SingleOrDefault();

            if (null == modelDef) {
                return null;
            }

            return new AttrModel(context, modelDef);
        }

        public async Task<IAttrProperty> GetAttrProperty(int propertyID) {
            await Task.Yield();

            var propertyDef = (
                from e in events
                where e.Value.ValueID == StaticEvent.AttributeProperty && e.Value.Value == propertyID.ToString()
                select e.Value
            ).SingleOrDefault();

            if (null == propertyDef) {
                return null;
            }

            return new AttrProperty(context, propertyDef);
        }

        public async Task<IEnumerable<IAttrProperty>> GetAttrProperties() {
            await Task.Yield();

            var propertyIDs =
                from e in events
                where e.Value.ValueID == StaticEvent.AttributeProperty
                select Int32.Parse(e.Value.Value);

            var result = new List<IAttrProperty>();
            foreach (var propertyID in propertyIDs) {
                var propertyDef = await GetEvent(propertyID);
                var property = new AttrProperty(context, propertyDef);
                result.Add(property);
            }

            return result;
        }

        public async Task<IAttrPropertyValue> GetAttrPropertyValue(int attrID, int propertyID) {
            var attrProto = await GetAttribute(attrID);

            if (null == attrProto) {
                return null;
            }

            var property = (
                from prop in events
                join attr in events on prop.Value.BaseEventID equals attr.Key
                where prop.Value.ValueID == propertyID && attr.Key == attrID
                select prop.Value
            ).SingleOrDefault();

            if (null == property) {
                return null;
            }

            return new AttrPropertyValue(context, property);
        }

        public async Task<IEnumerable<IAttrPropertyValue>> GetAttrPropertyValues(int attrID) {
            var attrProto = await GetAttribute(attrID);

            if (null == attrProto) {
                return Array.Empty<IAttrPropertyValue>();
            }

            var properties =
                from prop in events
                join type in events on prop.Value.ValueID equals type.Key
                where prop.Value.BaseEventID == attrID && type.Value.BaseEventID == StaticEvent.AttributeProperty
                select prop.Value;

            return properties.Select(p => new AttrPropertyValue(context, p));
        }

        public async Task<IModel> GetModel(int id) {
            await Task.Yield();
            var modelDef = await GetEvent(id);

            if (modelDef.ValueID != StaticEvent.Model) {
                return null;
            }

            return new Model(context, modelDef);
        }

        public async Task<IIndividual> GetIndividual(int id) {
            await Task.Yield();
            var individualDef = await GetEvent(id);

            if (individualDef.ValueID != StaticEvent.Individual) {
                return null;
            }

            return new Individual(context, individualDef);
        }

        public async Task<IReadOnlyDictionary<int, string>> GetIndividualAttributes(int id) {
            await Task.Yield();

            var attributes =
                from e in events
                join subEvent in events
                on e.Value.ValueID
                equals subEvent.Key
                where e.Value.BaseEventID == id && subEvent.Value.ValueID == StaticEvent.Attribute
                select new {
                    ID = subEvent.Value.ID,
                    Value = e.Value.Value
                };

            var result = new Dictionary<int, string>();
            foreach (var attribute in attributes) {
                result.Add(attribute.ID, attribute.Value);
            }

            return result;
        }

        public async Task<bool> IsEventAncestor(int ancestor, int checkValue) {
            var queue = new Queue<int>(new[] { checkValue });
            while (queue.Count > 0) {
                int eventID = queue.Dequeue();

                if (eventID == ancestor) {
                    return true;
                }

                var checkAncestor = await GetEvent(eventID);
                if (checkAncestor != null && eventID != checkAncestor.BaseEventID) {
                    queue.Enqueue(checkAncestor.BaseEventID);
                }
            }

            return false;
        }
    }
}