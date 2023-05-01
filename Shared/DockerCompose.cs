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
    public class DockerCompose
    {
        public Dictionary<string, Service> Services { get; set; }
        public Dictionary<string, Volume> Volumes { get; set; }
        public bool IsError { get; set; } = false;
        public string ErrorMessage { get; set; } = string.Empty;

        public DockerCompose DeserializeYaml(string yamlString)
        {
            var deserializer = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)  // see height_in_inches in sample yml 
            .Build();

            //yml contains a string containing your YAML
            return deserializer.Deserialize<DockerCompose>(yamlString);
        }

        public string SerializeYaml()
        {
            var serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
            return serializer.Serialize(this);
        }
    }

    public class Service
    {
        public string Image { get; set; }
        public string Restart { get; set; }
        public Dictionary<string, Dependency> DependsOn { get; set; }
        public List<string> Volumes { get; set; }
        public string Command { get; set; }
        public List<string> EntryPoint { get; set; }
        public List<string> Ports { get; set; }
        public HealthCheck HealthCheck { get; set; }
    }

    public class Dependency
    {
        public string Condition { get; set; }
    }

    public class Volume
    {
        public string Driver { get; set; }
    }

    public class HealthCheck
    {
        public List<string> Test { get; set; }
        public string Interval { get; set; }
        public string Timeout { get; set; }
        public int Retries { get; set; }
    }
}
