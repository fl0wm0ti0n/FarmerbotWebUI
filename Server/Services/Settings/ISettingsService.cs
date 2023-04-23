using FarmerBotWebUI.Shared;
using Newtonsoft.Json.Linq;

namespace FarmerbotWebUI.Server.Services.Settings
{
    public interface ISettingsService
    {
        IConfiguration Configuration { get; set; }
        AppSettings AppSetting { get; }
        ServiceResponse<IConfiguration> ReloadConfiguration();
        ServiceResponse<string> UpdateConfiguration(string key, string value);
        ServiceResponse<AppSettings> GetConfigurationObject();
        ServiceResponse<AppSettings> SetConfigurationObject(AppSettings appSettings);
    }
}
