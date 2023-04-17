using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FarmerBotWebUI.Shared
{
    public partial class BotNode
    {
        public List<BotNode> Node { get; set; }
        public BotFarm Farm { get; set; }
    }

    public partial class BotNode
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("nodeId")]
        public long NodeId { get; set; }

        [JsonProperty("farmId")]
        public long FarmId { get; set; }

        [JsonProperty("twinId")]
        public long TwinId { get; set; }

        [JsonProperty("gridVersion")]
        public long GridVersion { get; set; }

        [JsonProperty("uptime")]
        public long Uptime { get; set; }

        [JsonProperty("created")]
        public long Created { get; set; }

        [JsonProperty("updatedAt")]
        public long UpdatedAt { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }

    public partial class BotFarm
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("farmId")]
        public long FarmId { get; set; }

        [JsonProperty("twinId")]
        public long TwinId { get; set; }
    }
}