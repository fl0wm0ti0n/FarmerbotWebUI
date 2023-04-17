
namespace FarmerBotWebUI.Shared
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class Farms
    {
        [JsonProperty("data")]
        public Data Data { get; set; }
    }

    public partial class Data
    {
        [JsonProperty("farms")]
        public List<Farm> Farms { get; set; }
    }

    public partial class Farm
    {
        [JsonProperty("certification")]
        public string Certification { get; set; }

        [JsonProperty("dedicatedFarm")]
        public bool DedicatedFarm { get; set; }

        [JsonProperty("farmID")]
        public long FarmId { get; set; }

        [JsonProperty("gridVersion")]
        public long GridVersion { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("pricingPolicyID")]
        public long PricingPolicyId { get; set; }

        [JsonProperty("stellarAddress")]
        public string StellarAddress { get; set; }

        [JsonProperty("twinID")]
        public long TwinId { get; set; }

        [JsonProperty("publicIPs")]
        public List<object> PublicIPs { get; set; }
    }

    public partial class Farms
    {
        public static Farms FromJson(string json) => JsonConvert.DeserializeObject<Farms>(json, FarmerBotWebUI.Shared.FarmConverter.Settings);
    }

    public static class FarmSerialize
    {
        public static string ToJson(this Farms self) => JsonConvert.SerializeObject(self, FarmerBotWebUI.Shared.FarmConverter.Settings);
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
