using FarmerBotWebUI.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmerbotWebUI.Shared
{
    public class NodeStatus
    {
        public BotNode BotNode { get; set; }
        public Node GridNode { get; set; }
        public BotFarm BotFarm { get; set; }
        public Farm GridFarm { get; set; }

    }
}
