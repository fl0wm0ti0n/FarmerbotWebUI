
using FarmerbotWebUI.Shared.BotConfig;

namespace FarmerbotWebUI.Server.Services.Filesystem
{
    public interface IFileService
    {
        Task<ServiceResponse<string>> GetLocalLogAsync(string botName, CancellationToken cancellationToken);
        Task<ServiceResponse<string>> GetRawMarkdownConfigAsync(string botName, CancellationToken cancellationToken);
        Task<ServiceResponse<FarmerBotConfig>> GetMarkdownConfigAsync(string botName, CancellationToken cancellationToken);
        Task<ServiceResponse<string>> SetRawMarkdownConfigAsync(string config, string botName, CancellationToken cancellationToken);
        Task<ServiceResponse<string>> SetMarkdownConfigAsync(FarmerBotConfig config, string botName, CancellationToken cancellationToken);
        Task<ServiceResponse<string>> GetRawComposeFileAsync(string botName, CancellationToken cancellationToken);
        Task<ServiceResponse<DockerCompose>> GetComposeFileAsync(string botName, CancellationToken cancellationToken);
        Task<ServiceResponse<string>> SetRawComposeFileAsync(string compose, string botName, CancellationToken cancellationToken);
        Task<ServiceResponse<string>> SetComposeFileAsync(DockerCompose compose, string botName, CancellationToken cancellationToken);
        Task<ServiceResponse<string>> GetRawEnvFileAsync(string botName, CancellationToken cancellationToken);
        Task<ServiceResponse<EnvFile>> GetEnvFileAsync(string botName, CancellationToken cancellationToken);
        Task<ServiceResponse<string>> SetRawEnvFileAsync(string env, string botName, CancellationToken cancellationToken);
        Task<ServiceResponse<string>> SetEnvFileAsync(EnvFile env, string botName, CancellationToken cancellationToken);
        Task<ServiceResponse<FarmerBot>> GetFarmerBotAsync(string botName, CancellationToken cancellationToken);
        Task<ServiceResponse<string>> SetFarmerBotAsync(FarmerBot bot, CancellationToken cancellationToken);
    }
}
