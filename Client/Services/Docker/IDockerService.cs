using FarmerbotWebUI.Shared;

namespace FarmerbotWebUI.Client.Services.Docker
{
    public interface IDockerService
    {
        Task<ServiceResponse<string>> StartComposeAsync();
        Task<ServiceResponse<string>> StopComposeAsync();
        Task<ServiceResponse<string>> GetComposeProcessesAsync();
        Task<ServiceResponse<string>> GetComposeListAsync();
        Task<ServiceResponse<string>> GetComposeLogsAsync();
        Task<ServiceResponse<string>> GetLocalLogAsync(string path);
        Task<ServiceResponse<FarmerBotConfig>> GetMarkdownConfigAsync(string path);
        Task<ServiceResponse<string>> GetRawMarkdownConfigAsync(string path);
        Task<ServiceResponse<string>> SetMarkdownConfigAsync(FarmerBotConfig config, string path);
        Task<ServiceResponse<string>> SetMarkdownConfigAsync(string config, string path);
        Task<ServiceResponse<string>> GetComposeFileAsync(string path);
        Task<ServiceResponse<string>> SetComposeFileAsync(string compose, string path);
    }
}
