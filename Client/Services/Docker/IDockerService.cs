using FarmerbotWebUI.Shared;

namespace FarmerbotWebUI.Client.Services.Docker
{
    public interface IDockerService
    {
        FarmerBotStatus ActualFarmerBotStatus { get; }
        Task<bool> CancelOperation(ActionType command);
        Task<ServiceResponse<string>> StartComposeAsync();
        Task<ServiceResponse<string>> StopComposeAsync();
        Task<ServiceResponse<string>> GetComposeProcessesAsync();
        Task<ServiceResponse<string>> GetComposeListAsync();
        Task<ServiceResponse<string>> GetComposeLogsAsync();
        Task<ServiceResponse<FarmerBotStatus>> GetComposeStatusAsync();
    }
}
