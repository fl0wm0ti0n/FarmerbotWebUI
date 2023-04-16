
namespace FarmerbotWebUI.Client.Services.EventConsole
{
    public interface IEventConsoleService
    {
        public event Action AddedMessage;
        public event Action UpdatedMessage;
        public event Action EventConsoleChanged;
        public event Action ClearedMessages;
        public event Action RemovedMessage;
        public List<EventMessage> Messages { get; set; }
        public List<EventMessage> GetMessages();
        public EventMessage? GetMessage(int id);
        public EventSourceActionId AddMessage(EventSourceActionId id, string title, string message, bool? showPrograssBar, bool? done, bool? showInGui, LogLevel level, EventResult result);
        public EventSourceActionId AddMessage(EventMessage message);
        public void UpdateMessage(EventSourceActionId id, string? title, string? message, bool? showPrograssBar, bool? done, bool? showInGui, LogLevel? level, EventResult? result);
        public void UpdateMessage(EventMessage message);
        public void RemoveMessage(int id);
        public void RemoveMessage(EventSourceActionId id);
        public void ClearMessages();
        public string? Transform(bool toggle);
    }
}
