
namespace FarmerbotWebUI.Client.Services.EventConsole
{
    public interface IEventConsoleService
    {
        public event Action NewMessage;
        public event Action AddedMessage;
        public event Action UpdatedMessage;
        public event Action EventConsoleChanged;
        public event Action ClearedMessages;
        public event Action RemovedMessage;
        public List<EventMessage> Messages { get; set; }
        public List<EventMessage> GetMessages();
        public EventMessage? GetMessage(int id);
        public int LogMessage(string message, LogLevel level, string title, MessageSource source, MessageResult result, bool showPrograssBar);
        public void UpdateMessage(DateTime? time, int messageId, string? message, LogLevel? level, string? title, MessageSource? source, MessageResult? result, bool? showPrograssBar);
        public void AddMessage(EventMessage message);
        public string? Transform(bool toggle);
        public void ClearMessages();
        public void RemoveMessage(int id);
    }
}
