
namespace FarmerBotWebUI.Shared.NodeStatus
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class Farms
    {
        [JsonProperty("data")] 
        public Data Data { get; set; } = new Data();
    }

    public partial class Data
    {
        [JsonProperty("farms")]
        public List<Farm> Farms { get; set; } = new List<Farm>();
    }

    public partial class Farm
    {
        [JsonProperty("certification")]
        public string Certification { get; set; } = string.Empty;

        [JsonProperty("dedicatedFarm")]
        public bool DedicatedFarm { get; set; }

        [JsonProperty("farmID")]
        public long FarmId { get; set; }

        [JsonProperty("gridVersion")]
        public long GridVersion { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("name")] 
        public string Name { get; set; } = string.Empty;

        [JsonProperty("pricingPolicyID")]
        public long PricingPolicyId { get; set; }

        [JsonProperty("stellarAddress")] 
        public string StellarAddress { get; set; } = string.Empty;

        [JsonProperty("twinID")]
        public long TwinId { get; set; }

        [JsonProperty("publicIPs")] 
        public List<object> PublicIPs { get; set; } = new List<object>();
    }

    public partial class Farms
    {
        public static Farms FromJson(string json) => JsonConvert.DeserializeObject<Farms>(json, FarmConverter.Settings);
    }

    public static class FarmSerialize
    {
        public static string ToJson(this Farms self) => JsonConvert.SerializeObject(self, NodeConverter.Settings);
    }

    internal static class FarmConverter
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
