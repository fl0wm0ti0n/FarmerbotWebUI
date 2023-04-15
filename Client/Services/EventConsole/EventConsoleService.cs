
using Docker.DotNet.Models;
using FarmerbotWebUI.Client.Shared;
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
        public event Action AddedMessage = () => { };
        public event Action UpdatedMessage = () => { };
        public event Action EventConsoleChanged = () => { };
        public event Action ClearedMessages = () => { };
        public event Action RemovedMessage = () => { };

        public List<EventMessage> GetMessages() => Messages;
        public EventMessage? GetMessage(int id) => Messages.FirstOrDefault(m => m.Id.Id == id);

        public EventConsoleService(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public EventSourceActionId AddMessage(EventMessage message)
        {
            _messageId++;
            message.Id.Id = _messageId;

            if (message.Typ == EventTyp.UserAction)
            {
                message.ShowInGui = true;
            }

            if (message.Done)
            {
                message.EndTime = DateTime.UtcNow;
                // TODO: send it to LogService
            }

            if (message.ShowInGui)
            {
                Messages.Add(message);
                _notificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = message, Detail = $"{_eventConsole.Transform(response.Success)}", Duration = 4000, CloseOnClick = true, Payload = DateTime.Now });

                EventConsoleChanged.Invoke();
                AddedMessage.Invoke();
            }

            return message.Id;
        }

        public EventSourceActionId AddMessage(EventSourceActionId id, string title, string message, bool showPrograssBar, bool done, bool showInGui, LogLevel level, EventResult result, EventTyp typ)
        {
            _messageId++;
            id.Id = _messageId;

            if (typ == EventTyp.UserAction)
            {
                showInGui = true;
            }

            DateTime? endTime = null;
            if (done)
            {
                endTime = DateTime.UtcNow;
                // TODO: send it to LogService 
            }

            if (showInGui)
            {
                Messages.Add(new EventMessage { Id = id, EndTime = endTime, Title = title, Message = message, ShowPrograssBar = showPrograssBar, Done = done, ShowInGui = showInGui, Severity = level, Typ = typ, Result = result });
                _notificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = message, Detail = $"{_eventConsole.Transform(response.Success)}", Duration = 4000, CloseOnClick = true, Payload = DateTime.Now });

                EventConsoleChanged.Invoke();
                AddedMessage.Invoke();
            }

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
                if (messageToUpdate.ShowInGui)
                {
                    Messages.Add(messageToUpdate);
                    EventConsoleChanged.Invoke();
                    UpdatedMessage.Invoke();
                }
            }
        }

        public void UpdateMessage(EventSourceActionId id, string? title, string? message, bool? showPrograssBar, bool? done, bool? showInGui, LogLevel? level, EventResult? result, EventTyp? typ)
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
                if (typ != null)
                {
                    messageToUpdate.Typ = (EventTyp)typ;
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
                if (messageToUpdate.ShowInGui)
                {
                    Messages.Add(messageToUpdate);
                    EventConsoleChanged.Invoke();
                    UpdatedMessage.Invoke();
                }
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
    }
}