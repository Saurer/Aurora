using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuroraCore.Storage;

namespace Aurora.Models {
    public class IndividualAttrData {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public string Type { get; private set; }
        public string Value { get; private set; }
        public IEnumerable<AttrPropertyData> Properties { get; private set; }

        private IndividualAttrData() {

        }

        public static async Task<IndividualAttrData> Instantiate(IAttr attr, string value = null) {
            var plainProperties = await attr.GetProperties();
            var dataType = await attr.GetDataType();
            var properties =
                from p in plainProperties
                select new AttrPropertyData(p.BaseEventID, p.ValueID, Int32.Parse(p.Value));

            return new IndividualAttrData {
                ID = attr.ID,
                Name = attr.Value,
                Type = dataType.Name,
                Value = value,
                Properties = properties
            };
        }
    }
}