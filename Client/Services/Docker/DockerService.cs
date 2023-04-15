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

        public Dictionary<ActionType, CancellationTokenSource> CancellationTokens = new Dictionary<ActionType, CancellationTokenSource>();
        public event Action StatusChanged = () => { };

        enum DockerServiceActions
        {
            Start,
            Stop,
            Status,
        }

        public FarmerBotStatus? ActualFarmerBotStatus { get; private set; } = null;
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
                        EventSourceActionId eventSourceActionId = new EventSourceActionId { Action = EventAction.Status, Source = EventSource.DockerService };
                        _eventSourceActionIds.Add(eventSourceActionId);
                        var response = await GetComposeStatusAsync(eventSourceActionId);
                        if (!response.Success && response.Data == null)
                        {
                            ActualFarmerBotStatus = null;
                            var message = $"Error getting FarmerBot status: {response.Message}. The Docker-Engine may not reachable.";
                            _eventConsole.AddMessage(eventSourceActionId, message, LogLevel.Error, "Getting FarmerBot status...", MessageSource.SystemEvent, MessageResult.Unsuccessfully, ActionType.DockerStatus, false);
                        }
                    }
                    finally
                    {
                        _statusSemaphore.Release();
                    }
                }
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(_farmerBotStatusInterval));
        }

        public Task<bool> CancelOperation(ActionType command)
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
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(cancelTimeout);
            CancellationTokens.Add(ActionType.DockerStart, cancellationTokenSource);
            var response = await _httpClient.GetFromJsonAsync<ServiceResponse<FarmerBotStatus>>("api/docker/start", cancellationTokenSource.Token);
            CancellationTokens.Remove(ActionType.DockerStart);
            if (response.Success && response.Data != null)
            {
                ActualFarmerBotStatus = response.Data;
                StatusChanged.Invoke();
            }
            return response;
        }

        public async Task<ServiceResponse<FarmerBotStatus>> StopComposeAsync(EventSourceActionId id)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(cancelTimeout);
            CancellationTokens.Add(ActionType.DockerStop, cancellationTokenSource);
            var response = await _httpClient.GetFromJsonAsync<ServiceResponse<FarmerBotStatus>>("api/docker/stop", cancellationTokenSource.Token);
            CancellationTokens.Remove(ActionType.DockerStop);
            if (response.Success && response.Data != null)
            {
                ActualFarmerBotStatus = response.Data;
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
            _lockInterval = true;
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(cancelTimeout);
            CancellationTokens.Add(ActionType.DockerStatus, cancellationTokenSource);
            var response = await _httpClient.GetFromJsonAsync<ServiceResponse<FarmerBotStatus>> ("api/docker/status", cancellationTokenSource.Token);
            CancellationTokens.Remove(ActionType.DockerStatus);
            if (response.Success && response.Data != null)
            {
                ActualFarmerBotStatus = response.Data;
                StatusChanged.Invoke();
            }
            _lockInterval = false;
            return response;
        }
    }
}