
using Docker.DotNet.Models;
using FarmerbotWebUI.Client.Shared;
using FarmerBotWebUI.Shared;
using Radzen;
using System;
using System.Reflection.Emit;

namespace FarmerbotWebUI.Client.Services.EventConsole
{
    public class EventConsoleService : IEventConsoleService
    {
        private int _messageId = 0;
        public List<EventMessage> Messages { get; set; } = new List<EventMessage>();
        private readonly NotificationService _notificationService;
        private IAppSettings _appSettings;

        public event Action AddedMessage = () => { };
        public event Action UpdatedMessage = () => { };
        public event Action EventConsoleChanged = () => { };
        public event Action ClearedMessages = () => { };
        public event Action RemovedMessage = () => { };

        public List<EventMessage> GetMessages() => Messages;
        public EventMessage? GetMessage(int id) => Messages.FirstOrDefault(m => m.Id.Id == id);

        public EventConsoleService(NotificationService notificationService, IAppSettings appSettings)
        {
            _notificationService = notificationService;
            _appSettings = appSettings;
            _appSettings.OnAppSettingsChanged += UpdateAppSettings;
        }

        private void UpdateAppSettings(object sender, AppSettings newAppSettings)
        {
            _appSettings = newAppSettings;
        }

        public EventSourceActionId AddMessage(EventMessage message)
        {
            _messageId++;
            message.Id.Id = _messageId;

            if (message.Id.Typ == EventTyp.UserAction)
            {
                message.ShowInGui = true;
            }

            if (message.Done)
            {
                message.EndTime = DateTime.UtcNow;
                // TODO: send it to LogService
            }

            Messages.Add(message);
            if (_appSettings.NotificationSettings.GuiNotification && message.ShowInGui)
                _notificationService.Notify(new NotificationMessage { Severity = LogLevelStyleMapper.LogLevelToNotificationSeverity(message.Severity) , Summary = message.Message, Detail = $"{message.Result}", Duration = 4000, CloseOnClick = true, Payload = DateTime.Now });

            EventConsoleChanged.Invoke();
            AddedMessage.Invoke();

            return message.Id;
        }

        public EventSourceActionId AddMessage(EventSourceActionId id, string title, string message, bool? showPrograssBar, bool? done, bool? showInGui, LogLevel level, EventResult result)
        {
            _messageId++;
            id.Id = _messageId;

            if (id.Typ == EventTyp.UserAction)
            {
                showInGui = true;
            }

            DateTime? endTime = null;
            if (done != null)
            {
                if ((bool)done)
                {
                    endTime = DateTime.UtcNow;
                    // TODO: send it to LogService 
                }
            }

            if (showInGui == null)
            {
                showInGui = false;
            }

            Messages.Add(new EventMessage { Id = id, EndTime = endTime, Title = title, Message = message, ShowPrograssBar = (bool)showPrograssBar, Done = (bool)done, ShowInGui = (bool)showInGui, Severity = level, Result = result });
            if (_appSettings.NotificationSettings.GuiNotification && (bool)showInGui)
                _notificationService.Notify(new NotificationMessage { Severity = LogLevelStyleMapper.LogLevelToNotificationSeverity(level), Summary = message, Detail = $"{result}", Duration = 4000, CloseOnClick = true, Payload = DateTime.Now });

            EventConsoleChanged.Invoke();
            AddedMessage.Invoke();

            return id;
        }

        public void UpdateMessage(EventMessage message)
        {
            var id = message.Id;
            var messageToUpdate = Messages.FirstOrDefault(m => m.Id.Id == id.Id && m.Id.Source == id.Source && m.Id.Action == id.Action);
            if (messageToUpdate != null)
            {
                Messages.Remove(messageToUpdate);
                messageToUpdate = message;
                if (messageToUpdate.Done)
                {
                    messageToUpdate.EndTime = DateTime.UtcNow;
                    // TODO: send it to LogService
                }

                Messages.Add(messageToUpdate);
                if (_appSettings.NotificationSettings.GuiNotification && message.ShowInGui)
                    _notificationService.Notify(new NotificationMessage { Severity = LogLevelStyleMapper.LogLevelToNotificationSeverity(message.Severity), Summary = message.Message, Detail = $"{message.Result}", Duration = 4000, CloseOnClick = true, Payload = DateTime.Now });

                EventConsoleChanged.Invoke();
                UpdatedMessage.Invoke();
            }
        }

        public void UpdateMessage(EventSourceActionId id, string? title, string? message, bool? showPrograssBar, bool? done, bool? showInGui, LogLevel? level, EventResult? result)
        {
            var messageToUpdate = Messages.FirstOrDefault(m => m.Id.Id == id.Id && m.Id.Source == id.Source && m.Id.Action == id.Action);

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
                if (result != null)
                {
                    messageToUpdate.Result = (EventResult)result;
                }
                if (done != null)
                {
                    messageToUpdate.Done = (bool)done;
                    if ((bool)done)
                    {
                        messageToUpdate.EndTime = DateTime.UtcNow;
                        // TODO: send it to LogService
                    }
                }
                if (showInGui != null)
                {
                    messageToUpdate.ShowInGui = (bool)showInGui;
                }

                Messages.Add(messageToUpdate);
                if (_appSettings.NotificationSettings.GuiNotification && (bool)showInGui)
                    _notificationService.Notify(new NotificationMessage { Severity = LogLevelStyleMapper.LogLevelToNotificationSeverity(messageToUpdate.Severity), Summary = messageToUpdate.Message, Detail = $"{messageToUpdate.Result}", Duration = 4000, CloseOnClick = true, Payload = DateTime.Now });

                EventConsoleChanged.Invoke();
                UpdatedMessage.Invoke();
            }
        }

        public void RemoveMessage(EventSourceActionId id)
        {
            throw new NotImplementedException();
        }

        public void RemoveMessage(int messageId)
        {
            var messageToUpdate = Messages.FirstOrDefault(m => m.Id.Id == messageId);

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

        public void Dispose()
        {
            _appSettings.OnAppSettingsChanged -= UpdateAppSettings;
        }
    }
}