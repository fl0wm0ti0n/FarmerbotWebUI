using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FarmerBotWebUI.Shared
{
    public partial class Nodes
    {
        [JsonProperty("data")]
        public Data Data { get; set; }
    }

    public partial class Data
    {
        [JsonProperty("nodes")]
        public List<Node> Nodes { get; set; }
    }
    public partial class Node
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("nodeID")]
        public long NodeId { get; set; }

        [JsonProperty("uptime")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Uptime { get; set; }

        [JsonProperty("virtualized")]
        public bool Virtualized { get; set; }

        [JsonProperty("updatedAt")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long UpdatedAt { get; set; }

        [JsonProperty("twinID")]
        public long TwinId { get; set; }

        [JsonProperty("secure")]
        public bool Secure { get; set; }

        [JsonProperty("serialNumber")]
        public string SerialNumber { get; set; }

        [JsonProperty("resourcesTotal")]
        public ResourcesTotal ResourcesTotal { get; set; }

        [JsonProperty("publicConfig")]
        public PublicConfig PublicConfig { get; set; }

        [JsonProperty("power")]
        public Power Power { get; set; }

        [JsonProperty("location")]
        public Location Location { get; set; }

        [JsonProperty("interfaces")]
        public List<Interface> Interfaces { get; set; }

        [JsonProperty("gridVersion")]
        public long GridVersion { get; set; }

        [JsonProperty("farmingPolicyId")]
        public long FarmingPolicyId { get; set; }

        [JsonProperty("farmID")]
        public long FarmId { get; set; }

        [JsonProperty("createdAt")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long CreatedAt { get; set; }

        [JsonProperty("created")]
        public long Created { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("connectionPrice")]
        public object ConnectionPrice { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("certification")]
        public string Certification { get; set; }
    }

    public partial class Interface
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("mac")]
        public string Mac { get; set; }

        [JsonProperty("ips")]
        public string Ips { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }
    }

    public partial class Location
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("latitude")]
        public string Latitude { get; set; }

        [JsonProperty("longitude")]
        public string Longitude { get; set; }
    }

    public partial class Power
    {
        [JsonProperty("target")]
        public string Target { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }
    }

    public partial class PublicConfig
    {
        [JsonProperty("ipv6")]
        public string Ipv6 { get; set; }

        [JsonProperty("ipv4")]
        public string Ipv4 { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("gw6")]
        public string Gw6 { get; set; }

        [JsonProperty("gw4")]
        public string Gw4 { get; set; }

        [JsonProperty("domain")]
        public string Domain { get; set; }
    }

    public partial class ResourcesTotal
    {
        [JsonProperty("sru")]
        public string Sru { get; set; }

        [JsonProperty("mru")]
        public string Mru { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("hru")]
        public string Hru { get; set; }

        [JsonProperty("cru")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Cru { get; set; }
    }

    public partial class Nodes
    {
        public static Nodes FromJson(string json) => JsonConvert.DeserializeObject<Nodes>(json, FarmerBotWebUI.Shared.NodeConverter.Settings);
    }

    public static class NodeSerialize
    {
        public static string ToJson(this Nodes self) => JsonConvert.SerializeObject(self, FarmerBotWebUI.Shared.NodeConverter.Settings);
    }

    internal static class NodeConverter
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

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }

}