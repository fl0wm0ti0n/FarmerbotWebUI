using FarmerbotWebUI.Shared;

namespace FarmerbotWebUI.Server.Services.Docker
{
    public interface IDockerService
    {
        List<FarmerBotStatus> ActualFarmerBotStatus { get; }
        //FarmerBotServices FarmerBotServices { get; }
        Task<ServiceResponse<FarmerBotStatus>> StartComposeAsync(string botName, CancellationToken cancellationToken);
        Task<ServiceResponse<FarmerBotStatus>> StopComposeAsync(string botName, CancellationToken cancellationToken);
        Task<ServiceResponse<string>> GetComposeProcessesAsync(string botName, CancellationToken cancellationToken);
        Task<ServiceResponse<string>> GetComposeListAsync(string botName, CancellationToken cancellationToken);
        Task<ServiceResponse<string>> GetComposeLogsAsync(string botName, CancellationToken cancellationToken);
        Task<ServiceResponse<FarmerBotStatus>> GetComposeStatusAsync(string botName, CancellationToken cancellationToken);
        Task<ServiceResponse<List<FarmerBotStatus>>> GetComposeStatusListAsync(CancellationToken cancellationToken);
    }
}
