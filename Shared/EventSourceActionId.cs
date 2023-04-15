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
        public EventSource Source { get; set; }
        public string SourceClass => new StackTrace().GetFrame(1).GetMethod().DeclaringType.Name;
        public EventAction Action { get; set; }
        public string ActionClass => new StackTrace().GetFrame(1).GetMethod().Name;
        public int Id { get; set; } = 0;
    }


    public enum EventSource
    {
        Default,
        MainLayout,
        Dashboard,
        Settings,
        DockerService
    }

    public enum EventAction
    {
        Default,
        Start,
        Stop,
        Status,
        Unknown,
    }
}
