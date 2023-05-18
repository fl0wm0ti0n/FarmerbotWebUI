using FarmerbotWebUI.Shared.BotConfig;

namespace FarmerbotWebUI.Client.Services.Docker
{
    public interface IDockerService
    {
        event Action StatusChanged;
        List<FarmerBotStatus> ActualFarmerBotStatusList { get; }
        void StartStatusInterval();
        Task<bool> CancelOperation(EventAction command);
        Task<ServiceResponse<FarmerBotStatus>> StartComposeAsync(string botName, EventSourceActionId id);
        Task<ServiceResponse<FarmerBotStatus>> StopComposeAsync(string botName, EventSourceActionId id);
        Task<ServiceResponse<string>> GetComposeProcessesAsync(string botName, EventSourceActionId id);
        Task<ServiceResponse<string>> GetComposeListAsync(string botName, EventSourceActionId id);
        Task<ServiceResponse<string>> GetComposeLogsAsync(string botName, EventSourceActionId id);
        Task<ServiceResponse<FarmerBotStatus>> GetComposeStatusAsync(string botName, EventSourceActionId id);
        Task<ServiceResponse<List<FarmerBotStatus>>> GetComposeStatusListAsync(EventSourceActionId id);
    }
}
