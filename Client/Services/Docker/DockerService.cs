using FarmerbotWebUI.Shared.BotConfig;
using FarmerBotWebUI.Shared;
using Radzen;
using System.Net.Http.Json;
using System.Timers;

namespace FarmerbotWebUI.Client.Services.Docker
{
    public class DockerService : IDockerService
    {
        private readonly HttpClient _httpClient;
        private readonly IEventConsoleService _eventConsole;
        private IAppSettings _appSettings;
        private System.Timers.Timer _timer;

        public Dictionary<EventAction, CancellationTokenSource> CancellationTokens = new Dictionary<EventAction, CancellationTokenSource>();
        public event Action StatusChanged = () => { };
        public List<FarmerBotStatus> ActualFarmerBotStatusList { get; private set; } = new List<FarmerBotStatus>();

        public DockerService(HttpClient httpClient, IEventConsoleService eventConsole, IAppSettings appSettings)
        {
            _httpClient = httpClient;
            _eventConsole = eventConsole;
            _appSettings = appSettings;
            _appSettings.OnAppSettingsChanged += UpdateAppSettings;

            _timer = new System.Timers.Timer();
            _timer.Interval = _appSettings.FarmerBotSettings.FarmerBotStatusInterval * 1000; // Set the interval to 1 second
            _timer.Elapsed += OnTimerElapsed;
            _timer.AutoReset = true;
        }

        private void UpdateAppSettings(object sender, AppSettings newAppSettings)
        {
            _appSettings.SaveSettings(newAppSettings);
            _timer.Interval = _appSettings.FarmerBotSettings.FarmerBotStatusInterval * 1000; // Set the interval to 1 second
        }

        public void StartStatusInterval()
        {
            _timer.Interval = _appSettings.FarmerBotSettings.FarmerBotStatusInterval * 1000; // Set the interval to 1 second
            _timer.Start();
        }

        private async void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            ServiceResponse<List<FarmerBotStatus>> response = new ServiceResponse<List<FarmerBotStatus>>();
            EventSourceActionId eventSourceActionId = new EventSourceActionId { Action = EventAction.FarmerBotStatus, Source = EventSource.DockerService, Typ = EventTyp.ClientJob };
            response = await GetComposeStatusListAsync(eventSourceActionId);
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
            // Event handling
            var title = "Starting FarmerBot";
            var message = $"Starting FarmerBot...";
            var GuiAndProgress = id.Typ == EventTyp.UserAction ? true : false;
            id = _eventConsole.AddMessage(id, title, message, GuiAndProgress, false, GuiAndProgress, LogLevel.Information, EventResult.Valueless);

            // Cancelation handling
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(_appSettings.GeneralSettings.CancelationTimeout);
            CancellationTokens.Add(EventAction.FarmerBotStart, cancellationTokenSource);

            // Request
            var response = await _httpClient.GetFromJsonAsync<ServiceResponse<FarmerBotStatus>>($"api/docker/start/{botName}",  cancellationTokenSource.Token);

            CancellationTokens.Remove(EventAction.FarmerBotStart);

            ActualFarmerBotStatusList.Remove(ActualFarmerBotStatusList.Find(s => s.Name == response.Data.Name));
            ActualFarmerBotStatusList.Add(response.Data);
            // Response Error handling
            if (response.Success && response.Data.Status())
            {
                _eventConsole.UpdateMessage(id, title, response.Message, false, true, GuiAndProgress, LogLevel.Information, EventResult.Successfully);
            }
            else if (!response.Success)
            {
                message = $"Error starting FarmerBot...\n{response.Message}";
                _eventConsole.UpdateMessage(id, title, message, false, true, true, LogLevel.Error, EventResult.Unsuccessfully);
            }
            else if (response.Success && !response.Data.Status())
            {
                _eventConsole.UpdateMessage(id, title, response.Message, false, true, true, LogLevel.Error, EventResult.Unsuccessfully);
            }

            StatusChanged.Invoke();

            return response;
        }

        public async Task<ServiceResponse<FarmerBotStatus>> StopComposeAsync(string botName, EventSourceActionId id)
        {
            // Event handling
            var title = "Stopping FarmerBot";
            var message = $"Stopping FarmerBot...";
            var GuiAndProgress = id.Typ == EventTyp.UserAction ? true : false;
            id = _eventConsole.AddMessage(id, title, message, GuiAndProgress, false, GuiAndProgress, LogLevel.Information, EventResult.Valueless);

            // Cancelation handling
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(_appSettings.GeneralSettings.CancelationTimeout);
            CancellationTokens.Add(EventAction.FarmerBotStop, cancellationTokenSource);

            // Request
            var response = await _httpClient.GetFromJsonAsync<ServiceResponse<FarmerBotStatus>>($"api/docker/stop/{botName}", cancellationTokenSource.Token);

            CancellationTokens.Remove(EventAction.FarmerBotStop);

            ActualFarmerBotStatusList.Remove(ActualFarmerBotStatusList.Find(s => s.Name == response.Data.Name));
            ActualFarmerBotStatusList.Add(response.Data);
            // Response Error handling
            if (response.Success && !response.Data.Status())
            {
                _eventConsole.UpdateMessage(id, title, response.Message, false, true, GuiAndProgress, LogLevel.Information, EventResult.Successfully);
            }
            else if (!response.Success)
            {
                message = $"Error stopping FarmerBot...\n{response.Message}";
                _eventConsole.UpdateMessage(id, title, message, false, true, true, LogLevel.Error, EventResult.Unsuccessfully);
            }
            else if (response.Success && response.Data.Status())
            {
                _eventConsole.UpdateMessage(id, title, response.Message, false, true, true, LogLevel.Error, EventResult.Unsuccessfully);
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
            _timer.Stop();
            // Event handling
            var title = "Getting FarmerBot status";
            var message = $"Getting FarmerBot status...";
            var GuiAndProgress = id.Typ == EventTyp.UserAction ? true : false;
            id = _eventConsole.AddMessage(id, title, message, GuiAndProgress, false, GuiAndProgress, LogLevel.Information, EventResult.Valueless);

            // Cancelation handling
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(_appSettings.GeneralSettings.CancelationTimeout);
            CancellationTokens.Add(EventAction.FarmerBotStatus, cancellationTokenSource);

            // Request
            var response = await _httpClient.GetFromJsonAsync<ServiceResponse<FarmerBotStatus>>($"api/docker/status/{botName}", cancellationTokenSource.Token);

            CancellationTokens.Remove(EventAction.FarmerBotStatus);

            ActualFarmerBotStatusList.Remove(ActualFarmerBotStatusList.Find(s => s.Name == response.Data.Name));
            ActualFarmerBotStatusList.Add(response.Data);
            // Response Error handling
            if (response.Success && response.Data.Status())
            {
                _eventConsole.UpdateMessage(id, title, response.Message, false, true, GuiAndProgress, LogLevel.Information, EventResult.Successfully);
            }
            else if (!response.Success)
            {
                message = $"Error getting FarmerBot status...\n{response.Message}";
                _eventConsole.UpdateMessage(id, title, message, false, true, true, LogLevel.Error, EventResult.Unsuccessfully);
            }

            StatusChanged.Invoke();

            _timer.Start();
            return response;
        }

        public async Task<ServiceResponse<List<FarmerBotStatus>>> GetComposeStatusListAsync(EventSourceActionId id)
        {
            _timer.Stop();
            // Event handling
            var title = "Getting FarmerBot status list";
            var message = $"Getting FarmerBot status list...";
            var GuiAndProgress = id.Typ == EventTyp.UserAction ? true : false;
            id = _eventConsole.AddMessage(id, title, message, GuiAndProgress, false, GuiAndProgress, LogLevel.Information, EventResult.Valueless);

            // Cancelation handling
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(_appSettings.GeneralSettings.CancelationTimeout);
            CancellationTokens.Add(EventAction.FarmerBotStatusList, cancellationTokenSource);

            // Request
            var response = await _httpClient.GetFromJsonAsync<ServiceResponse<List<FarmerBotStatus>>>($"api/docker/status", cancellationTokenSource.Token);

            CancellationTokens.Remove(EventAction.FarmerBotStatusList);

            // Response Error handling
            foreach (var item in response.Data)
            {
                ActualFarmerBotStatusList.Remove(ActualFarmerBotStatusList.Find(s => s.Name == item.Name));
                ActualFarmerBotStatusList.Add(item);
                if (item.Status())
                {
                    _eventConsole.UpdateMessage(id, title, response.Message, false, true, GuiAndProgress, LogLevel.Information, EventResult.Successfully);
                }
                else if (!response.Success)
                {
                    message = $"Error getting FarmerBot status list...\n{response.Message}";
                    _eventConsole.UpdateMessage(id, title, message, false, true, true, LogLevel.Error, EventResult.Unsuccessfully);
                }
            }

            StatusChanged.Invoke();

            _timer.Start();
            return response;
        }

        public void Dispose()
        {
            _appSettings.OnAppSettingsChanged -= UpdateAppSettings;
            _timer.Dispose();
        }
    }
}