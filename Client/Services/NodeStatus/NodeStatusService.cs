using FarmerbotWebUI.Shared.BotConfig;
using FarmerbotWebUI.Shared.NodeStatus;
using FarmerBotWebUI.Shared;
using System.Net.Http.Json;
using System.Timers;

namespace FarmerbotWebUI.Client.Services.NodeStatus
{
    public class NodeStatusService : INodeStatusService
    {
        private readonly HttpClient _httpClient;
        private readonly IEventConsoleService _eventConsole;
        private IAppSettings _appSettings;
        private System.Timers.Timer _timer;

        public event Action StatusChanged = () => { };
        List<NodeStatusCollection> ActualNodeStatusCollectionList = new List<NodeStatusCollection>();
        public NodeStatusService(HttpClient httpClient, IEventConsoleService eventConsole, IAppSettings appSettings)
        {
            _httpClient = httpClient;
            _eventConsole = eventConsole;
            _appSettings = appSettings;
            _appSettings.OnAppSettingsChanged += UpdateAppSettings;

            _timer = new System.Timers.Timer();
            _timer.Interval = _appSettings.GeneralSettings.ApiCallInterval * 1000; // Set the interval to 1 second
            _timer.Elapsed += OnTimerElapsed;
            _timer.AutoReset = true;
        }

        private void UpdateAppSettings(object sender, AppSettings newAppSettings)
        {
            _appSettings.SaveSettings(newAppSettings);
            _timer.Interval = _appSettings.GeneralSettings.ApiCallInterval * 1000; // Set the interval to 1 second
        }

        public void StartStatusInterval()
        {
            _timer.Interval = _appSettings.GeneralSettings.ApiCallInterval * 1000; // Set the interval to 1 second
            _timer.Start();
        }

        private async void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            ServiceResponse<List<NodeStatusCollection>> response = new ServiceResponse<List<NodeStatusCollection>>();
            EventSourceActionId eventSourceActionId = new EventSourceActionId { Action = EventAction.GetSettings, Source = EventSource.SettingsService, Typ = EventTyp.ClientJob };
            response = await GetNodeStatusListAsync(eventSourceActionId);
        }

        public async Task<ServiceResponse<NodeStatusSet>> GetNodeStatusAsync(int nodeId, EventSourceActionId id)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<NodeStatusCollection>> GetNodeStatusCollectionAsync(EventSourceActionId id)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<List<NodeStatusCollection>>> GetNodeStatusListAsync(EventSourceActionId id)
        {
            _timer.Stop();
            // Event handling
            var title = "Getting Node status list";
            var message = $"Getting Node status list...";
            var GuiAndProgress = id.Typ == EventTyp.UserAction ? true : false;
            id = _eventConsole.AddMessage(id, title, message, GuiAndProgress, false, GuiAndProgress, LogLevel.Information, EventResult.Valueless);

            // Request
            var response = await _httpClient.GetFromJsonAsync<ServiceResponse<List<NodeStatusCollection>>>($"api/nodestatus/nodes/list", CancellationToken.None);

            // Response Error handling
            foreach (var item in response.Data)
            {
                ActualNodeStatusCollectionList.Remove(ActualNodeStatusCollectionList.Find(s => s.BotName == item.BotName));
                ActualNodeStatusCollectionList.Add(item);
                if (!item.NoStatus)
                {
                    _eventConsole.UpdateMessage(id, title, response.Message, false, true, GuiAndProgress, LogLevel.Information, EventResult.Successfully);
                }
                else if (!response.Success)
                {
                    message = $"Error getting Node status list...\n{response.Message}";
                    _eventConsole.UpdateMessage(id, title, message, false, true, true, LogLevel.Error, EventResult.Unsuccessfully);
                }
            }

            StatusChanged.Invoke();

            _timer.Start();
            return response;
        }
    }
}
