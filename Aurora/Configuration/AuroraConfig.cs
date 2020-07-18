using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Aurora.Configuration {
    public static class AuroraConfig {
        public static ConfigData Load(string filename) {
            var file = File.OpenText(filename);

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            var config = deserializer.Deserialize<ConfigData>(file);
            return config;
        }
    }
}