
namespace FarmerbotWebUI.Client.Services.EventConsole
{
    public class EventConsoleService : IEventConsoleService
    {
        private int _messageId = 0;
        public List<EventMessage> Messages { get; set; } = new List<EventMessage>();
        public event Action NewMessage = () => { };
        public event Action AddedMessage = () => { };
        public event Action UpdatedMessage = () => { };
        public event Action EventConsoleChanged = () => { };
        public event Action ClearedMessages = () => { };
        public event Action RemovedMessage = () => { };

        public List<EventMessage> GetMessages() => Messages;
        public EventMessage? GetMessage(int id) => Messages.FirstOrDefault(m => m.Id == id);

        public int LogMessage(string message, LogLevel level, string title, MessageSource source, MessageResult result, ActionType action, bool showPrograssBar)
        {
            _messageId++;
            Messages.Add(new EventMessage { Id = _messageId, Timestamp = DateTime.Now, Message = message, Severity = level, Title = title, Source = source, Result = result, ShowPrograssBar = showPrograssBar });
            EventConsoleChanged.Invoke();
            NewMessage.Invoke();
            return _messageId;
        }

        public void UpdateMessage(DateTime? time, int messageId, string? message, LogLevel? level, string? title, MessageSource? source, MessageResult? result, ActionType action, bool? showPrograssBar)
        {
            var messageToUpdate = Messages.FirstOrDefault(m => m.Id == messageId);

            if (messageToUpdate != null)
            {
                Messages.Remove(messageToUpdate);

                if (message != null)
                {
                    messageToUpdate.Message = message;
                }
                if (level != null)
                {
                    messageToUpdate.Severity = (LogLevel)level;
                }
                if (showPrograssBar != null)
                {
                    messageToUpdate.ShowPrograssBar = (bool)showPrograssBar;
                }
                if (title != null)
                {
                    messageToUpdate.Title = title;
                }
                if (source != null)
                {
                    messageToUpdate.Source = (MessageSource)source;
                }
                if (result != null)
                {
                    messageToUpdate.Result = (MessageResult)result;
                }
                if (time != null)
                {
                    messageToUpdate.Timestamp = DateTime.Now;
                }
                Messages.Add(messageToUpdate);
                EventConsoleChanged.Invoke();
                UpdatedMessage.Invoke();
            }
        }

        public void AddMessage(EventMessage message)
        {
            Messages.Add(message);
            EventConsoleChanged.Invoke();
            AddedMessage.Invoke();
        }

        public void RemoveMessage(int messageId)
        {
            var messageToUpdate = Messages.FirstOrDefault(m => m.Id == messageId);

            if (messageToUpdate != null)
            {
                Messages.Remove(messageToUpdate);
            };
            EventConsoleChanged.Invoke();
            RemovedMessage.Invoke();
        }

        public void ClearMessages()
        {
            Messages.Clear();
            EventConsoleChanged.Invoke();
            ClearedMessages.Invoke();
        }

        public string? Transform(bool toggle)
        {
            return toggle ? "successfully" : "unsuccessfully";
        }
    }
}