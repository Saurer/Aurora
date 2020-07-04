using System;
using AuroraCore.Controllers;
using AuroraCore.Events;
using AuroraCore.Storage;

namespace Aurora.Controllers {
    public class EventController : Controller {
        [EventReaction(StaticEvent.Event)]
        public void Event(IEventData e) {
            State.Register(e);
        }

        [EventReaction(StaticEvent.SubEvent)]
        public void SubEvent(IEventData e) {
            IEventData parent = State.Get(e.BaseEventID);

            if (e.BaseEventID == StaticEvent.AttributeProperty) {
                State.RegisterAttrProperty(e.ID, e.Value);
            }
            Console.WriteLine("SubEvent:: [{0}]{1} of type [{2}]{3}", e.ID, e.Value, parent.ID, parent.Value);
        }

        [EventReaction(StaticEvent.AttributeProperty)]
        public void AttributeProperty(IEventData e) {
            IEventData parent = State.Get(e.BaseEventID);
            int propertyDefID;

            if (!Int32.TryParse(e.Value, out propertyDefID)) {
                throw new Exception("Expected ID: " + e.Value);
            }

            IEventData propertyDef = State.Get(propertyDefID);
            if (parent.ValueID == StaticEvent.Model && parent.BaseEventID == StaticEvent.Attribute) {
                State.AttributeModel.RegisterProperty(propertyDef.ID, propertyDef.Value);
                State.FlushAttrProperties(propertyDef.ID);
            }
            else if (parent.BaseEventID == StaticEvent.Attribute) {
                Attr attr;
                Individual valueIndividual;

                if (!State.Attributes.TryGetValue(e.BaseEventID, out attr)) {
                    throw new Exception("Attribute " + e.BaseEventID + " does not exist");
                }

                if (!State.Individuals.TryGetValue(propertyDefID, out valueIndividual)) {
                    throw new Exception("Attribute value " + propertyDefID + " does not exist");
                }

                attr.SetProperty(e.ValueID, valueIndividual.ID);
            }
            else {
                throw new Exception("Illegal operation");
            }
        }

        [EventReaction(StaticEvent.Model)]
        public void Model(IEventData e) {
            Model parentModel = null;
            IEventData parent = State.Get(e.ConditionEventID);

            if (parent.ValueID != StaticEvent.Model && parent.ValueID != StaticEvent.Event) {
                throw new Exception("Model base can only be another model");
            }

            if (e.ConditionEventID != StaticEvent.Event && !State.Models.TryGetValue(e.ConditionEventID, out parentModel)) {
                throw new Exception("Model " + e.ConditionEventID + " does not exist");
            }

            if (e.BaseEventID == StaticEvent.Attribute) {
                State.RegisterAttrModel(e.ID, e.Value);
            }
            else {
                State.RegisterModel(e.ID, e.Value, parentModel);
            }

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

                if (!State.Attributes.TryGetValue(attrID, out attr)) {
                    throw new Exception("Attribute " + attrID + " does not exist");
                }

                if (!State.Models.TryGetValue(e.BaseEventID, out model)) {
                    throw new Exception("Model " + e.BaseEventID + " does not exist");
                }

                model.AddAttribute(attr);
                Console.WriteLine("Attribute: {0}, Parent: {1}", attr.Name, model.Name);
            }
            else if (baseEvent.ValueID == StaticEvent.Individual) {
                if (State.Individuals.TryGetValue(baseEvent.ID, out var individual)) {
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

            if (!State.Models.TryGetValue(e.ConditionEventID, out model)) {
                throw new Exception("Model " + e.ConditionEventID + " does not exist");
            }

            var individual = State.RegisterIndividual(e.ID, e.Value, model);
            Console.WriteLine("Individual: [{0}]{1} of type [{2}]{3}", individual.ID, individual.Name, parentEvent.ID, parentEvent.Value);

            if (e.BaseEventID == StaticEvent.DataType) {
                State.Types.Activate(e.Value);
            }

            if (State.PropertyRegistered(e.BaseEventID)) {
                State.RegisterAttrPropertyValue(e.BaseEventID, e.ID, e.Value);
            }
            else if (e.BaseEventID == StaticEvent.Attribute) {
                State.RegisterAttr(e.ID, e.Value);
            }
        }

        [EventReaction(StaticEvent.DataType)]
        public void DataType(IEventData e) {
            int typeID;
            Individual typeValue;

            if (!Int32.TryParse(e.Value, out typeID)) {
                throw new Exception("Invalid DataType: " + e.Value);
            }

            if (!State.Individuals.TryGetValue(typeID, out typeValue)) {
                throw new Exception("DataType '" + e.Value + "' is not registered");
            }

            if (!State.Types.IsActive(typeValue.Name)) {
                throw new Exception("Type '" + typeValue.Name + "' is not active");
            }
        }
    }
}