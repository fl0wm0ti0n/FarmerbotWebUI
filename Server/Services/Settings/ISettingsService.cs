using FarmerBotWebUI.Shared;
using Newtonsoft.Json.Linq;

namespace FarmerbotWebUI.Server.Services.Settings
{
    public interface ISettingsService
    {
        IConfiguration Configuration { get; set; }
        AppSettings AppSetting { get; }
        IConfiguration ReloadConfiguration();
        void UpdateConfiguration(string key, string value);
        AppSettings GetConfigurationObject();
        void SetConfigurationObject(AppSettings appSettings);
    }
}
