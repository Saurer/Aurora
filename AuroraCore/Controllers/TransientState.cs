using System;
using System.Collections.Generic;
using AuroraCore.Events;
using AuroraCore.Storage;
using AuroraCore.Types;

namespace AuroraCore.Controllers {
    public class TransientState {
        private Dictionary<int, IEventData> events = new Dictionary<int, IEventData>();
        private Dictionary<int, List<IEventData>> descendants = new Dictionary<int, List<IEventData>>();
        private Dictionary<int, int> ancestors = new Dictionary<int, int>();
        private Dictionary<int, Model> models = new Dictionary<int, Model>();
        private Dictionary<int, Individual> individuals = new Dictionary<int, Individual>();
        private AttrModel attrModel;
        private Dictionary<int, AttrModel.AttrProperty> attrProperties = new Dictionary<int, AttrModel.AttrProperty>();
        private Dictionary<int, Attr> attributes = new Dictionary<int, Attr>();
        private TypeManager types = new TypeManager();

        public TypeManager Types {
            get {
                return types;
            }
        }

        public AttrModel AttributeModel {
            get {
                if (null == attrModel) {
                    throw new Exception("Attribute model is not instantiated");
                }

                return attrModel;
            }
        }

        public IReadOnlyDictionary<int, Attr> Attributes {
            get {
                return attributes;
            }
        }

        public IReadOnlyDictionary<int, Model> Models {
            get {
                return models;
            }
        }

        public IReadOnlyDictionary<int, Individual> Individuals {
            get {
                return individuals;
            }
        }

        public void Register(IEventData e) {
            if (events.TryGetValue(e.BaseEventID, out var parent) || IsGenesisValue(e)) {
                events[e.ID] = e;

                if (!descendants.ContainsKey(e.BaseEventID)) {
                    descendants[e.BaseEventID] = new List<IEventData>();
                }

                descendants[e.BaseEventID].Add(e);
                ancestors[e.ID] = parent?.ID ?? e.ID;
            }
            else {
                throw new Exception("Value ID does not exist");
            }
        }

        public IEventData Get(int eventID) {
            return events[eventID];
        }

        public bool IsEventAncestor(int ancestor, int checkValue) {
            var queue = new Queue<int>(new[] { checkValue });
            while (queue.Count > 0) {
                int eventID = queue.Dequeue();

                if (eventID == ancestor) {
                    return true;
                }

                if (ancestors.TryGetValue(eventID, out var newEventID) && eventID != newEventID) {
                    queue.Enqueue(newEventID);
                }
            }

            return false;
        }

        public AttrModel RegisterAttrModel(int id, string name, Model parent = null) {
            if (null != attrModel) {
                throw new Exception("Unable to instantiate AttrModel since it already exists");
            }

            attrModel = new AttrModel(id, name, parent);
            models.Add(id, attrModel);
            return attrModel;
        }

        public void RegisterAttrProperty(int id, string name) {
            attrProperties.Add(id, new AttrModel.AttrProperty(id, name));
        }

        public void RegisterAttrPropertyValue(int id, int propertyID, string value) {
            attrProperties[id].RegisterValue(propertyID, value);
        }

        public Attr RegisterAttr(int id, string name) {
            var attr = new Attr(id, name, attrModel);
            attributes.Add(id, attr);
            return attr;
        }

        public Model RegisterModel(int id, string name, Model parent = null) {
            var model = new Model(id, name, parent);
            models.Add(id, model);
            return model;
        }

        public Individual RegisterIndividual(int id, string name, Model model) {
            var individual = new Individual(id, name, model);
            individuals.Add(id, individual);
            return individual;
        }

        public bool PropertyRegistered(int id) {
            return attrProperties.ContainsKey(id);
        }

        public void FlushAttrProperties(int propertyID) {
            var property = attrProperties[propertyID];

            foreach (var kv in property.Values) {
                attrModel.RegisterValue(propertyID, kv.Key, kv.Value);
            }

            attrProperties.Remove(propertyID);
        }

        private bool IsGenesisValue(IEventData e) =>
            e.ID == e.ValueID && events.Count == 0;
    }
}