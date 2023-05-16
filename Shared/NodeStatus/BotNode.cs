using System;
using System.Collections.Generic;

using System.Globalization;
using FarmerbotWebUI.Shared.BotConfig;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FarmerbotWebUI.Shared.NodeStatus
{
    public partial class BotNodeSet
    {
        public List<BotNode> Nodes { get; set; } = new List<BotNode>();
        public BotFarm Farm { get; set; } = new BotFarm();
        public bool IsError { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }

    public partial class BotNode
    {
        public int NodeId { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool Online { get; set; }
        public NodeDefinition NodeDefinition { get; set; } = new NodeDefinition();
    }

    public partial class BotFarm
    {
        public string BotName { get; set; } = string.Empty;
        public long FarmId { get; set; }
    }
}