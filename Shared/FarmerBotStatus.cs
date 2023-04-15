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
        public List<ContainerListObject> Containers { get; set; } = new List<ContainerListObject>();
        public bool Status() => Containers.Any(c => c.Running == true);
        public bool ComposeOk { get; set; }
        public bool EnvOk { get; set; }
        public bool ConfigOk { get; set; }
        public DateTime LastUpdate { get; set; }

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

    //public class Container
    //{
    //    public string Name { get; set; } = string.Empty;
    //    public string Status { get; set; } = string.Empty;
    //    public bool IsRunning { get; set; }
    //    public string Image { get; set; } = string.Empty;
    //    public string Restart { get; set; } = string.Empty;
    //    public Dictionary<string, Dependency> DependsOn { get; set; }
    //    public List<string> Volumes { get; set; } = new List<string>();
    //    public string Command { get; set; } = string.Empty;
    //    public string ImageID { get; set; }
    //    public DateTime Created { get; set; }
    //    public IList<Port> Ports { get; set; }
    //    public long SizeRw { get; set; }
    //    public long SizeRootFs { get; set; }
    //    public IDictionary<string, string> Labels { get; set; }
    //    public string State { get; set; }
    //    public SummaryNetworkSettings NetworkSettings { get; set; }
    //    public IList<MountPoint> Mounts { get; set; }

    //    public RunningStatus GetStatusAsEnum()
    //    {
    //         return IsRunning switch
    //         {
    //             true => RunningStatus.Running,
    //             false => RunningStatus.NotRunning
    //         };
    //    }

    //    public void SetStatusAsBool(RunningStatus status)
    //    {
    //        switch (status)
    //        {
    //            case RunningStatus.Running:
    //                IsRunning = true;
    //                break;
    //            case RunningStatus.NotRunning:
    //                IsRunning = false;
    //                break;
    //            case RunningStatus.Unknown:
    //                IsRunning = false;
    //                break;
    //        }
    //    }
    //}
    public enum RunningStatus
    {
        Running,
        NotRunning,
        Unknown
    }
}
