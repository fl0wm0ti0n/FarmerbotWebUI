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

namespace FarmerbotWebUI.Shared
{
    public class FarmerBotStatus
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public List<ContainerListObject> Containers { get; set; } = new List<ContainerListObject>();
        public bool Status() => Containers.Any(c => c.Running == true);
        public bool ComposeOk { get; set; }
        public bool EnvOk { get; set; }
        public bool ConfigOk { get; set; }
        public DateTime LastUpdate { get; set; }
        public bool NoStatus { get; set; } = false;

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

    public enum RunningStatus
    {
        Running,
        NotRunning,
        Unknown
    }
}
