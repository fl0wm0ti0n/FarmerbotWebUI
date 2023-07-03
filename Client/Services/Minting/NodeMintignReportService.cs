using FarmerbotWebUI.Shared.BotConfig;
using FarmerbotWebUI.Shared.NodeStatus;
using FarmerBotWebUI.Shared;
using System.Net.Http.Json;
using System.Timers;

namespace FarmerbotWebUI.Client.Services.Minting
{
    public class NodeMintignReportService : INodeMintingReportService
    {
        private readonly HttpClient _httpClient;
        private readonly IEventConsoleService _eventConsole;
        private IAppSettings _appSettings;
        private System.Timers.Timer _timer;

        public event Action StatusChanged = () => { };
        public List<NodeMintingCollection> ActualMintingReportCollectionList { get; private set; } = new List<NodeMintingCollection>();

        public NodeMintignReportService(HttpClient httpClient, IEventConsoleService eventConsole, IAppSettings appSettings)
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
            ServiceResponse<List<NodeMintingCollection>> response = new ServiceResponse<List<NodeMintingCollection>>();
            EventSourceActionId eventSourceActionId = new EventSourceActionId { Action = EventAction.GetNodeMinting, Source = EventSource.NodeMintingReportService, Typ = EventTyp.ClientJob };
            response = await GetNodeMintingReportListAsync(eventSourceActionId);
        }

        public async Task<ServiceResponse<MintingReport>> GetNodeMintingReportAsync(int nodeId, EventSourceActionId id)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<NodeMintingCollection>> GetNodeMintingReportCollectionAsync(EventSourceActionId id)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<List<NodeMintingCollection>>> GetNodeMintingReportListAsync(EventSourceActionId id)
        {
            _timer.Stop();
            // Event handling
            var title = "Getting Node minting report list";
            var message = $"Getting Node minting report list...";
            var GuiAndProgress = id.Typ == EventTyp.UserAction ? true : false;
            id = _eventConsole.AddMessage(id, title, message, GuiAndProgress, false, GuiAndProgress, LogLevel.Information, EventResult.Valueless);

            // Request
            var response = await _httpClient.GetFromJsonAsync<ServiceResponse<List<NodeMintingCollection>>>($"api/nodeminting/nodes/list", CancellationToken.None);

            // Response Error handling
            foreach (var item in response.Data)
            {
                ActualMintingReportCollectionList.Remove(ActualMintingReportCollectionList.Find(s => s.BotName == item.BotName));
                ActualMintingReportCollectionList.Add(item);
                if (!item.NoStatus)
                {
                    _eventConsole.UpdateMessage(id, title, response.Message, false, true, GuiAndProgress, LogLevel.Information, EventResult.Successfully);
                }
                else if (!response.Success)
                {
                    message = $"Error getting Node minting report list...\n{response.Message}";
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
