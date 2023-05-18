using FarmerBotWebUI.Shared;

namespace FarmerbotWebUI.Client.Services.Settings
{
    public interface ISettingsService
    {
        void StartStatusInterval();
        Task<ServiceResponse<AppSettings>> GetConfigurationObject(EventSourceActionId id);
        Task<ServiceResponse<AppSettings>> SetConfigurationObject(AppSettings appSettings, EventSourceActionId id);
    }
}
