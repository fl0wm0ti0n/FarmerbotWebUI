using FarmerBotWebUI.Shared;
using Newtonsoft.Json.Linq;

namespace FarmerbotWebUI.Server.Services.Settings
{
    public interface ISettingsService
    {
        IConfiguration Configuration { get; set; }
        //AppSettings AppSetting { get; }
        Task<ServiceResponse<IConfiguration>> ReloadConfiguration();
        Task<ServiceResponse<string>> UpdateConfiguration(string key, string value);
        ServiceResponse<AppSettings> GetConfigurationObject();
        ServiceResponse<AppSettings> SetConfigurationObject(AppSettings appSettings);
    }
}
