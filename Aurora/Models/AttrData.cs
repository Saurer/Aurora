using System.Linq;
using System.Collections.Generic;
using AuroraCore.Storage;
using System.Threading.Tasks;
using System;

namespace Aurora.Models {
    public class AttrData {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public string Type { get; private set; }
        public IEnumerable<AttrPropertyData> Properties { get; private set; }

        private AttrData() {

        }

        public static async Task<AttrData> Instantiate(IAttr attr) {
            var plainProperties = await attr.GetProperties();
            var dataType = await attr.GetDataType();
            var properties =
                from p in plainProperties
                select new AttrPropertyData(p.BaseEventID, p.ValueID, Int32.Parse(p.Value));

            return new AttrData {
                ID = attr.ID,
                Name = attr.Value,
                Type = dataType.Name,
                Properties = properties
            };
        }
    }
}