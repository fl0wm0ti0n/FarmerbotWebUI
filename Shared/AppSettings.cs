using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FarmerBotWebUI.Shared
{

    public partial class AppSettings
    {
        [JsonProperty("Logging")]
        public Logging Logging { get; set; }

        [JsonProperty("AllowedHosts")]
        public string AllowedHosts { get; set; }

        [JsonProperty("DockerSettings")]
        public DockerSettings DockerSettings { get; set; }

        [JsonProperty("FarmerBotSettings")]
        public FarmerBotSettings FarmerBotSettings { get; set; }

        [JsonProperty("ThreefoldFarmSettings")]
        public ThreefoldFarmSettings ThreefoldFarmSettings { get; set; }

        [JsonProperty("ThreefoldApiSettings")]
        public ThreefoldApiSettings ThreefoldApiSettings { get; set; }

        [JsonProperty("SecuritySettings")]
        public SecuritySettings SecuritySettings { get; set; }
    }

    public partial class DockerSettings
    {
        [JsonProperty("DockerEndpointWindows")]
        public string DockerEndpointWindows { get; set; }

        [JsonProperty("DockerEndpointLinux")]
        public string DockerEndpointLinux { get; set; }
    }

    public partial class FarmerBotSettings
    {
        [JsonProperty("WorkingDirectory")]
        public string WorkingDirectory { get; set; }

        [JsonProperty("ComposeFile")]
        public string ComposeFile { get; set; }

        [JsonProperty("ThreefoldNetworkFile")]
        public string ThreefoldNetworkFile { get; set; }

        [JsonProperty("FarmerBotConfigFile")]
        public string FarmerBotConfigFile { get; set; }

        [JsonProperty("FarmerBotLogFile")]
        public string FarmerBotLogFile { get; set; }

        [JsonProperty("FarmerBotStatusInterval")]
        public long FarmerBotStatusInterval { get; set; }
    }

    public partial class Logging
    {
        [JsonProperty("LogLevel")]
        public LogLevel LogLevel { get; set; }
    }

    public partial class LogLevel
    {
        [JsonProperty("Default")]
        public string Default { get; set; }

        [JsonProperty("Microsoft.AspNetCore")]
        public string MicrosoftAspNetCore { get; set; }
    }

    public partial class SecuritySettings
    {
        [JsonProperty("DontShowEnv")]
        public bool DontShowEnv { get; set; }
    }

    public partial class ThreefoldApiSettings
    {
        [JsonProperty("GraphQl")]
        public Uri GraphQl { get; set; }

        [JsonProperty("GridProxy")]
        public Uri GridProxy { get; set; }
    }

    public partial class ThreefoldFarmSettings
    {
        [JsonProperty("FarmId")]
        public long FarmId { get; set; }

        [JsonProperty("Network")]
        public string Network { get; set; }

        [JsonProperty("NetworkRelay")]
        public string NetworkRelay { get; set; }
    }

    public partial class AppSettings
    {
        public static AppSettings FromJson(string json) => JsonConvert.DeserializeObject<AppSettings>(json, FarmerBotWebUI.Shared.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this AppSettings self) => JsonConvert.SerializeObject(self, FarmerBotWebUI.Shared.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
