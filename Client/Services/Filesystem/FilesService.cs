using FarmerbotWebUI.Client.Shared;
using FarmerbotWebUI.Shared.BotConfig;
using FarmerbotWebUI.Shared.NodeStatus;
using FarmerBotWebUI.Shared;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Timers;
using static System.Net.WebRequestMethods;

namespace FarmerbotWebUI.Client.Services.Filesystem
{
    public class FilesService : IFileService
    {

        private readonly HttpClient _httpClient;
        private readonly IEventConsoleService _eventConsole;
        private IAppSettings _appSettings;
        private System.Timers.Timer _timer;

        public event Action StatusChanged = () => { };

        public List<FarmerBot> ActualFarmerBotList { get; private set; } = new List<FarmerBot>();   
        public FarmerBot NewFarmerBot { get; set; } = new FarmerBot();

        public FilesService(HttpClient httpClient, IEventConsoleService eventConsole, IAppSettings appSettings)
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
            _timer.Interval = _appSettings.GeneralSettings.ServerUpdateInterval * 1000; // Set the interval to 1 second
            _timer.Start();
        }

        private async void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            ServiceResponse<List<FarmerBot>> response = new ServiceResponse<List<FarmerBot>>();
            EventSourceActionId eventSourceActionId = new EventSourceActionId { Action = EventAction.GetFarmerBot, Source = EventSource.FileService, Typ = EventTyp.ClientJob };
            response = await GetFarmerBotListAsync(eventSourceActionId);
        }

        #region Misc
        public async Task<ServiceResponse<string>> GetLocalLogAsync(string botName, EventSourceActionId id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        #endregion Misc

        #region Markdown
        public async Task<ServiceResponse<string>> GetRawMarkdownConfigAsync(string botName, EventSourceActionId id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<FarmerBotConfig>> GetMarkdownConfigAsync(string botName, EventSourceActionId id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<string>> SetRawMarkdownConfigAsync(string config, string botName, EventSourceActionId id, CancellationToken cancellationToken)
        {
            var content = new StringContent(config, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"api/filesystem/markdown/set/{botName}", content);

            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ServiceResponse<string>>(responseContent);

            return result;
        }

        public async Task<ServiceResponse<string>> SetMarkdownConfigAsync(FarmerBotConfig config, string botName, EventSourceActionId id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        #endregion Markdown

        #region Compose
        public async Task<ServiceResponse<string>> GetRawComposeFileAsync(string botName, EventSourceActionId id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<DockerCompose>> GetComposeFileAsync(string botName, EventSourceActionId id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<string>> SetRawComposeFileAsync(string compose, string botName, EventSourceActionId id, CancellationToken cancellationToken)
        {
            var content = new StringContent(compose, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"api/filesystem/compose/set/{botName}", content);

            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ServiceResponse<string>>(responseContent);

            return result;
        }

        public async Task<ServiceResponse<string>> SetComposeFileAsync(DockerCompose compose, string botName, EventSourceActionId id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        #endregion Compose

        #region Env
        public async Task<ServiceResponse<string>> GetRawEnvFileAsync(string botName, EventSourceActionId id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();

        }

        public async Task<ServiceResponse<EnvFile>> GetEnvFileAsync(string botName, EventSourceActionId id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<string>> SetRawEnvFileAsync(string env, string botName, EventSourceActionId id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<string>> SetEnvFileAsync(EnvFile env, string botName, EventSourceActionId id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        #endregion Env

        #region FarmerBot
        public async Task<ServiceResponse<List<FarmerBot>>> GetFarmerBotListAsync(EventSourceActionId id)
        {
            _timer.Stop();
            var title = "Getting FarmerBot list";
            var message = $"Getting FarmerBot list...";
            var GuiAndProgress = id.Typ == EventTyp.UserAction ? true : false;
            id = _eventConsole.AddMessage(id, title, message, GuiAndProgress, false, GuiAndProgress, LogLevel.Information, EventResult.Valueless);

            var response = await _httpClient.GetFromJsonAsync<ServiceResponse<List<FarmerBot>>>($"api/filesystem/bot/get");

            // Response Error handling
            foreach (var item in response.Data)
            {
                ActualFarmerBotList.Remove(ActualFarmerBotList.Find(s => s.Name == item.Name));
                ActualFarmerBotList.Add(item);
                if (response.Success)
                {
                    _eventConsole.UpdateMessage(id, title, response.Message, false, true, GuiAndProgress, LogLevel.Information, EventResult.Successfully);
                }
                else if (!response.Success)
                {
                    message = $"Error getting FarmerBot list...\n{response.Message}";
                    _eventConsole.UpdateMessage(id, title, message, false, true, true, LogLevel.Error, EventResult.Unsuccessfully);
                }
            }
            StatusChanged.Invoke();

            _timer.Start();
            return response;
        }

        public async Task<ServiceResponse<FarmerBot>> GetFarmerBotAsync(string botName, EventSourceActionId id, CancellationToken cancellationToken)
        {
            var title = "Getting FarmerBot Files";
            var message = $"Getting FarmerBot Files...";
            var GuiAndProgress = id.Typ == EventTyp.UserAction ? true : false;
            id = _eventConsole.AddMessage(id, title, message, GuiAndProgress, false, GuiAndProgress, LogLevel.Information, EventResult.Valueless);

            var response = await _httpClient.GetFromJsonAsync<ServiceResponse<FarmerBot>>($"api/filesystem/bot/get/{botName}");
            if (response.Success)
            {
                ActualFarmerBotList.Add(response.Data);
                _eventConsole.UpdateMessage(id, title, response.Message, false, true, GuiAndProgress, LogLevel.Information, EventResult.Successfully);
            }
            else if (!response.Success)
            {
                message = $"Error getting FarmerBot Files...\n{response.Message}";
                _eventConsole.UpdateMessage(id, title, message, false, true, true, LogLevel.Error, EventResult.Unsuccessfully);
            }
            return response;
        }

        public async Task<ServiceResponse<string>> SetFarmerBotAsync(FarmerBot bot, EventSourceActionId id, CancellationToken cancellationToken)
        {
            var content = JsonContent.Create(bot);
            var response = await _httpClient.PostAsync($"api/filesystem/bot/set", content, cancellationToken);

            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ServiceResponse<string>>(responseContent);

            return result;
        }
        #endregion FarmerBot

        public void Dispose()
        {
            _appSettings.OnAppSettingsChanged -= UpdateAppSettings;
            _timer.Dispose();
        }
    }
}