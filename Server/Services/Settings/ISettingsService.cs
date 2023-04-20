using FarmerBotWebUI.Shared;
using Newtonsoft.Json.Linq;

namespace FarmerbotWebUI.Server.Services.Settings
{
    public interface ISettingsService
    {
        ServiceResponse<IConfiguration> Configuration { get; set; }
        ServiceResponse<AppSettings> AppSetting { get; }
        ServiceResponse<IConfiguration> ReloadConfiguration();
        ServiceResponse<bool> UpdateConfiguration(string key, string value);
        ServiceResponse<AppSettings> GetConfigurationObject();
        ServiceResponse<bool> SetConfigurationObject(AppSettings appSettings);
    }
}
