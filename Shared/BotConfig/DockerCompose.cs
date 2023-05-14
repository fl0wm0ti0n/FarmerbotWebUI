using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace FarmerbotWebUI.Shared.BotConfig
{
    public class DockerCompose
    {
        [YamlMember(Alias = "services")]
        public Dictionary<string, Service> Services { get; set; }

        [YamlMember(Alias = "volumes")]
        public Dictionary<string, Volume> Volumes { get; set; }

        [YamlIgnore]
        public bool IsError { get; set; } = false;

        [YamlIgnore]
        public string ErrorMessage { get; set; } = string.Empty;

        public DockerCompose DeserializeYaml(string yamlString)
        {
            // NullNamingConvention
            // HyphenatedNamingConvention
            // CamelCaseNamingConvention
            // UnderscoredNamingConvention

            var deserializer = new DeserializerBuilder()
            .WithNamingConvention(NullNamingConvention.Instance)
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

        [YamlMember(Alias = "image")]
        public string Image { get; set; }

        [YamlMember(Alias = "restart")]
        public string Restart { get; set; }

        [YamlMember(Alias = "depends_on")]
        public Dictionary<string, Dependency> DependsOn { get; set; }

        [YamlMember(Alias = "volumes")]
        public List<string> Volumes { get; set; }

        [YamlMember(Alias = "command")]
        public object Command { get; set; }

        [YamlMember(Alias = "entrypoint")]
        public string EntryPoint { get; set; }

        [YamlMember(Alias = "ports")]
        public List<string> Ports { get; set; }

        [YamlMember(Alias = "healthcheck")]
        public HealthCheck HealthCheck { get; set; }
    }

    public class Dependency
    {
        [YamlMember(Alias = "condition")]
        public string Condition { get; set; }
    }

    public class Volume
    {
        [YamlMember(Alias = "driver")]
        public string Driver { get; set; }
    }

    public class HealthCheck
    {
        [YamlMember(Alias = "test")]
        public List<string> Test { get; set; }

        [YamlMember(Alias = "interval")]
        public string Interval { get; set; }

        [YamlMember(Alias = "timeout")]
        public string Timeout { get; set; }

        [YamlMember(Alias = "retries")]
        public int Retries { get; set; }
    }
}
