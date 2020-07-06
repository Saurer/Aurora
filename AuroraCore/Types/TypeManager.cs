using System;
using System.Collections.Generic;

namespace AuroraCore.Types {
    public interface ITypeManager {
        DataType Get(string name);
        DataType Get(int id);
        bool IsActive(string name);
        bool IsRegistered(string name);
    }

    internal class TypeManager : ITypeManager {
        private Dictionary<string, Type> registry = new Dictionary<string, Type>();
        private Dictionary<string, DataType> dataTypes = new Dictionary<string, DataType>();
        private Dictionary<int, DataType> dataTypesID = new Dictionary<int, DataType>();

        public void Register<T>(string name) where T : DataType {
            registry.Add(name, typeof(T));
        }

        public void Activate(int id, string name) {
            if (registry.TryGetValue(name, out var dataType)) {
                if (dataTypes.ContainsKey(name)) {
                    throw new Exception("Type '" + name + "' is already activated");
                }
                else {
                    var instance = (DataType)Activator.CreateInstance(dataType);
                    instance.Name = name;
                    dataTypes.Add(name, instance);
                    dataTypesID.Add(id, instance);
                }
            }
            else {
                throw new Exception("Type '" + name + "' is not registered");
            }
        }

        public DataType Get(string name) {
            return dataTypes[name];
        }

        public DataType Get(int id) {
            return dataTypesID[id];
        }

        public bool IsActive(string name) {
            return dataTypes.ContainsKey(name);
        }

        public bool IsRegistered(string name) {
            return registry.ContainsKey(name);
        }
    }
}