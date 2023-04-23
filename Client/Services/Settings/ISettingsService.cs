using FarmerBotWebUI.Shared;

namespace FarmerbotWebUI.Client.Services.Settings
{
    public interface ISettingsService
    {
        AppSettings AppSetting { get; }
        Task<ServiceResponse<AppSettings>> StartStatusInterval();
        Task<ServiceResponse<AppSettings>> GetConfigurationObject(EventSourceActionId id);
        Task<ServiceResponse<AppSettings>> SetConfigurationObject(AppSettings appSettings, EventSourceActionId id);
    }
}
