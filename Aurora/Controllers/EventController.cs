using System;
using AuroraCore;
using AuroraCore.Controllers;
using AuroraCore.Events;
using AuroraCore.Storage;
using AuroraCore.Transactions;

namespace Aurora.Controllers {
    public class EventController : Controller {
        [EventReaction(StaticEvent.Event)]
        public void Event(IEventData e, Transaction tx) {
            tx.AddEvent(e);
        }

        [EventReaction(StaticEvent.SubEvent)]
        public void SubEvent(IEventData e, Transaction tx) {
            IEventData parent;

            if (!State.TryGetEvent(e.BaseEventID, out parent)) {
                throw new Exception($"Event '{e.BaseEventID}' does not exist");
            }

            if (e.BaseEventID == StaticEvent.AttributeProperty) {
                tx.AddAttrProperty(e.ID, e.Value);
            }
        }

        [EventReaction(StaticEvent.AttributeProperty)]
        public void AttributeProperty(IEventData e, Transaction tx) {
            IEventData parent;
            IEventData propertyDef;
            int propertyDefID;

            if (!State.TryGetEvent(e.BaseEventID, out parent)) {
                throw new Exception($"Base event {e.BaseEventID} does not exist");
            }

            if (!Int32.TryParse(e.Value, out propertyDefID)) {
                throw new Exception($"Expected ID: '{e.Value}'");
            }

            if (!State.TryGetEvent(propertyDefID, out propertyDef)) {
                throw new Exception($"Property definition {propertyDefID} does not exist");
            }

            if (parent.ValueID == StaticEvent.Model && parent.BaseEventID == StaticEvent.Attribute) {
                tx.AddAttrProperty(propertyDef.ID, propertyDef.Value);
            }
            else if (parent.BaseEventID == StaticEvent.Attribute) {
                IAttr attr;
                IIndividual valueIndividual;

                if (!State.TryGetValue(e.BaseEventID, out attr)) {
                    throw new Exception("Attribute " + e.BaseEventID + " does not exist");
                }

                if (!State.TryGetValue(propertyDefID, out valueIndividual)) {
                    throw new Exception("Attribute value " + propertyDefID + " does not exist");
                }

                tx.SetAttrProperty(attr.ID, e.ValueID, valueIndividual.ID);

            }
            else {
                throw new Exception("Illegal operation");
            }
        }

        [EventReaction(StaticEvent.Model)]
        public void Model(IEventData e, Transaction tx) {
            IModel parentModel = null;
            IEventData parent;

            if (!State.TryGetEvent(e.ConditionEventID, out parent)) {
                throw new Exception($"Event '{e.ConditionEventID}' does not exist");
            }

            if (parent.ValueID != StaticEvent.Model && parent.ValueID != StaticEvent.Event) {
                throw new Exception("Model base can only be another model");
            }

            if (e.ConditionEventID != StaticEvent.Event && !State.TryGetValue(e.ConditionEventID, out parentModel)) {
                throw new Exception("Model " + e.ConditionEventID + " does not exist");
            }

            tx.AddModel(e, parentModel);
        }

        [EventReaction(StaticEvent.Attribute)]
        public void Attribute(IEventData e, Transaction tx) {
            IEventData baseEvent;

            if (!State.TryGetEvent(e.BaseEventID, out baseEvent)) {
                throw new Exception($"Event '{e.BaseEventID}' does not exist");
            }

            if (baseEvent.ValueID == StaticEvent.Model) {
                int attrID;
                IAttr attr;
                IModel model;

                if (!Int32.TryParse(e.Value, out attrID)) {
                    throw new Exception("Invalid attribute ID: " + e.Value);
                }

                if (!State.TryGetValue(attrID, out attr)) {
                    throw new Exception("Attribute " + attrID + " does not exist");
                }

                if (!State.TryGetValue(e.BaseEventID, out model)) {
                    throw new Exception("Model " + e.BaseEventID + " does not exist");
                }

                tx.AddModelAttribute(model.ID, attr);
            }
            else if (baseEvent.ValueID == StaticEvent.Individual) {
                IIndividual individual;
                IEventData attribute;

                if (!State.TryGetValue(baseEvent.ID, out individual)) {
                    throw new Exception("Individual " + baseEvent.ID + " does not exist");
                }

                if (!State.TryGetEvent(e.ValueID, out attribute)) {
                    throw new Exception($"Event '{e.ValueID}' does not exist");
                }

                tx.AddIndividualAttribute(individual.ID, e.ValueID, e.Value);
            }
            else {
                throw new Exception("Attribute can be added only to a model or an individual");
            }
        }

        [EventReaction(StaticEvent.Individual)]
        public void Individual(IEventData e, Transaction tx) {
            IEventData parentEvent;
            IModel model;

            if (!State.TryGetEvent(e.BaseEventID, out parentEvent)) {
                throw new Exception($"Event '{e.BaseEventID}' does not exist");
            }

            if (!State.TryGetValue(e.ConditionEventID, out model)) {
                throw new Exception("Model " + e.ConditionEventID + " does not exist");
            }

            if (e.BaseEventID == StaticEvent.DataType) {
                tx.ActivateType(e.ID, e.Value);
            }

            if (State.PropertyRegistered(e.BaseEventID)) {
                tx.AddAttrPropertyValue(e.BaseEventID, e.ID, e.Value);
                tx.AddIndividual(e, model);
            }
            else if (e.BaseEventID == StaticEvent.Attribute) {
                tx.AddAttr(e);
            }
            else {
                tx.AddIndividual(e, model);
            }
        }

        [EventReaction(StaticEvent.DataType)]
        public void DataType(IEventData e, Transaction tx) {
            int typeID;
            IIndividual typeValue;

            if (!Int32.TryParse(e.Value, out typeID)) {
                throw new Exception("Invalid DataType: " + e.Value);
            }

            if (!State.TryGetValue(typeID, out typeValue)) {
                throw new Exception("DataType '" + e.Value + "' is not registered");
            }

            if (!State.Types.IsActive(typeValue.Value)) {
                throw new Exception("Type '" + typeValue.Value + "' is not active");
            }
        }
    }
}