
namespace FarmerbotWebUI.Server.Services.Filesystem
{
    public interface IFileService
    {
        Task<ServiceResponse<string>> GetLocalLogAsync(string path, CancellationToken cancellationToken);
        Task<ServiceResponse<FarmerBotConfig>> GetMarkdownConfigAsync(string path, CancellationToken cancellationToken);
        Task<ServiceResponse<string>> GetRawMarkdownConfigAsync(string path, CancellationToken cancellationToken);
        Task<ServiceResponse<string>> SetMarkdownConfigAsync(FarmerBotConfig config, string path, CancellationToken cancellationToken);
        Task<ServiceResponse<string>> SetMarkdownConfigAsync(string config, string path, CancellationToken cancellationToken);
        Task<ServiceResponse<string>> GetRawComposeFileAsync(string path, CancellationToken cancellationToken);
        Task<ServiceResponse<FarmerBotServices>> GetComposeFileAsync(string path, CancellationToken cancellationToken);
        Task<ServiceResponse<string>> SetComposeFileAsync(string compose, string path, CancellationToken cancellationToken);
        Task<ServiceResponse<string>> SetComposeFileAsync(FarmerBotServices compose, string path, CancellationToken cancellationToken);
        Task<ServiceResponse<string>> GetEnvFileAsync(string path, CancellationToken cancellationToken);
        Task<ServiceResponse<string>> SetEnvFileAsync(string env, string path, CancellationToken cancellationToken);
    }
}
