using FarmerbotWebUI.Shared.BotConfig;
using FarmerBotWebUI.Shared.NodeStatus;
using System.Text;

namespace FarmerbotWebUI.Shared.NodeStatus
{
    public class NodeStatusCollection
    {
        public List<NodeStatusSet> NodeStatusSets { get; set; } = new List<NodeStatusSet>();
        public string BotName { get; set; } = string.Empty;
        public DateTime LastUpdate { get; set; }
        public bool NoStatus { get; set; } = false;
        public bool IsError() => NodeStatusSets.Any(n => n.IsError == true);
        public string ErrorMessage => NodeStatusSets.Aggregate(new StringBuilder(), (sb, n) => sb.AppendLine(n.ErrorMessage)).ToString();

        //public BotNodeSet BotNodeSet { get; set; } = new BotNodeSet();
        //public GridNodeSet GridNodeSet { get; set; } = new GridNodeSet();

        //public bool IsError
        //{
        //    get
        //    {
        //        return (BotNodeSet?.IsError ?? false) ||
        //               (GridNodeSet?.IsError ?? false);
        //    }
        //}

        //public string ErrorMessage
        //{
        //    get
        //    {
        //        var errorMessageBuilder = new StringBuilder();

        //        if (BotNodeSet?.IsError ?? false)
        //        {
        //            errorMessageBuilder.AppendLine(BotNodeSet.ErrorMessage);
        //        }

        //        if (GridNodeSet?.IsError ?? false)
        //        {
        //            errorMessageBuilder.AppendLine(GridNodeSet.ErrorMessage);
        //        }

        //        return errorMessageBuilder.ToString();
        //    }
        //}
    }
}
