using FarmerbotWebUI.Shared;

namespace FarmerbotWebUI.Client.Services.Docker
{
    public interface IDockerService
    {
        public event Action StatusChanged;
        FarmerBotStatus ActualFarmerBotStatus { get; }
        Task<ServiceResponse<FarmerBotStatus>> StartStatusInterval();
        Task<bool> CancelOperation(EventAction command);
        Task<ServiceResponse<FarmerBotStatus>> StartComposeAsync(string botName, EventSourceActionId id);
        Task<ServiceResponse<FarmerBotStatus>> StopComposeAsync(string botName, EventSourceActionId id);
        Task<ServiceResponse<string>> GetComposeProcessesAsync(string botName, EventSourceActionId id);
        Task<ServiceResponse<string>> GetComposeListAsync(string botName, EventSourceActionId id);
        Task<ServiceResponse<string>> GetComposeLogsAsync(string botName, EventSourceActionId id);
        Task<ServiceResponse<FarmerBotStatus>> GetComposeStatusAsync(string botName, EventSourceActionId id);
    }
}
