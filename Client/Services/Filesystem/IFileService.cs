using FarmerbotWebUI.Shared;

namespace FarmerbotWebUI.Client.Services.Filesystem
{
    public interface IFileService
    {
        Task<ServiceResponse<string>> GetLocalLogAsync(string path);
        Task<ServiceResponse<FarmerBotConfig>> GetMarkdownConfigAsync(string path);
        Task<ServiceResponse<string>> GetRawMarkdownConfigAsync(string path);
        Task<ServiceResponse<string>> SetMarkdownConfigAsync(FarmerBotConfig config, string path);
        Task<ServiceResponse<string>> SetMarkdownConfigAsync(string config, string path);
        Task<ServiceResponse<string>> GetRawComposeFileAsync(string path);
        Task<ServiceResponse<FarmerBotServices>> GetComposeFileAsync(string path);
        Task<ServiceResponse<string>> SetComposeFileAsync(string compose, string path);
        Task<ServiceResponse<string>> SetComposeFileAsync(FarmerBotServices compose, string path);
        Task<ServiceResponse<string>> GetEnvFileAsync(string path);
        Task<ServiceResponse<string>> SetEnvFileAsync(string env, string path);
    }
}
