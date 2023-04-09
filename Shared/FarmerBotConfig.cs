using FarmerbotWebUI.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmerbotWebUI.Shared
{
    public class FarmerBotConfig
    {
        public List<NodeConfig> Nodes { get; set; } = new List<NodeConfig>();
        public List<FarmConfig> Farms { get; set; } = new List<FarmConfig>();
        public PowerConfig Power { get; set; } = new PowerConfig();

        public string MapToString => string.Join(Environment.NewLine, Nodes.Select(n => n.ToString()).Concat(Farms.Select(f => f.ToString())).Concat(new[] { Power.ToString() }));

    }

    //## Node Configuration
    //id: the id of the node
    //twinid: the twin id of the node
    //never_shutdown: a value telling the farmerbot whether or not the node should never be shutdown
    //cpuoverprovision: a value between 1 and 4 defining how much the cpu can be overprovisioned (2 means the farmerbot will allocate 2 deployments to one cpu)
    //public_config: a value telling the farmerbot whether or not the node has a public config
    //dedicated: a value telling the farmerbot whether or not the node is dedicated (only allow renting the full node)
    //certified: a value telling the farmerbot whether or not the node is certified
    public class NodeConfig
    {
        public string Category { get; set; } = "!!farmerbot.nodemanager.define";
        public int Id { get; set; }
        public int TwinId { get; set; }
        public bool? NeverShutdown { get; set; }
        public int? CpuOverProvision { get; set; }
        public bool? PublicConfig { get; set; }
        public int? Dedicated { get; set; }
        public string? Certified
        {
            get
            {
                return certifiedBool ? "yes" : "no";
            }
            set
            {
                certifiedBool = value.Equals("yes", StringComparison.OrdinalIgnoreCase);
            }
        }
        private bool certifiedBool = false;
    }

    //## Farm Configuration
    //id: the id of the farm
    //public_ips: the amount of public ips that the farm has
    public class FarmConfig
    {
        public string Category { get; set; } = "!!farmerbot.farmmanager.define";
        public int Id { get; set; }
        public int PublicIps { get; set; }
    }

    //## Power Configuration
    //wake_up_threshold: a value between 50 and 80 defining the threshold at which nodes will be powered on or off. If the usage percentage (total used resources devided by the total amount of resources) is greater then this threshold a new node will be powered on. In the other case the farmerbot will try to power off nodes if possible.
    //periodic_wakeup: nodes have to be woken up once a day, this variable defines the time at which this should happen. The offline nodes will be powered on sequentially with an interval of 5 minutes starting at the time defined by this variable.

    public class PowerConfig
    {
        public string Category { get; set; } = "!!farmerbot.powermanager.define";
        public int WakeUpThreshold { get; set; }
        public string PeriodicWakeup { get; set; }
    }
}

//Example
//
//My nodes
//!!farmerbot.nodemanager.define
//    id:20
//    twinid: 105
//    public_config: true
//    dedicated: 1
//    certified: yes
//    cpuoverprovision:2

//!!farmerbot.nodemanager.define
//    id:21
//    twinid: 106

//!!farmerbot.nodemanager.define
//    id:22
//    twinid: 107

//Farm configuration
//!!farmerbot.farmmanager.define
//    id:3
//    public_ips: 2

//Power configuration
//!!farmerbot.powermanager.define
//    wake_up_threshold:75
//    periodic_wakeup: 8:30AM
