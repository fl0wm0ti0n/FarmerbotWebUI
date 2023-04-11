using FarmerbotWebUI.Shared;

namespace FarmerbotWebUI.Server.Services.Docker
{
    public interface IDockerService
    {
        Task<ServiceResponse<string>> StartComposeAsync(CancellationToken cancellationToken);
        Task<ServiceResponse<string>> StopComposeAsync(CancellationToken cancellationToken);
        Task<ServiceResponse<string>> GetComposeProcessesAsync(CancellationToken cancellationToken);
        Task<ServiceResponse<string>> GetComposeListAsync(CancellationToken cancellationToken);
        Task<ServiceResponse<string>> GetComposeLogsAsync(CancellationToken cancellationToken);
        Task<ServiceResponse<FarmerBotStatus>> GetComposeStatusAsync(CancellationToken cancellationToken);
    }
}
