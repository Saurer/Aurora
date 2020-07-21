using System.Collections.Generic;
using System.Linq;

namespace AuroraCore.Types {
    public interface ITypeManager {
        DataType Get(string name);
    }

    public class TypeManager : ITypeManager {
        private Dictionary<string, DataType> registry = new Dictionary<string, DataType>();

        public TypeManager() {
            Register<BasicType>();
        }

        public void Register<T>() where T : DataType, new() {
            var type = new T();
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

        public IEnumerable<DataType> GetAll() {
            return registry.Select(l => l.Value);
        }
    }
}