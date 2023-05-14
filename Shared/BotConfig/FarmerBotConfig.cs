using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Markdig;
using Markdig.Syntax;
using Markdig.Extensions.CustomContainers;


namespace FarmerbotWebUI.Shared.BotConfig
{
    public class FarmerBotConfig
    {
        public List<NodeDefinition> NodeDefinitions { get; set; } = new List<NodeDefinition>();
        public FarmDefinition FarmDefinition { get; set; } = new FarmDefinition();
        public PowerDefinition PowerDefinition { get; set; } = new PowerDefinition();
        public bool IsError { get; set; } = false;
        public string ErrorMessage { get; set; } = string.Empty;

        public string Serialize()
        {
            StringBuilder sb = new StringBuilder();

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
                sb.AppendLine($"    periodic_wakeup: {PowerDefinition.PeriodicWakeup}");
            }
            return sb.ToString();
        }

        public FarmerBotConfig Deserialize(string markdown)
        {
            FarmerBotConfig parsedMarkdown = new FarmerBotConfig();

            // Parse the markdown
            var pipeline = new MarkdownPipelineBuilder().UseCustomContainers().Build();
            var document = Markdown.Parse(markdown, pipeline);

            // Process the parsed markdown elements
            foreach (var block in document)
            {
                if (block is CustomContainer container)
                {
                    if (container.Info == "farmerbot.nodemanager.define")
                    {
                        NodeDefinition node = ParseNodeDefinition(container);
                        parsedMarkdown.NodeDefinitions.Add(node);
                    }
                    else if (container.Info == "farmerbot.farmmanager.define")
                    {
                        parsedMarkdown.FarmDefinition = ParseFarmDefinition(container);
                    }
                    else if (container.Info == "farmerbot.powermanager.define")
                    {
                        parsedMarkdown.PowerDefinition = ParsePowerDefinition(container);
                    }
                }
            }

            return parsedMarkdown;
        }

        private NodeDefinition ParseNodeDefinition(CustomContainer container)
        {
            NodeDefinition node = new NodeDefinition();
            foreach (var block in container)
            {
                if (block is ParagraphBlock paragraph)
                {
                    string line = paragraph.ToString().Trim();
                    string[] keyValue = line.Split(':');
                    switch (keyValue[0].Trim())
                    {
                        case "id":
                            node.Id = int.Parse(keyValue[1].Trim());
                            break;
                        case "twinid":
                            node.TwinId = int.Parse(keyValue[1].Trim());
                            break;
                        case "public_config":
                            node.PublicConfig = keyValue[1].Trim().Equals("true", StringComparison.OrdinalIgnoreCase);
                            break;
                        case "dedicated":
                            node.Dedicated = int.Parse(keyValue[1].Trim());
                            break;
                        case "certified":
                            node.Certified = keyValue[1].Trim().Equals("yes", StringComparison.OrdinalIgnoreCase);
                            break;
                        case "cpuoverprovision":
                            node.CpuOverProvision = int.Parse(keyValue[1].Trim());
                            break;
                    }
                }
            }

            return node;
        }

        private FarmDefinition ParseFarmDefinition(CustomContainer container)
        {
            FarmDefinition farm = new FarmDefinition();
            foreach (var block in container)
            {
                if (block is ParagraphBlock paragraph)
                {
                    string line = paragraph.ToString().Trim();
                    string[] keyValue = line.Split(':');
                    switch (keyValue[0].Trim())
                    {
                        case "id":
                            farm.Id = int.Parse(keyValue[1].Trim());
                            break;
                        case "public_ips":
                            farm.PublicIps = int.Parse(keyValue[1].Trim());
                            break;
                    }
                }
            }

            return farm;
        }

        private PowerDefinition ParsePowerDefinition(CustomContainer container)
        {
            PowerDefinition power = new PowerDefinition();
            foreach (var block in container)
            {
                if (block is ParagraphBlock paragraph)
                {
                    string line = paragraph.ToString().Trim();
                    string[] keyValue = line.Split(':');
                    switch (keyValue[0].Trim())
                    {
                        case "wake_up_threshold":
                            power.WakeUpThreshold = int.Parse(keyValue[1].Trim());
                            break;
                        case "periodic_wakeup":
                            power.PeriodicWakeup = keyValue[1].Trim();
                            break;
                    }
                }
            }

            return power;
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
