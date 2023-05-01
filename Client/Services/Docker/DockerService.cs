﻿using FarmerBotWebUI.Shared;
using Radzen;
using System.Net.Http.Json;

namespace FarmerbotWebUI.Client.Services.Docker
{
    public class DockerService : IDockerService
    {
        private readonly HttpClient _httpClient;
        private readonly IEventConsoleService _eventConsole;
        private IAppSettings _appSettings;
        private SemaphoreSlim _statusSemaphore = new SemaphoreSlim(1);
        private bool _lockInterval = false;

        public Dictionary<EventAction, CancellationTokenSource> CancellationTokens = new Dictionary<EventAction, CancellationTokenSource>();
        public event Action StatusChanged = () => { };
        public FarmerBotStatus ActualFarmerBotStatus { get; private set; } = new FarmerBotStatus { NoStatus = true };

        public DockerService(HttpClient httpClient, IEventConsoleService eventConsole, IAppSettings appSettings)
        {

            _httpClient = httpClient;
            _eventConsole = eventConsole;
            _appSettings = appSettings;
            _appSettings.OnAppSettingsChanged += UpdateAppSettings;
        }

        private void UpdateAppSettings(object sender, AppSettings newAppSettings)
        {
            _appSettings = newAppSettings;
        }

        public async Task<ServiceResponse<FarmerBotStatus>> StartStatusInterval()
        {
            TaskCompletionSource<ServiceResponse<FarmerBotStatus>> taskCompletionSource = new TaskCompletionSource<ServiceResponse<FarmerBotStatus>>();
            SemaphoreSlim firstExecutionSemaphore = new SemaphoreSlim(0, 1);

            Timer timer = new Timer(async (e) =>
            {
                if (!_lockInterval)
                {
                    await _statusSemaphore.WaitAsync();
                    try
                    {
                        EventSourceActionId eventSourceActionId = new EventSourceActionId { Action = EventAction.FarmerBotStatus, Source = EventSource.DockerService, Typ = EventTyp.ClientJob };
                        var response = await GetComposeStatusAsync(eventSourceActionId);

                        // Setzen Sie das Ergebnis des TaskCompletionSource, wenn die Semaphore noch nicht freigegeben wurde
                        if (firstExecutionSemaphore.CurrentCount == 0)
                        {
                            taskCompletionSource.SetResult(response);
                            firstExecutionSemaphore.Release();
                        }
                    }
                    finally
                    {
                        _statusSemaphore.Release();
                    }
                }
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(_appSettings.FarmerBotSettings.FarmerBotStatusInterval));

            // Warten Sie auf das Ergebnis des TaskCompletionSource, bevor Sie die Antwort zurückgeben
            return await taskCompletionSource.Task;
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

        public async Task<ServiceResponse<FarmerBotStatus>> StartComposeAsync(string botName, EventSourceActionId id)
        {
            var title = "Starting FarmerBot";
            var message = $"Starting FarmerBot...";
            var GuiAndProgress = id.Typ == EventTyp.UserAction ? true : false;
            id = _eventConsole.AddMessage(id, title, message, GuiAndProgress, false, GuiAndProgress, LogLevel.Information, EventResult.Valueless);

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(_appSettings.GeneralSettings.CancelationTimeout);
            CancellationTokens.Add(EventAction.FarmerBotStart, cancellationTokenSource);
            var response = await _httpClient.GetFromJsonAsync<ServiceResponse<FarmerBotStatus>>($"api/docker/start/{botName}",  cancellationTokenSource.Token);
            CancellationTokens.Remove(EventAction.FarmerBotStart);

            if (response.Success && response.Data.Status())
            {
                ActualFarmerBotStatus = response.Data;
                _eventConsole.UpdateMessage(id, title, response.Message, false, true, GuiAndProgress, LogLevel.Information, EventResult.Successfully);
            }
            else if (!response.Success)
            {
                ActualFarmerBotStatus = null;
                message = $"Error starting FarmerBot...\n{response.Message}";
                _eventConsole.UpdateMessage(id, title, message, false, true, true, LogLevel.Error, EventResult.Unsuccessfully);
            }
            else if (response.Success && !response.Data.Status())
            {
                ActualFarmerBotStatus = response.Data;
                message = $"{response.Message}";
                _eventConsole.UpdateMessage(id, title, message, false, true, true, LogLevel.Error, EventResult.Unsuccessfully);
            }
            StatusChanged.Invoke();

            return response;
        }

        public async Task<ServiceResponse<FarmerBotStatus>> StopComposeAsync(string botName, EventSourceActionId id)
        {
            var title = "Stopping FarmerBot";
            var message = $"Stopping FarmerBot...";
            var GuiAndProgress = id.Typ == EventTyp.UserAction ? true : false;
            id = _eventConsole.AddMessage(id, title, message, GuiAndProgress, false, GuiAndProgress, LogLevel.Information, EventResult.Valueless);

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(_appSettings.GeneralSettings.CancelationTimeout);
            CancellationTokens.Add(EventAction.FarmerBotStop, cancellationTokenSource);
            var response = await _httpClient.GetFromJsonAsync<ServiceResponse<FarmerBotStatus>>($"api/docker/stop/{botName}", cancellationTokenSource.Token);
            CancellationTokens.Remove(EventAction.FarmerBotStop);

            if (response.Success && !response.Data.Status())
            {
                ActualFarmerBotStatus = response.Data;
                _eventConsole.UpdateMessage(id, title, response.Message, false, true, GuiAndProgress, LogLevel.Information, EventResult.Successfully);
            }
            else if (!response.Success)
            {
                ActualFarmerBotStatus = null;
                message = $"Error stopping FarmerBot...\n{response.Message}";
                _eventConsole.UpdateMessage(id, title, message, false, true, true, LogLevel.Error, EventResult.Unsuccessfully);
            }
            else if (response.Success && response.Data.Status())
            {
                ActualFarmerBotStatus = response.Data;
                message = $"{response.Message}";
                _eventConsole.UpdateMessage(id, title, message, false, true, true, LogLevel.Error, EventResult.Unsuccessfully);
            }
            StatusChanged.Invoke();

            return response;
        }

        public async Task<ServiceResponse<string>> GetComposeListAsync(string botName, EventSourceActionId id)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<string>> GetComposeLogsAsync(string botName, EventSourceActionId id)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<string>> GetComposeProcessesAsync(string botName, EventSourceActionId id)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<FarmerBotStatus>> GetComposeStatusAsync(string botName, EventSourceActionId id)
        {
            var title = "Getting FarmerBot status";
            var message = $"Getting FarmerBot status...";
            var GuiAndProgress = id.Typ == EventTyp.UserAction ? true : false;
            id = _eventConsole.AddMessage(id, title, message, GuiAndProgress, false, GuiAndProgress, LogLevel.Information, EventResult.Valueless);

            _lockInterval = true;
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(_appSettings.GeneralSettings.CancelationTimeout);
            CancellationTokens.Add(EventAction.FarmerBotStatus, cancellationTokenSource);
            var response = await _httpClient.GetFromJsonAsync<ServiceResponse<FarmerBotStatus>>($"api/docker/status/{botName}", cancellationTokenSource.Token);
            CancellationTokens.Remove(EventAction.FarmerBotStatus);
            if (response.Success && response.Data.Status())
            {
                ActualFarmerBotStatus = response.Data;
                _eventConsole.UpdateMessage(id, title, response.Message, false, true, GuiAndProgress, LogLevel.Information, EventResult.Successfully);
            }
            else if (!response.Success)
            {
                ActualFarmerBotStatus = response.Data;
                message = $"Error getting FarmerBot status...\n{response.Message}";
                _eventConsole.UpdateMessage(id, title, message, false, true, true, LogLevel.Error, EventResult.Unsuccessfully);
            }
            //else if (response.Success && !response.Data.Status())
            //{
            //    ActualFarmerBotStatus = response.Data;
            //    message = $"{response.Message}";
            //    _eventConsole.UpdateMessage(id, title, message, false, true, true, LogLevel.Error, EventResult.Unsuccessfully);
            //    StatusChanged.Invoke();
            //}
            StatusChanged.Invoke();

            _lockInterval = false;
            return response;
        }

        public void Dispose()
        {
            _appSettings.OnAppSettingsChanged -= UpdateAppSettings;
        }
    }
}