using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmerbotWebUI.Shared
{
    public class FarmerBotStatus
    {
        public bool IsRunning { get; set; }
        public string Log { get; set; }
        public string LogPath { get; set; }
        public string ComposePs { get; set; }
        public string ComposeLs { get; set; }

    }
}
