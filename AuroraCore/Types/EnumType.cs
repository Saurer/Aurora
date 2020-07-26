using System.Text.RegularExpressions;
using AuroraCore.Storage;

namespace AuroraCore.Types {
    internal class EnumType : DataType {
        private Regex validationPattern = new Regex("^[a-zA-Z_0-9 ]+$");

        public override string Name => "enum_type";

        public override bool Validate(string data) {
            return validationPattern.IsMatch(data);
        }

        public override bool AllowsBoxedValue(IEvent e) {
            return Validate(e.Value);
        }
    }
}