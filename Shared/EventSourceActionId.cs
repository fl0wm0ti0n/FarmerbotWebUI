using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmerbotWebUI.Shared
{
    public class EventSourceActionId
    {
        public EventSource Source { get; set; } = EventSource.Default;
        public string SourceClass => new StackTrace().GetFrame(1).GetMethod().DeclaringType.Name;
        public EventAction Action { get; set; }
        public string ActionClass => new StackTrace().GetFrame(1).GetMethod().Name;
        public EventTyp Typ { get; set; } = EventTyp.Default;
        public int Id { get; set; } = 0;
    }

    public enum EventTyp
    {
        Default,
        UserAction,
        ClientJob,
        ServerJob,
        DockerEvent,
        FarmerbotEvent,
        Unknown,
    }

public enum EventSource
    {
        Default,
        MainLayout,
        Dashboard,
        Settings,
        DockerService,
        TfGraphQlApiClient
    }

    public enum EventAction
    {
        Default,
        Unknown,
        FarmerBotStart,
        FarmerBotStop,
        FarmerBotStatus,
        FarmerBotPs,
        FarmerBotLs,
        FarmerBotLivelog,
        GetGridNodeStatus,
        GetGridFarmStatus
    }
}
