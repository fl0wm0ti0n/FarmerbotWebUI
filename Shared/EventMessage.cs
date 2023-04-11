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
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public LogLevel Severity { get; set; } = LogLevel.Information;
        public MessageSource Source { get; set; } = MessageSource.Default;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public MessageResult Result { get; set; } = MessageResult.Unknown;
        public bool ShowPrograssBar { get; set; } = false;
        public ActionType Action { get; set; } = ActionType.Default;

        public bool GetResultAsBool()
        {
            return Result switch
            {
                MessageResult.Successfully => true,
                MessageResult.Unsuccessfully => false,
                MessageResult.Valueless => true,
                MessageResult.Unknown => true,
                _ => true,
            };
        }

        public void SetResultAsEnum(bool result)
        {
            switch (result)
            {
                case true:
                    Result = MessageResult.Successfully;
                    break;
                case false:
                    Result = MessageResult.Unsuccessfully;
                    break;
            }
        }
    }

public enum MessageSource
    {
        Default,
        UserAction,
        SystemEvent,
        DockerEvent,
        FarmerbotEvent,
        Unknown,
    }
    public enum MessageResult
    {
        Successfully,
        Unsuccessfully,
        Valueless,
        Unknown,

    }
}
