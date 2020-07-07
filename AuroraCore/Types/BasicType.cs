using System.Text.RegularExpressions;

namespace AuroraCore.Types {
    internal class BasicType : DataType {
        private Regex validationPattern = new Regex("^[a-zA-Z_0-9 ]+$");

        public override string Name => "basic_type";

        public override bool Validate(string data) {
            return validationPattern.IsMatch(data);
        }
    }
}