using FarmerbotWebUI.Client.Shared;
using FarmerbotWebUI.Shared;
using FarmerBotWebUI.Shared;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using static System.Net.WebRequestMethods;

namespace FarmerbotWebUI.Client.Services.Filesystem
{
    public class FilesService : IFileService
    {

        private readonly HttpClient _httpClient;
        private readonly IEventConsoleService _eventConsole;
        private IAppSettings _appSettings;

        public FilesService(HttpClient httpClient, IEventConsoleService eventConsole, IAppSettings appSettings)
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
            var response = await _httpClient.PostAsync($"api/markdown/set/{botName}", content);

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
            var response = await _httpClient.PostAsync($"api/compose/set/{botName}", content);

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
        public async Task<ServiceResponse<FarmerBot>> GetFarmerBotAsync(string botName, EventSourceActionId id, CancellationToken cancellationToken)
        {
            var title = "Getting FarmerBot Files";
            var message = $"Getting FarmerBot Files...";
            var GuiAndProgress = id.Typ == EventTyp.UserAction ? true : false;
            id = _eventConsole.AddMessage(id, title, message, GuiAndProgress, false, GuiAndProgress, LogLevel.Information, EventResult.Valueless);

            var response = await _httpClient.GetFromJsonAsync<ServiceResponse<AppSettings>>("api/settings/getsettings");
            if (response.Success)
            {
                _appSettings = response.Data;
                AppSetting = response.Data;
                _appSettings.InvokeOnAppSettingsChanged(AppSetting);
                _eventConsole.UpdateMessage(id, title, response.Message, false, true, GuiAndProgress, LogLevel.Information, EventResult.Successfully);
            }
            else if (!response.Success)
            {
                _appSettings = response.Data;
                AppSetting = response.Data;
                OnAppSettingsChanged?.Invoke(this, response.Data);
                message = $"Error getting FarmerBot Files...\n{response.Message}";
                _eventConsole.UpdateMessage(id, title, message, false, true, true, LogLevel.Error, EventResult.Unsuccessfully);
            }
            _lockInterval = false;
            return response;
        }

        public async Task<ServiceResponse<string>> SetFarmerBotAsync(FarmerBot bot, EventSourceActionId id, CancellationToken cancellationToken)
        {
            var content = JsonContent.Create(bot);
            var response = await _httpClient.PostAsync($"api/compose/set", content, cancellationToken);

            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ServiceResponse<string>>(responseContent);

            return result;
        }
        #endregion FarmerBot
    }
}