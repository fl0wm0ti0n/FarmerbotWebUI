using FarmerbotWebUI.Shared;

namespace FarmerbotWebUI.Client.Services.Docker
{
    public interface IDockerService
    {
        public event Action StatusChanged;
        FarmerBotStatus ActualFarmerBotStatus { get; }
        Task<bool> CancelOperation(EventAction command);
        Task<ServiceResponse<FarmerBotStatus>> StartComposeAsync(EventSourceActionId id);
        Task<ServiceResponse<FarmerBotStatus>> StopComposeAsync(EventSourceActionId id);
        Task<ServiceResponse<string>> GetComposeProcessesAsync(EventSourceActionId id);
        Task<ServiceResponse<string>> GetComposeListAsync(EventSourceActionId id);
        Task<ServiceResponse<string>> GetComposeLogsAsync(EventSourceActionId id);
        Task<ServiceResponse<FarmerBotStatus>> GetComposeStatusAsync(EventSourceActionId id);
    }
}
