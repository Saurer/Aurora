using System.Collections.Generic;

namespace AuroraCore.Types {
    public interface ITypeManager {
        DataType Get(string name);
        DataType Get(int id);
    }

    public class TypeManager : ITypeManager {
        private Dictionary<string, DataType> registry = new Dictionary<string, DataType>();
        private Dictionary<int, DataType> dataTypesID = new Dictionary<int, DataType>();

        public void Register<T>(T type) where T : DataType {
            registry.Add(type.Name, type);
        }

        public DataType Get(string name) {
            if (registry.TryGetValue(name, out var value)) {
                return value;
            }
            else {
                return null;
            }
        }

        public DataType Get(int id) {
            return dataTypesID[id];
        }
    }
}