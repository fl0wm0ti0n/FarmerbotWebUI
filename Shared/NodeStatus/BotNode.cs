using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FarmerbotWebUI.Shared.NodeStatus
{
    public partial class BotNodeSet
    {
        public List<BotNode> Nodes { get; set; }
        public BotFarm Farm { get; set; }
        public bool IsError { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }

    public partial class BotNode
    {
        public string Id { get; set; } = string.Empty;
        public int NodeId { get; set; }
        public int FarmId { get; set; }
        public int TwinId { get; set; }
        public long GridVersion { get; set; }
        public TimeSpan Uptime { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool Online { get; set; }
        public NodeDefinitionInfo NodeDefinitionInfo { get; set; }
    }

    //## Node Configuration
    //id: the id of the node
    //twinid: the twin id of the node
    //never_shutdown: a value telling the farmerbot whether or not the node should never be shutdown
    //cpuoverprovision: a value between 1 and 4 defining how much the cpu can be overprovisioned (2 means the farmerbot will allocate 2 deployments to one cpu)
    //public_config: a value telling the farmerbot whether or not the node has a public config
    //dedicated: a value telling the farmerbot whether or not the node is dedicated (only allow renting the full node)
    //certified: a value telling the farmerbot whether or not the node is certified
    public class NodeDefinitionInfo
    {
        public int Id { get; set; }
        public int TwinId { get; set; }
        public bool NeverShutdown { get; set; } = false;
        public int CpuOverProvision { get; set; } = 0;
        public bool PublicConfig { get; set; } = false;
        public int Dedicated { get; set; } = 0;
        public bool Certified { get; set; } = false;
    }

    public partial class BotFarm
    {
        public string Name { get; set; } = string.Empty;
        public long FarmId { get; set; }
        public long TwinId { get; set; }
    }
}