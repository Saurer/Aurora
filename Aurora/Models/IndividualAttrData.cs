using AuroraCore.Events;

namespace Aurora.Models {
    public class IndividualAttrData : AttrData {
        public string Value { get; private set; }

        public IndividualAttrData(IAttr attr, string value = null) : base(attr) {
            Value = value;
        }
    }
}