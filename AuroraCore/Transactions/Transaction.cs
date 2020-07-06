using System;
using System.Collections.Generic;
using AuroraCore.Events;
using AuroraCore.Storage;

namespace AuroraCore.Transactions {
    public class Transaction {
        private IAttrModel attrModel;

        internal List<IEventData> Events { get; private set; } = new List<IEventData>();
        internal List<IEvent> Values { get; private set; } = new List<IEvent>();
        internal List<IAttrProperty> AttrProperties { get; private set; } = new List<IAttrProperty>();

        internal List<(int, int, string)> AttrPropertyValues { get; private set; } = new List<(int, int, string)>();
        internal List<(int, IAttr)> ModelAttributes { get; private set; } = new List<(int, IAttr)>();
        internal List<(int, int, string)> IndividualAttributes { get; private set; } = new List<(int, int, string)>();
        internal List<(int, int, int)> AttributeProperties { get; private set; } = new List<(int, int, int)>();
        internal List<(int, string)> TypeActivations { get; private set; } = new List<(int, string)>();

        public Transaction(IAttrModel attrModel) {
            this.attrModel = attrModel;
        }

        public void AddEvent(IEventData e) {
            Events.Add(e);
        }

        public void AddAttrProperty(int id, string name) {
            AttrProperties.Add(new AttrProperty(id, name));
        }

        public void AddAttrPropertyValue(int id, int propertyID, string value) {
            AttrPropertyValues.Add((id, propertyID, value));
        }

        public IAttr AddAttr(IEventData e) {
            if (null == attrModel) {
                throw new Exception("Unable to instantiate attribute since AttrModel is null");
            }

            var attr = new Attr(e, attrModel);
            Values.Add(attr);
            return attr;
        }

        public IModel AddModel(IEventData e, IModel parent = null) {
            var model = new Model(e, parent);
            Values.Add(model);
            return model;
        }

        public IIndividual AddIndividual(IEventData e, IModel model) {
            var individual = new Individual(e, model);
            Values.Add(individual);
            return individual;
        }

        public void SetAttrProperty(int attrID, int propertyID, int valueIndividualID) {
            AttributeProperties.Add((attrID, propertyID, valueIndividualID));
        }

        public void AddModelAttribute(int modelID, IAttr attr) {
            ModelAttributes.Add((modelID, attr));
        }

        public void AddIndividualAttribute(int individualID, int attributeID, string value) {
            IndividualAttributes.Add((individualID, attributeID, value));
        }

        public void ActivateType(int id, string name) {
            TypeActivations.Add((id, name));
        }
    }
}