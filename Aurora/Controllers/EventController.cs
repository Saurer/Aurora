using System;
using System.Collections.Generic;
using Aurora.DataTypes;
using AuroraCore.Controllers;
using AuroraCore.Storage;

namespace Aurora.Controllers {
    public class EventController : Controller {
        private Dictionary<int, Model> models = new Dictionary<int, Model>();
        private Dictionary<int, Individual> individuals = new Dictionary<int, Individual>();
        private AttrModel attrModel = new AttrModel();
        private Dictionary<int, Attr> attributes = new Dictionary<int, Attr>();
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

            if (e.BaseEventID == StaticEvent.AttributeProperty) {
                attrModel.RegisterProperty(e.ID, e.Value);
            }
            Console.WriteLine("SubEvent:: [{0}]{1} of type [{2}]{3}", e.ID, e.Value, parent.ID, parent.Value);
        }

        [EventReaction(StaticEvent.Model)]
        public void Model(IEventData e) {
            IEventData parent = State.Get(e.ConditionEventID);

            if (parent.ValueID != StaticEvent.Model && parent.ValueID != StaticEvent.Event) {
                throw new Exception("Model base can only be another model");
            }

            Model parentModel = null;
            if (e.ConditionEventID != StaticEvent.Event) {
                parentModel = models[parent.ID];
            }

            var model = new Model(e.ID, e.Value, parentModel);
            models.Add(e.ID, model);
            Console.WriteLine("Model registered:: {0}, Parent:: {1}", e.Value, parent.Value);
        }

        [EventReaction(StaticEvent.Attribute)]
        public void Attribute(IEventData e) {
            var baseEvent = State.Get(e.BaseEventID);

            if (baseEvent.ValueID == StaticEvent.Model) {
                int attrID;
                Attr attr;
                Model model;

                if (!Int32.TryParse(e.Value, out attrID)) {
                    throw new Exception("Invalid attribute ID: " + e.Value);
                }

                if (!attributes.TryGetValue(attrID, out attr)) {
                    throw new Exception("Attribute " + attrID + " does not exist");
                }

                if (!models.TryGetValue(e.BaseEventID, out model)) {
                    throw new Exception("Model " + e.BaseEventID + " does not exist");
                }

                model.AddAttribute(attr);
                Console.WriteLine("Attribute: {0}, Parent: {1}", attr.Name, model.Name);
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
            Model model;

            if (!models.TryGetValue(e.ConditionEventID, out model)) {
                throw new Exception("Model " + e.ConditionEventID + " does not exist");
            }

            var individual = new Individual(e, model);
            individuals.Add(e.ID, individual);
            Console.WriteLine("Individual: [{0}]{1} of type [{2}]{3}", e.ID, e.Value, parentEvent.ID, parentEvent.Value);

            if (e.BaseEventID == StaticEvent.DataType) {
                types.Activate(e.Value);
            }

            if (attrModel.PropertyRegistered(e.BaseEventID)) {
                attrModel.RegisterValue(e.BaseEventID, e.ID, e.Value);
            }
            else if (e.BaseEventID == StaticEvent.Attribute) {
                attributes.Add(e.ID, new Attr(e.ID, e.Value));
            }
        }

        [EventReaction(StaticEvent.DataType)]
        public void DataType(IEventData e) {
            int typeID;
            Individual typeValue;

            if (!Int32.TryParse(e.Value, out typeID)) {
                throw new Exception("Invalid DataType: " + e.Value);
            }

            if (!individuals.TryGetValue(typeID, out typeValue)) {
                throw new Exception("DataType '" + e.Value + "' is not registered");
            }

            if (!types.IsActive(typeValue.Name)) {
                throw new Exception("Type '" + typeValue.Name + "' is not active");
            }
        }
    }
}