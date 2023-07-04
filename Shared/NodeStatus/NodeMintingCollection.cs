using FarmerbotWebUI.Shared.BotConfig;
using FarmerBotWebUI.Shared.NodeStatus;
using System.Text;

namespace FarmerbotWebUI.Shared.NodeStatus
{
    public class NodeMintingCollection
    {
        public Dictionary<int, List<MintingReport>> MintingReports { get; set; } = new Dictionary<int, List<MintingReport>>();
        public Farm Farm { get; set; } = new Farm();
        public string BotName { get; set; } = string.Empty;
        public DateTime LastUpdate { get; set; }
        public bool NoStatus { get; set; } = false;
        public bool IsError() => MintingReports.Any(n => n.Value.IsError == true);
        public string ErrorMessage => MintingReports.Aggregate(new StringBuilder(), (sb, n) => sb.AppendLine(n.Value.ErrorMessage)).ToString();
    }
}