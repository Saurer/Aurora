using System;
using System.Threading.Tasks;
using AuroraCore.Storage;

namespace AuroraCore.Controllers {
    public class EventController : Controller {
        [EventReaction(StaticEvent.Event)]
        public async Task Event(IEvent e) {
            var existingEvent = await Storage.GetEvent(e.ID);
            var conditionEvent = await Storage.GetEvent(e.ConditionEventID);

            if (null != existingEvent) {
                throw new Exception($"Event '{e.ID}' already exists");
            }

            if (0 != e.ID && null == conditionEvent) {
                throw new Exception($"Event '{e.ConditionEventID}' does not exist");
            }
        }

        [EventReaction(StaticEvent.SubEvent)]
        public async Task SubEvent(IEvent e) {
            IEvent parent = await Storage.GetEvent(e.BaseEventID);

            if (null == parent) {
                throw new Exception($"Event '{e.BaseEventID}' does not exist");
            }
        }

        [EventReaction(StaticEvent.AttributeProperty)]
        public async Task AttributeProperty(IEvent e) {
            IEvent parent = await Storage.GetEvent(e.BaseEventID);
            if (null == parent) {
                throw new Exception($"Base event {e.BaseEventID} does not exist");
            }

            int propertyDefID;
            if (!Int32.TryParse(e.Value, out propertyDefID)) {
                throw new Exception($"Expected ID: '{e.Value}'");
            }

            IEvent propertyDef = await Storage.GetEvent(propertyDefID);
            if (null == propertyDef) {
                throw new Exception($"Property definition {propertyDefID} does not exist");
            }

            if (parent.ValueID == StaticEvent.Model || parent.BaseEventID == StaticEvent.Attribute) {
#warning FIXME
            }
            else if (parent.BaseEventID == StaticEvent.Attribute) {
                IAttr attr = await Storage.GetAttribute(e.BaseEventID);
                if (null == attr) {
                    throw new Exception("Attribute " + e.BaseEventID + " does not exist");
                }

                IIndividual valueIndividual = await Storage.GetIndividual(propertyDefID);
                if (null == valueIndividual) {
                    throw new Exception("Attribute value " + propertyDefID + " does not exist");
                }
            }
            else {
                throw new Exception("Illegal operation");
            }
        }

        [EventReaction(StaticEvent.Model)]
        public async Task Model(IEvent e) {
            var parent = await Storage.GetEvent(e.ConditionEventID);
            if (null == parent) {
                throw new Exception($"Event '{e.ConditionEventID}' does not exist");
            }

            if (parent.ValueID != StaticEvent.Model && parent.ValueID != StaticEvent.Event) {
                throw new Exception("Model base can only be another model");
            }

            var parentModel = await Storage.GetModel(parent.ID);
            if (e.ConditionEventID != StaticEvent.Event && null == parentModel) {
                throw new Exception("Model " + e.ConditionEventID + " does not exist");
            }
        }

        [EventReaction(StaticEvent.Attribute)]
        public async Task Attribute(IEvent e) {
            IEvent baseEvent = await Storage.GetEvent(e.BaseEventID);

            if (null == baseEvent) {
                throw new Exception($"Event '{e.BaseEventID}' does not exist");
            }

            if (baseEvent.ValueID == StaticEvent.Model) {
                int attrID;

                if (!Int32.TryParse(e.Value, out attrID)) {
                    throw new Exception("Invalid attribute ID: " + e.Value);
                }

                var attr = await Storage.GetAttribute(attrID);
                if (null == attr) {
                    throw new Exception("Attribute " + attrID + " does not exist");
                }

                var model = await Storage.GetModel(e.BaseEventID);
                if (null == model) {
                    throw new Exception("Model " + e.BaseEventID + " does not exist");
                }
            }
            else if (baseEvent.ValueID == StaticEvent.Individual) {
                var individual = await Storage.GetIndividual(baseEvent.ID);
                if (null == individual) {
                    throw new Exception("Individual " + baseEvent.ID + " does not exist");
                }

                var attribute = await Storage.GetAttribute(e.ValueID);
                if (null == attribute) {
                    throw new Exception($"Event '{e.ValueID}' does not exist");
                }
            }
            else {
                throw new Exception("Attribute can be added only to a model or an individual");
            }
        }

        [EventReaction(StaticEvent.Individual)]
        public async Task Individual(IEvent e) {
            var parentEvent = await Storage.GetEvent(e.BaseEventID);
            if (null == parentEvent) {
                throw new Exception($"Event '{e.BaseEventID}' does not exist");
            }

            var model = await Storage.GetModel(e.ConditionEventID);
            if (null == model) {
                throw new Exception("Model " + e.ConditionEventID + " does not exist");
            }

            if (e.BaseEventID == StaticEvent.DataType) {
#warning FIXME
                // tx.ActivateType(e.ID, e.Value);
            }
        }

        [EventReaction(StaticEvent.DataType)]
        public async Task DataType(IEvent e) {
            int typeID;
            if (!Int32.TryParse(e.Value, out typeID)) {
                throw new Exception("Invalid DataType: " + e.Value);
            }

            var typeValue = await Storage.GetIndividual(typeID);
            if (null == typeValue) {
                throw new Exception("DataType '" + e.Value + "' is not registered");
            }

#warning FIXME
            // if (!State.Types.IsActive(typeValue.Value)) {
            // throw new Exception("Type '" + typeValue.Value + "' is not active");
            // }
        }
    }
}