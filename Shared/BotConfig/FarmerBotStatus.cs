using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace FarmerbotWebUI.Shared.BotConfig
{
    public class FarmerBotStatus
    {
        public string Name { get; set; } = string.Empty;
        public int Id { get; set; }
        public List<ContainerListObject> Containers { get; set; } = new List<ContainerListObject>();
        public bool Status() => Containers.Any(c => c.Running == true);
        public bool ComposeOk { get; set; }
        public string ComposeError { get; set; } = string.Empty;
        public bool EnvOk { get; set; }
        public string EnvError { get; set; } = string.Empty;
        public bool ConfigOk { get; set; }
        public string ConfigError { get; set; } = string.Empty;
        public DateTime LastUpdate { get; set; }
        public bool NoStatus { get; set; } = false;
        public BotDefinitionInfos BotDefinitionInfos { get; set; } = new BotDefinitionInfos();

        public RunningStatus GetStatusAsEnum()
        {
            return Status() switch
            {
                true => RunningStatus.Running,
                false => RunningStatus.NotRunning
            };
        }
    }

    public class ContainerListObject
    {
        public string Name { get; set; } = string.Empty;
        public bool Running { get; set; }
        public bool NoContainer { get; set; }
        public ContainerListResponse? Container { get; set; }
    }

    //## Farm Configuration
    //id: the id of the farm
    //public_ips: the amount of public ips that the farm has
    //## Power Configuration
    //wake_up_threshold: a value between 50 and 80 defining the threshold at which nodes will be powered on or off. If the usage percentage (total used resources devided by the total amount of resources) is greater then this threshold a new node will be powered on. In the other case the farmerbot will try to power off nodes if possible.
    //periodic_wakeup: nodes have to be woken up once a day, this variable defines the time at which this should happen. The offline nodes will be powered on sequentially with an interval of 5 minutes starting at the time defined by this variable.
    public class BotDefinitionInfos
    {
        public int Id { get; set; }
        public int PublicIps { get; set; }
        public int WakeUpThreshold { get; set; }
        public string PeriodicWakeup { get; set; } = string.Empty;
    }

    public enum RunningStatus
    {
        Running,
        NotRunning,
        Unknown
    }

}
