using FarmerbotWebUI.Shared.BotConfig;
using FarmerBotWebUI.Shared.NodeStatus;
using System.Text;

namespace FarmerbotWebUI.Shared.NodeStatus
{
    public class NodeStatusSet
    {
        public BotNode BotNode { get; set; } = new BotNode();
        public Node GridNode { get; set; } = new Node();
        public Farm Farm { get; set; } = new Farm();
        public string BotName { get; set; } = string.Empty;
        public bool NotConfigured { get; set; } = false;
        public DateTime LastUpdate { get; set; }
        public bool NoStatus { get; set; } = false;
        public bool IsError { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
