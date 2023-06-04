using FarmerbotWebUI.Shared.BotConfig;
using FarmerBotWebUI.Shared.NodeStatus;
using System.Text;

namespace FarmerbotWebUI.Shared.NodeStatus
{
    public class NodeStatusCollection
    {
        public List<NodeStatusSet> NodeStatusSets { get; set; } = new List<NodeStatusSet>();
        public Farm Farm { get; set; } = new Farm();
        public string BotName { get; set; } = string.Empty;
        public DateTime LastUpdate { get; set; }
        public bool NoStatus { get; set; } = false;
        public bool IsError() => NodeStatusSets.Any(n => n.IsError == true);
        public string ErrorMessage => NodeStatusSets.Aggregate(new StringBuilder(), (sb, n) => sb.AppendLine(n.ErrorMessage)).ToString();
    }
}