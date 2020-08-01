using System;
using System.Threading.Tasks;
using AuroraCore.Storage;

namespace AuroraCore.Controllers {
    public class EventController : Controller {
        [EventReaction(StaticEvent.Event)]
        public async Task Event(IEvent e) {
            var existingEvent = await Storage.GetEvent(e.ID);
            if (null != existingEvent) {
                throw new Exception($"Event '{e.ID}' already exists");
            }

            var genesisEvent = await Storage.GetEvent(StaticEvent.Event);
            var baseEvent = await Storage.GetEvent(e.BaseEventID);
            if (null != genesisEvent && null == baseEvent) {
                throw new Exception($"Event '{e.BaseEventID}' does not exist");
            }

            var valueEvent = await Storage.GetEvent(e.ValueID);
            if (null == valueEvent && null != genesisEvent) {
                throw new Exception($"Event '{e.ValueID}' does not exist");
            }

            var conditionEvent = await Storage.GetEvent(e.ConditionEventID);
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

            if (parent.ValueID == StaticEvent.Model) {
                if (parent.BaseEventID != StaticEvent.Attribute) {
                    throw new Exception($"Only attributes can have properties");
                }
            }
            else if (parent.BaseEventID == StaticEvent.Attribute) {
                var attr = await Storage.GetAttribute(e.BaseEventID);
                if (null == attr) {
                    throw new Exception("Attribute " + e.BaseEventID + " does not exist");
                }

                var property = await Storage.GetAttrProperty(e.ValueID);

                if (null == property) {
                    throw new Exception($"Attribute property '{propertyDefID}' does not exist");
                }

                var exists = await property.ContainsValue(propertyDefID);
                if (!exists) {
                    throw new Exception($"Value '{propertyDefID}' is not assignable to this event");
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

                var boxed = await attribute.IsBoxed();
                if (boxed) {
                    if (Int32.TryParse(e.Value, out var valueID)) {
                        var valueIndividual = await Storage.GetAttrValue(attribute.ID, valueID);

                        if (null == valueIndividual) {
                            throw new Exception($"Attribute value '{valueID}' is not defined for attribute '{attribute.ID}'");
                        }
                    }
                    else {
                        throw new Exception($"Invalid attribute value ID: '{e.Value}', expected number");
                    }
                }
                else {
                    var valid = await attribute.Validate(e.Value);
                    if (!valid) {
                        throw new Exception($"Invalid value for attribute '{attribute.Value}'");
                    }
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
                var type = Storage.GetDataType(e.Value);
                if (null == type) {
                    throw new Exception($"Type '{e.Value}' does not exist");
                }
            }

            switch (e.BaseEventID) {
                case StaticEvent.Attribute:
                case StaticEvent.Actor:
                case StaticEvent.Role:
                case StaticEvent.DataType:
                    break;
                default:
                    switch (parentEvent.BaseEventID) {
                        case StaticEvent.Entity:
                            break;
                        default:
                            throw new Exception($"Invalid individual base event");
                    }
                    break;
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

            var dataType = Storage.GetDataType(typeValue.Value);
            if (null == dataType) {
                throw new Exception("Type '" + typeValue.Value + "' is not active");
            }
        }

        [EventReaction(StaticEvent.AttributeValue)]
        public async Task AttributeValue(IEvent e) {
            var attr = await Storage.GetAttribute(e.BaseEventID);

            if (null == attr) {
                throw new Exception($"Attribute '{e.BaseEventID}' does not exist");
            }

            var dt = await attr.GetDataType();
            if (!dt.AllowsBoxedValue(e.Value)) {
                throw new Exception($"Invalid value for attribute '{attr.Value}'");
            }
        }

        [EventReaction(StaticEvent.ValueProperty)]
        public async Task ValueProperty(IEvent e) {
            var baseEvent = await Storage.GetEvent(e.BaseEventID);

            if (StaticEvent.Attribute != baseEvent.ValueID) {
                throw new Exception($"Invalid base event value: '{e.BaseEventID}'");
            }

            var model = await Storage.GetModel(baseEvent.BaseEventID);
            if (null == model) {
                throw new Exception($"Model '{baseEvent.BaseEventID}' does not exist");
            }

            var attr = await Storage.GetAttribute(Int32.Parse(baseEvent.Value));
            if (null == attr) {
                throw new Exception($"Attribute '{baseEvent.Value}' does not exist");
            }

            switch (e.ValueID) {
                case StaticEvent.Required:
                case StaticEvent.Cardinality:
                    break;

                default:
                    throw new Exception($"Unhandled value property type: '{e.ValueID}'");
            }
        }
    }
}