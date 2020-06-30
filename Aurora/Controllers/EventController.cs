using System;
using System.Collections.Generic;
using Aurora.DataTypes;
using AuroraCore.Controllers;
using AuroraCore.Storage;

namespace Aurora.Controllers {
    public class EventController : Controller {
        private Dictionary<int, Model> models = new Dictionary<int, Model>();
        private Dictionary<int, Model> modelOwners = new Dictionary<int, Model>();
        private Dictionary<int, Individual> individuals = new Dictionary<int, Individual>();
        private TypeManager types = new TypeManager();

        public EventController() {
            types.Register<BasicType>("basic_type");
        }


        [EventReaction(StaticEvent.Event)]
        public void Event(IEventData e) {
            State.Register(e);
        }

        [EventReaction(StaticEvent.SubEvent)]
        public void SubEvent(IEventData e) {
            IEventData parent = State.Get(e.BaseEventID);
            Console.WriteLine("SubEvent:: [{0}]{1} of type [{2}]{3}", e.ID, e.Value, parent.ID, parent.Value);
        }

        [EventReaction(StaticEvent.Model)]
        public void Model(IEventData e) {
            IEventData parent = State.Get(e.ConditionEventID);

            if (parent.ValueID != StaticEvent.Model && parent.ValueID != StaticEvent.Event) {
                throw new Exception("Model base can only be another model");
            }

            if (modelOwners.ContainsKey(e.BaseEventID)) {
                throw new Exception("Only one model per instance is allowed");
            }

            Model parentModel = null;
            if (e.ConditionEventID != StaticEvent.Event) {
                parentModel = models[parent.ID];
            }

            var model = new Model(e.ID, e.Value, parentModel);
            models.Add(e.ID, model);
            modelOwners.Add(e.BaseEventID, model);
            Console.WriteLine("Model registered:: {0}, Parent:: {1}", e.Value, parent.Value);
        }

        [EventReaction(StaticEvent.Attribute)]
        public void Attribute(IEventData e) {
            var baseEvent = State.Get(e.BaseEventID);

            if (baseEvent.ValueID == StaticEvent.Model) {
                var attr = GetAttrFromEvent(e);

                if (models.TryGetValue(e.BaseEventID, out var model)) {
                    model.AddAttribute(attr);
                    Console.WriteLine("Attribute: {0}, Parent: {1}", attr.Name, model.Name);
                }
                else {
                    throw new Exception("Model " + e.BaseEventID + " does not exist");
                }
            }
            else if (baseEvent.ValueID == StaticEvent.Individual) {
                if (individuals.TryGetValue(baseEvent.ID, out var individual)) {
                    var attribute = State.Get(e.ValueID);
                    individual.SetAttribute(e);
                    Console.WriteLine("Set attribute [{0}]{1} of individual [{2}]{3} to value {4}",
                        attribute.ID, attribute.Value,
                        individual.ID, individual.Name,
                        e.Value
                    );
                }
                else {
                    throw new Exception("Individual " + baseEvent.ID + " does not exist");
                }
            }
            else {
                throw new Exception("Attribute can be added only to a model or an individual");
            }
        }

        [EventReaction(StaticEvent.Individual)]
        public void Individual(IEventData e) {
            IEventData parentEvent = State.Get(e.BaseEventID);

            // Register new DataType
            if (e.BaseEventID == StaticEvent.DataType) {
                if (types.IsActive(e.Value)) {
                    throw new Exception("DataType '" + e.Value + "' is already active");
                }
                else if (types.IsRegistered(e.Value)) {
                    types.Activate(e.Value);
                    Console.WriteLine("DataType: {0} activated", e.Value);
                }
                else {
                    throw new Exception("DataType '" + e.Value + "' is not registered");
                }
            }

            if (modelOwners.TryGetValue(e.BaseEventID, out var model)) {
                var individual = new Individual(e, model);
                individuals.Add(e.ID, individual);
                Console.WriteLine("Individual: [{0}]{1} of type [{2}]{3}", e.ID, e.Value, parentEvent.ID, parentEvent.Value);
            }
            else {
                throw new Exception("Trying to instantiate individual without model");
            }
        }

        public Attr GetAttrFromEvent(IEventData e) {
            string defaultValue = null;

            if (!String.IsNullOrEmpty(e.Value)) {
                IEventData defaultValueEvent = State.Get(Int32.Parse(e.Value));
                defaultValue = defaultValueEvent.Value;
            }

            IEventData attrData = e;
            IEventData parent = State.Get(e.BaseEventID);

            // Find attribute declataion
            var queue = new Queue<IEventData>(new[] { State.Get(e.ValueID) });
            while (queue.Count > 0) {
                attrData = queue.Dequeue();

                if (attrData.BaseEventID != StaticEvent.Attribute) {
                    queue.Enqueue(State.Get(attrData.BaseEventID));
                }
            }

            return new Attr(attrData.ID, attrData.Value, defaultValue);
        }
    }
}