using Docker.DotNet.Models;
using FarmerbotWebUI.Client.Shared;
using FarmerbotWebUI.Shared;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Radzen;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using static System.Net.WebRequestMethods;

namespace FarmerbotWebUI.Client.Services.Docker
{
    public class DockerService : IDockerService
    {
        private readonly int cancelTimeout = 300000; // TODO: get from config
        private readonly int _farmerBotStatusInterval = 60; // TODO: get from config
        private readonly HttpClient _httpClient;
        private readonly IEventConsoleService _eventConsole;
        private readonly NotificationService _notificationService;
        private SemaphoreSlim _statusSemaphore = new SemaphoreSlim(1);
        private bool _lockInterval = false;
        private List<EventSourceActionId> _eventSourceActionIds = new List<EventSourceActionId>();

        public Dictionary<EventAction, CancellationTokenSource> CancellationTokens = new Dictionary<EventAction, CancellationTokenSource>();
        public event Action StatusChanged = () => { };
        public FarmerBotStatus ActualFarmerBotStatus { get; private set; } = new FarmerBotStatus { NoStatus = true };

        public DockerService(HttpClient httpClient, IEventConsoleService eventConsole, NotificationService notificationService)
        {
            _httpClient = httpClient;
            _eventConsole = eventConsole;
            _notificationService = notificationService;
            StartStatusInterval();
        }

        private void StartStatusInterval()
        {
            Timer timer = new Timer(async (e) =>
            {
                if (!_lockInterval)
                {
                    await _statusSemaphore.WaitAsync();
                    try
                    {
                        EventSourceActionId eventSourceActionId = new EventSourceActionId { Action = EventAction.FarmerBotStatus, Source = EventSource.DockerService, Typ = EventTyp.ClientJob};
                        
                        var response = await GetComposeStatusAsync(eventSourceActionId);
                    }
                    finally
                    {
                        _statusSemaphore.Release();
                    }
                }
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(_farmerBotStatusInterval));
        }

        public Task<bool> CancelOperation(EventAction command)
        {
            try
            {
                CancellationTokens.FirstOrDefault(c => c.Key == command).Value.Cancel();
                return Task.FromResult(true);
            }
            catch (Exception)
            {
                return Task.FromResult(false);
            }
        }

        public async Task<ServiceResponse<FarmerBotStatus>> StartComposeAsync(EventSourceActionId id)
        {
            var title = "Starting FarmerBot";
            var message = $"Starting FarmerBot...";
            var GuiAndProgress = id.Typ == EventTyp.UserAction ? true : false;
            id = _eventConsole.AddMessage(id, title, message, GuiAndProgress, false, GuiAndProgress, LogLevel.Information, EventResult.Valueless);

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(cancelTimeout);
            CancellationTokens.Add(EventAction.FarmerBotStart, cancellationTokenSource);
            var response = await _httpClient.GetFromJsonAsync<ServiceResponse<FarmerBotStatus>>("api/docker/start", cancellationTokenSource.Token);
            CancellationTokens.Remove(EventAction.FarmerBotStart);

            if (response.Success && response.Data.Status())
            {
                ActualFarmerBotStatus = response.Data;
                _eventConsole.UpdateMessage(id, title, response.Message, false, true, GuiAndProgress, LogLevel.Information, EventResult.Successfully);
                StatusChanged.Invoke();
            }
            else if (!response.Success)
            {
                ActualFarmerBotStatus = null;
                message = $"Error starting FarmerBot...\n{response.Message}";
                _eventConsole.UpdateMessage(id, title, message, false, true, true, LogLevel.Error, EventResult.Unsuccessfully);
                StatusChanged.Invoke();
            }
            else if (response.Success && !response.Data.Status())
            {
                ActualFarmerBotStatus = response.Data;
                message = $"{response.Message}";
                _eventConsole.UpdateMessage(id, title, message, false, true, true, LogLevel.Error, EventResult.Unsuccessfully);
                StatusChanged.Invoke();
            }

            return response;
        }

        public async Task<ServiceResponse<FarmerBotStatus>> StopComposeAsync(EventSourceActionId id)
        {
            var title = "Stopping FarmerBot";
            var message = $"Stopping FarmerBot...";
            var GuiAndProgress = id.Typ == EventTyp.UserAction ? true : false;
            id = _eventConsole.AddMessage(id, title, message, GuiAndProgress, false, GuiAndProgress, LogLevel.Information, EventResult.Valueless);

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(cancelTimeout);
            CancellationTokens.Add(EventAction.FarmerBotStop, cancellationTokenSource);
            var response = await _httpClient.GetFromJsonAsync<ServiceResponse<FarmerBotStatus>>("api/docker/stop", cancellationTokenSource.Token);
            CancellationTokens.Remove(EventAction.FarmerBotStop);

            if (response.Success && !response.Data.Status())
            {
                ActualFarmerBotStatus = response.Data;
                _eventConsole.UpdateMessage(id, title, response.Message, false, true, GuiAndProgress, LogLevel.Information, EventResult.Successfully);
                StatusChanged.Invoke();
            }
            else if (!response.Success)
            {
                ActualFarmerBotStatus = null;
                message = $"Error stopping FarmerBot...\n{response.Message}";
                _eventConsole.UpdateMessage(id, title, message, false, true, true, LogLevel.Error, EventResult.Unsuccessfully);
                StatusChanged.Invoke();
            }
            else if (response.Success && response.Data.Status())
            {
                ActualFarmerBotStatus = response.Data;
                message = $"{response.Message}";
                _eventConsole.UpdateMessage(id, title, message, false, true, true, LogLevel.Error, EventResult.Unsuccessfully);
                StatusChanged.Invoke();
            }

            return response;
        }

        public async Task<ServiceResponse<string>> GetComposeListAsync(EventSourceActionId id)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<string>> GetComposeLogsAsync(EventSourceActionId id)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<string>> GetComposeProcessesAsync(EventSourceActionId id)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<FarmerBotStatus>> GetComposeStatusAsync(EventSourceActionId id)
        {
            var title = "Getting FarmerBot status";
            var message = $"Getting FarmerBot status...";
            var GuiAndProgress = id.Typ == EventTyp.UserAction ? true : false;
            id = _eventConsole.AddMessage(id, title, message, GuiAndProgress, false, GuiAndProgress, LogLevel.Information, EventResult.Valueless);

            _lockInterval = true;
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(cancelTimeout);
            CancellationTokens.Add(EventAction.FarmerBotStatus, cancellationTokenSource);
            var response = await _httpClient.GetFromJsonAsync<ServiceResponse<FarmerBotStatus>> ("api/docker/status", cancellationTokenSource.Token);
            CancellationTokens.Remove(EventAction.FarmerBotStatus);
            if (response.Success && response.Data.Status())
            {
                ActualFarmerBotStatus = response.Data;
                _eventConsole.UpdateMessage(id, title, response.Message, false, true, GuiAndProgress, LogLevel.Information, EventResult.Successfully);
                StatusChanged.Invoke();
            }
            else if (!response.Success)
            {
                ActualFarmerBotStatus = response.Data;
                message = $"Error getting FarmerBot status...\n{response.Message}";
                _eventConsole.UpdateMessage(id, title, message, false, true, true, LogLevel.Error, EventResult.Unsuccessfully);
                StatusChanged.Invoke();
            }
            //else if (response.Success && !response.Data.Status())
            //{
            //    ActualFarmerBotStatus = response.Data;
            //    message = $"{response.Message}";
            //    _eventConsole.UpdateMessage(id, title, message, false, true, true, LogLevel.Error, EventResult.Unsuccessfully);
            //    StatusChanged.Invoke();
            //}

            _lockInterval = false;
            return response;
        }
    }
}