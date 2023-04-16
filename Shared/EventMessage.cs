using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmerbotWebUI.Shared
{
    public class EventMessage
    {
        public EventSourceActionId Id { get; set; }
        public DateTime StartTime { get; set; } = DateTime.UtcNow;
        public DateTime? EndTime { get; set; } = null;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool ShowPrograssBar { get; set; } = false;
        public bool Done { get; set; } = false;
        public bool ShowInGui { get; set; } = false;
        public LogLevel Severity { get; set; } = LogLevel.Information;
        public EventResult Result { get; set; } = EventResult.Unknown;

        public bool GetResultAsBool()
        {
            return Result switch
            {
                EventResult.Successfully => true,
                EventResult.Unsuccessfully => false,
                EventResult.Valueless => true,
                EventResult.Unknown => true,
                _ => true,
            };
        }

        public void SetResultAsEnum(bool result)
        {
            switch (result)
            {
                case true:
                    Result = EventResult.Successfully;
                    break;
                case false:
                    Result = EventResult.Unsuccessfully;
                    break;
            }
        }
    }

    public enum EventResult
    {
        Successfully,
        Unsuccessfully,
        Valueless,
        Unknown,

    }
}
