using System;
using System.Collections.Generic;

namespace AuroraCore.Types {
    public class TypeManager {
        private Dictionary<string, Type> registry = new Dictionary<string, Type>();
        private Dictionary<string, DataType> dataTypes = new Dictionary<string, DataType>();

        public void Register<T>(string name) where T : DataType {
            registry.Add(name, typeof(T));
        }

        public void Activate(string name) {
            if (registry.TryGetValue(name, out var dataType)) {
                if (dataTypes.ContainsKey(name)) {
                    throw new Exception("Type '" + name + "' is already activated");
                }
                else {
                    var instance = (DataType)Activator.CreateInstance(dataType);
                    dataTypes.Add(name, instance);
                }
            }
            else {
                throw new Exception("Type '" + name + "' is not registered");
            }
        }

        public DataType Get(string name) {
            return dataTypes[name];
        }

        public bool IsActive(string name) {
            return dataTypes.ContainsKey(name);
        }

        public bool IsRegistered(string name) {
            return registry.ContainsKey(name);
        }
    }
}