using FarmerbotWebUI.Client.Shared;
using FarmerbotWebUI.Shared;
using FarmerBotWebUI.Shared;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;

namespace FarmerbotWebUI.Client.Services.Settings
{
    public class SettingsService : ISettingsService
    {
        private readonly HttpClient _httpClient;
        private readonly IEventConsoleService _eventConsole;
        private readonly ISettingsService _settingsService;
        private SemaphoreSlim _statusSemaphore = new SemaphoreSlim(1);
        private bool _lockInterval = false;

        public AppSettings AppSetting { get; private set; } = new AppSettings();

        public SettingsService(HttpClient httpClient, IEventConsoleService eventConsole, ISettingsService settingsService)
        {
            _httpClient = httpClient;
            _eventConsole = eventConsole;
            _settingsService = settingsService;
        }

        public async Task<ServiceResponse<AppSettings>> StartStatusInterval()
        {
            TaskCompletionSource<ServiceResponse<AppSettings>> taskCompletionSource = new TaskCompletionSource<ServiceResponse<AppSettings>>();
            SemaphoreSlim firstExecutionSemaphore = new SemaphoreSlim(0, 1);

            Timer timer = new Timer(async (e) =>
            {
                if (!_lockInterval)
                {
                    await _statusSemaphore.WaitAsync();
                    try
                    {
                        EventSourceActionId eventSourceActionId = new EventSourceActionId { Action = EventAction.GetSettings, Source = EventSource.SettingsService, Typ = EventTyp.ClientJob };
                        var response = await GetConfigurationObject(eventSourceActionId);

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
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(_settingsService.AppSetting.GeneralSettings.ServerUpdateInterval));

            // Warten Sie auf das Ergebnis des TaskCompletionSource, bevor Sie die Antwort zurückgeben
            return await taskCompletionSource.Task;
        }

        public async Task<ServiceResponse<AppSettings>> GetConfigurationObject(EventSourceActionId id)
        {
            var title = "Getting Settings";
            var message = $"Getting Settings...";
            var GuiAndProgress = id.Typ == EventTyp.UserAction ? true : false;
            id = _eventConsole.AddMessage(id, title, message, GuiAndProgress, false, GuiAndProgress, LogLevel.Information, EventResult.Valueless);

            _lockInterval = true;
            var response = await _httpClient.GetFromJsonAsync<ServiceResponse<AppSettings>>("api/settings/getsettings");
            if (response.Success)
            {
                AppSetting = response.Data;
                _eventConsole.UpdateMessage(id, title, response.Message, false, true, GuiAndProgress, LogLevel.Information, EventResult.Successfully);
            }
            else if (!response.Success)
            {
                AppSetting = response.Data;
                message = $"Error getting Settings...\n{response.Message}";
                _eventConsole.UpdateMessage(id, title, message, false, true, true, LogLevel.Error, EventResult.Unsuccessfully);
            }
            _lockInterval = false;
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
                //AppSetting = response.Data;
                _eventConsole.UpdateMessage(id, title, response.Message, false, true, GuiAndProgress, LogLevel.Information, EventResult.Successfully);
            }
            else if (!response.Success)
            {
                //AppSetting = response.Data;
                message = $"Error updating Settings...\n{response.Message}";
                _eventConsole.UpdateMessage(id, title, message, false, true, true, LogLevel.Error, EventResult.Unsuccessfully);
            }
            return response;
        }
    }
}
