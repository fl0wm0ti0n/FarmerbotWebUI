using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Markdig;
using Markdig.Syntax;
using Markdig.Extensions.CustomContainers;
using System.Reflection.Metadata;
using System.Globalization;

namespace FarmerbotWebUI.Shared.BotConfig
{
    public class FarmerBotConfig
    {
        public List<NodeDefinition> NodeDefinitions { get; set; } = new List<NodeDefinition>();
        public FarmDefinition FarmDefinition { get; set; } = new FarmDefinition();
        public PowerDefinition PowerDefinition { get; set; } = new PowerDefinition();
        public bool IsError { get; set; } = false;
        public string ErrorMessage { get; set; } = string.Empty;

        public void Deserialize(string document)
        {
            try
            {
                var lines = document.Split('\n');

                for (var i = 0; i < lines.Length; i++)
                {

                    //## Node Configuration
                    //id: the id of the node
                    //twinid: the twin id of the node
                    //never_shutdown: a value telling the farmerbot whether or not the node should never be shutdown
                    //cpuoverprovision: a value between 1 and 4 defining how much the cpu can be overprovisioned (2 means the farmerbot will allocate 2 deployments to one cpu)
                    //public_config: a value telling the farmerbot whether or not the node has a public config
                    //dedicated: a value telling the farmerbot whether or not the node is dedicated (only allow renting the full node)
                    //certified: a value telling the farmerbot whether or not the node is certified
                    if (lines[i].Trim() == "!!farmerbot.nodemanager.define")
                    {
                        var node = new NodeDefinition();
                        while (!string.IsNullOrWhiteSpace(lines[++i]))
                        {
                            var parts = lines[i].Split(':');
                            if (parts[0].Trim() == "id")
                                node.Id = int.Parse(parts[1]);
                            else if (parts[0].Trim() == "twinid")
                                node.TwinId = int.Parse(parts[1]);
                            else if (parts[0].Trim() == "never_shutdown")
                                node.NeverShutdown = bool.Parse(parts[1]);
                            else if (parts[0].Trim() == "cpuoverprovision")
                                node.CpuOverProvision = int.Parse(parts[1]);
                            else if (parts[0].Trim() == "public_config")
                                node.PublicConfig = bool.Parse(parts[1]);
                            else if (parts[0].Trim() == "dedicated")
                                node.Dedicated = int.Parse(parts[1]);
                            else if (parts[0].Trim() == "certified")
                                node.Certified = bool.Parse(parts[1]);
                        }
                        NodeDefinitions.Add(node);
                    }

                    //## Farm Configuration
                    //id: the id of the farm
                    //public_ips: the amount of public ips that the farm has
                    else if (lines[i].Trim() == "!!farmerbot.farmmanager.define")
                    {
                        var farm = new FarmDefinition();
                        while (!string.IsNullOrWhiteSpace(lines[++i]))
                        {
                            var parts = lines[i].Split(':');
                            if (parts[0].Trim() == "id")
                                farm.Id = int.Parse(parts[1]);
                            else if (parts[0].Trim() == "public_ips")
                                farm.PublicIps = int.Parse(parts[1]);
                        }
                        FarmDefinition = farm;
                    }

                    //## Power Configuration
                    //wake_up_threshold: a value between 50 and 80 defining the threshold at which nodes will be powered on or off. If the usage percentage (total used resources devided by the total amount of resources) is greater then this threshold a new node will be powered on. In the other case the farmerbot will try to power off nodes if possible.
                    //periodic_wakeup: nodes have to be woken up once a day, this variable defines the time at which this should happen. The offline nodes will be powered on sequentially with an interval of 5 minutes starting at the time defined by this variable.
                    else if (lines[i].Trim() == "!!farmerbot.powermanager.define")
                    {
                        var power = new PowerDefinition();
                        while (!string.IsNullOrWhiteSpace(lines[++i]))
                        {
                            var parts = lines[i].Split(':');
                            if (parts[0].Trim() == "periodic_wakeup")
                            {
                                string keyValue = $"{parts[1].Trim()}:{parts[2].Trim()}";
                                string[] formats = { "h:mmtt", "h:mm tt", "H:mm", "hh:mmtt" };
                                DateTime time;
                                if (DateTime.TryParseExact(keyValue, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out time))
                                {
                                    power.PeriodicWakeup = time;
                                }
                                else
                                {
                                    IsError = true;
                                    ErrorMessage = "Invalid time format for wake_up_threshold";
                                }
                            }
                            else if (parts[0].Trim() == "wake_up_threshold")
                            {
                                power.WakeUpThreshold = int.Parse(parts[1]);
                            }
                        }
                        PowerDefinition = power;
                    }
                }
            }
            catch (Exception ex)
            {
                IsError = true;
                ErrorMessage = ex.Message;
            }
        }

        public string Serialize()
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                // Serialize nodes
                sb.AppendLine("My nodes");
                foreach (var node in NodeDefinitions)
                {
                    sb.AppendLine("!!farmerbot.nodemanager.define");
                    sb.AppendLine($"    id:{node.Id}");
                    sb.AppendLine($"    twinid: {node.TwinId}");

                    if (node.PublicConfig.HasValue)
                        sb.AppendLine($"    public_config: {node.PublicConfig.Value.ToString().ToLowerInvariant()}");

                    if (node.Dedicated.HasValue)
                        sb.AppendLine($"    dedicated: {node.Dedicated.Value}");

                    if (node.Certified.HasValue)
                        sb.AppendLine($"    certified: {(node.Certified.Value ? "yes" : "no")}");

                    if (node.CpuOverProvision.HasValue)
                        sb.AppendLine($"    cpuoverprovision:{node.CpuOverProvision.Value}");
                }

                // Serialize farm configuration
                if (FarmDefinition != null)
                {
                    sb.AppendLine();
                    sb.AppendLine("Farm configuration");
                    sb.AppendLine("!!farmerbot.farmmanager.define");
                    sb.AppendLine($"    id:{FarmDefinition.Id}");
                    sb.AppendLine($"    public_ips: {FarmDefinition.PublicIps}");
                }

                // Serialize power configuration
                if (PowerDefinition != null)
                {
                    sb.AppendLine();
                    sb.AppendLine("Power configuration");
                    sb.AppendLine("!!farmerbot.powermanager.define");
                    sb.AppendLine($"    wake_up_threshold:{PowerDefinition.WakeUpThreshold}");
                    sb.AppendLine($"    periodic_wakeup: {PowerDefinition.PeriodicWakeup.ToString("h:mmtt")}");
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                IsError = true; 
                ErrorMessage = ex.Message;
                return ex.Message;
            }      
        }
    }

    //## Node Configuration
    //id: the id of the node
    //twinid: the twin id of the node
    //never_shutdown: a value telling the farmerbot whether or not the node should never be shutdown
    //cpuoverprovision: a value between 1 and 4 defining how much the cpu can be overprovisioned (2 means the farmerbot will allocate 2 deployments to one cpu)
    //public_config: a value telling the farmerbot whether or not the node has a public config
    //dedicated: a value telling the farmerbot whether or not the node is dedicated (only allow renting the full node)
    //certified: a value telling the farmerbot whether or not the node is certified
    public class NodeDefinition
    {
        public int Id { get; set; }
        public int TwinId { get; set; }
        public bool? NeverShutdown { get; set; }
        public int? CpuOverProvision { get; set; }
        public bool? PublicConfig { get; set; }
        public int? Dedicated { get; set; }
        public bool? Certified { get; set; }
    }

    //## Farm Configuration
    //id: the id of the farm
    //public_ips: the amount of public ips that the farm has
    public class FarmDefinition
    {
        public int Id { get; set; }
        public int PublicIps { get; set; }
    }

    //## Power Configuration
    //wake_up_threshold: a value between 50 and 80 defining the threshold at which nodes will be powered on or off. If the usage percentage (total used resources devided by the total amount of resources) is greater then this threshold a new node will be powered on. In the other case the farmerbot will try to power off nodes if possible.
    //periodic_wakeup: nodes have to be woken up once a day, this variable defines the time at which this should happen. The offline nodes will be powered on sequentially with an interval of 5 minutes starting at the time defined by this variable.

    public class PowerDefinition
    {
        public int WakeUpThreshold { get; set; }
        public DateTime PeriodicWakeup { get; set; }
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
