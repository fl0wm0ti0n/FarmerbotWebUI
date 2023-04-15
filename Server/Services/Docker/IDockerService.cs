using FarmerbotWebUI.Shared;

namespace FarmerbotWebUI.Server.Services.Docker
{
    public interface IDockerService
    {
        FarmerBotStatus ActualFarmerBotStatus { get; }
        FarmerBotServices FarmerBotServices { get; }
        Task<ServiceResponse<FarmerBotStatus>> StartComposeAsync(CancellationToken cancellationToken);
        Task<ServiceResponse<FarmerBotStatus>> StopComposeAsync(CancellationToken cancellationToken);
        Task<ServiceResponse<string>> GetComposeProcessesAsync(CancellationToken cancellationToken);
        Task<ServiceResponse<string>> GetComposeListAsync(CancellationToken cancellationToken);
        Task<ServiceResponse<string>> GetComposeLogsAsync(CancellationToken cancellationToken);
        Task<ServiceResponse<FarmerBotStatus>> GetComposeStatusAsync(CancellationToken cancellationToken);
    }
}
