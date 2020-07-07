using System.Text.RegularExpressions;
using AuroraCore.Types;

namespace Aurora.DataTypes {
    public class BasicType : DataType {
        private Regex validationPattern = new Regex("^[a-zA-Z_0-9 ]+$");

        public override string Name => "basic_type";

        public override bool Validate(string data) {
            return validationPattern.IsMatch(data);
        }
    }
}