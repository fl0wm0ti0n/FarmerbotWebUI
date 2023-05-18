using FarmerbotWebUI.Client.Shared;
using FarmerbotWebUI.Shared;
using FarmerbotWebUI.Shared.BotConfig;
using FarmerBotWebUI.Shared;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Timers;

namespace FarmerbotWebUI.Client.Services.Settings
{
    public class SettingsService : ISettingsService
    {
        private readonly HttpClient _httpClient;
        private readonly IEventConsoleService _eventConsole;
        private IAppSettings _appSettings;
        private System.Timers.Timer _timer;

        public event EventHandler<AppSettings> OnAppSettingsChanged;

        public SettingsService(HttpClient httpClient, IEventConsoleService eventConsole, IAppSettings appSettings)
        {
            _httpClient = httpClient;
            _eventConsole = eventConsole;
            _appSettings = appSettings; 
            _appSettings.OnAppSettingsChanged += UpdateAppSettings;

            _timer = new System.Timers.Timer();
            _timer.Interval = _appSettings.GeneralSettings.ServerUpdateInterval * 1000; // Set the interval to 1 second
            _timer.Elapsed += OnTimerElapsed;
            _timer.AutoReset = true;
        }

        private void UpdateAppSettings(object sender, AppSettings newAppSettings)
        {
            _appSettings.SaveSettings(newAppSettings);
            _timer.Interval = _appSettings.GeneralSettings.ServerUpdateInterval * 1000; // Set the interval to 1 second
        }

        public void StartStatusInterval()
        {
            _timer.Interval = _appSettings.GeneralSettings.ServerUpdateInterval * 1000; // Set the interval to 1 second
            _timer.Start();
        }

        private async void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            ServiceResponse<AppSettings> response = new ServiceResponse<AppSettings>();
            EventSourceActionId eventSourceActionId = new EventSourceActionId { Action = EventAction.GetSettings, Source = EventSource.SettingsService, Typ = EventTyp.ClientJob };
            response = await GetConfigurationObject(eventSourceActionId);
        }

        public async Task<ServiceResponse<AppSettings>> GetConfigurationObject(EventSourceActionId id)
        {
            _timer.Stop();

            var title = "Getting Settings";
            var message = $"Getting Settings...";
            var GuiAndProgress = id.Typ == EventTyp.UserAction ? true : false;
            id = _eventConsole.AddMessage(id, title, message, GuiAndProgress, false, GuiAndProgress, LogLevel.Information, EventResult.Valueless);

            var response = await _httpClient.GetFromJsonAsync<ServiceResponse<AppSettings>>("api/settings/getsettings");
            if (response.Success)
            {
                _appSettings.SaveSettings(response.Data);
                _appSettings.InvokeOnAppSettingsChanged();
                _eventConsole.UpdateMessage(id, title, response.Message, false, true, GuiAndProgress, LogLevel.Information, EventResult.Successfully);
            }
            else if (!response.Success)
            {
                _appSettings = response.Data;
                //OnAppSettingsChanged?.Invoke(this, response.Data);
                message = $"Error getting Settings...\n{response.Message}";
                _eventConsole.UpdateMessage(id, title, message, false, true, true, LogLevel.Error, EventResult.Unsuccessfully);
            }

            _timer.Start();
            return response;
        }

        public async Task<ServiceResponse<AppSettings>> SetConfigurationObject(AppSettings appSettings, EventSourceActionId id)
        {
            var title = "Updating Settings";
            var message = $"Updating Settings...";
            var GuiAndProgress = id.Typ == EventTyp.UserAction ? true : false;
            id = _eventConsole.AddMessage(id, title, message, GuiAndProgress, false, GuiAndProgress, LogLevel.Information, EventResult.Valueless);

            var response = await _httpClient.GetFromJsonAsync<ServiceResponse<AppSettings>>("api/settings/setsettings");
            if (response.Success)
            {
                _appSettings.SaveSettings(response.Data);
                _appSettings.InvokeOnAppSettingsChanged();
                _eventConsole.UpdateMessage(id, title, response.Message, false, true, GuiAndProgress, LogLevel.Information, EventResult.Successfully);
            }
            else if (!response.Success)
            {
                message = $"Error updating Settings...\n{response.Message}";
                _eventConsole.UpdateMessage(id, title, message, false, true, true, LogLevel.Error, EventResult.Unsuccessfully);
            }
            return response;
        }
    }
}
