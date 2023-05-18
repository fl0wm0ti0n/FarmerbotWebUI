using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FarmerBotWebUI.Shared.NodeStatus
{
    public class GridNodeSet
    {
        [JsonIgnore]
        public List<Node> Nodes { get; set; } = new List<Node>();

        [JsonIgnore]
        public Farm? GridFarm { get; set; }

        [JsonIgnore]
        public bool IsError { get; set; }
        [JsonIgnore]
        public string ErrorMessage { get; set; } = string.Empty;
    }

    public partial class Nodes
    {
        [JsonProperty("data")]
        public Data Data { get; set; } = new Data();

        [JsonIgnore]
        public bool IsError { get; set; }

        [JsonIgnore]
        public string ErrorMessage { get; set; } = string.Empty;

        [JsonIgnore]
        public int FarmId { get; set; }
    }

    public partial class Data
    {
        [JsonProperty("nodes")]
        public List<Node> Nodes { get; set; } = new List<Node>();
    }

    public partial class Node
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("nodeID")]
        public int NodeId { get; set; }

        [JsonProperty("uptime")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Uptime { get; set; }

        [JsonProperty("virtualized")]
        public bool Virtualized { get; set; }

        [JsonProperty("updatedAt")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long UpdatedAt { get; set; }

        [JsonProperty("twinID")]
        public int TwinId { get; set; }

        [JsonProperty("secure")]
        public bool Secure { get; set; }

        [JsonProperty("serialNumber")]
        public string SerialNumber { get; set; } = string.Empty;

        [JsonProperty("resourcesTotal")]
        public ResourcesTotal ResourcesTotal { get; set; } = new ResourcesTotal();

        [JsonProperty("publicConfig")]
        public PublicConfig PublicConfig { get; set; } = new PublicConfig();

        [JsonProperty("power")]
        public Power Power { get; set; } = new Power();

        [JsonProperty("location")]
        public Location Location { get; set; } = new Location();

        [JsonProperty("interfaces")]
        public List<Interface> Interfaces { get; set; } = new List<Interface>();

        [JsonProperty("gridVersion")]
        public long GridVersion { get; set; }

        [JsonProperty("farmingPolicyId")]
        public long FarmingPolicyId { get; set; }

        [JsonProperty("farmID")]
        public int FarmId { get; set; }

        [JsonProperty("createdAt")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long CreatedAt { get; set; }

        [JsonProperty("created")]
        public long Created { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; } = string.Empty;

        [JsonProperty("connectionPrice")]
        public object ConnectionPrice { get; set; } = string.Empty;

        [JsonProperty("city")]
        public string City { get; set; } = string.Empty;

        [JsonProperty("certification")]
        public string Certification { get; set; } = string.Empty;
    }

    public partial class Interface
    {
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("mac")] 
        public string Mac { get; set; } = string.Empty;

        [JsonProperty("ips")] 
        public string Ips { get; set; } = string.Empty;

        [JsonProperty("id")] 
        public string Id { get; set; } = string.Empty;
    }

    public partial class Location
    {
        [JsonProperty("id")] 
        public string Id { get; set; } = string.Empty;

        [JsonProperty("latitude")] 
        public string Latitude { get; set; } = string.Empty;

        [JsonProperty("longitude")] 
        public string Longitude { get; set; } = string.Empty;
    }

    public partial class Power
    {
        [JsonProperty("target")] 
        public string Target { get; set; } = string.Empty;

        [JsonProperty("state")] 
        public string State { get; set; } = string.Empty;
    }

    public partial class PublicConfig
    {
        [JsonProperty("ipv6")] 
        public string Ipv6 { get; set; } = string.Empty;

        [JsonProperty("ipv4")] 
        public string Ipv4 { get; set; } = string.Empty;

        [JsonProperty("id")] 
        public string Id { get; set; } = string.Empty;

        [JsonProperty("gw6")] 
        public string Gw6 { get; set; } = string.Empty;

        [JsonProperty("gw4")] 
        public string Gw4 { get; set; } = string.Empty;

        [JsonProperty("domain")] 
        public string Domain { get; set; } = string.Empty;
    }

    public partial class ResourcesTotal
    {
        [JsonProperty("sru")]
        public string Sru { get; set; } = string.Empty;

        [JsonProperty("mru")] 
        public string Mru { get; set; } = string.Empty;

        [JsonProperty("id")] 
        public string Id { get; set; } = string.Empty;

        [JsonProperty("hru")] 
        public string Hru { get; set; } = string.Empty;

        [JsonProperty("cru")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Cru { get; set; }
    }

    public partial class Nodes
    {
        public static Nodes FromJson(string json) => JsonConvert.DeserializeObject<Nodes>(json, NodeConverter.Settings);
    }

    public static class NodeSerialize
    {
        public static string ToJson(this Nodes self) => JsonConvert.SerializeObject(self, NodeConverter.Settings);
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