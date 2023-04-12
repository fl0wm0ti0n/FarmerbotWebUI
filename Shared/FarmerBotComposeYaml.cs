using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Core;

namespace FarmerbotWebUI.Shared
{

    public class FarmerBotServices
    {
        public Service Farmerbot { get; set; }
        public Service Rmbpeer { get; set; }
        public Service Grid3Client { get; set; }
        public Service Redis { get; set; }
    }

    public class Service
    {
        public string Image { get; set; }
        public string Restart { get; set; }
        public Dictionary<string, Dependency> DependsOn { get; set; }
        public List<string> Volumes { get; set; }
        public string Command { get; set; }
        public Dictionary<string, object> Ports { get; set; }
        public HealthCheck HealthCheck { get; set; }

        public static FarmerBotServices DeserializeYaml(string yamlString)
        {
            //var deserializer = new DeserializerBuilder()
            //    .WithNamingConvention(CamelCaseNamingConvention.Instance)
            //    .Build();

            //using (var reader = new StringReader(yamlString))
            //{
            //    var serviceDefinition = deserializer.Deserialize<ServiceDefinition>(reader);
            //    return serviceDefinition;
            //}

            var deserializer = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)  // see height_in_inches in sample yml 
            .Build();

            //yml contains a string containing your YAML
            return deserializer.Deserialize<FarmerBotServices>(yamlString);
        }

        public static string SerializeYaml(FarmerBotServices yamlObject)
        {
            var serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
            return serializer.Serialize(yamlObject);
        }
    }

    public class Dependency
    {
        public string Condition { get; set; }
    }

    public class HealthCheck
    {
        public List<string> Test { get; set; }
        public string Interval { get; set; }
        public string Timeout { get; set; }
        public int Retries { get; set; }
    }
}
