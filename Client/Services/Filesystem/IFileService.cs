using FarmerbotWebUI.Shared;

namespace FarmerbotWebUI.Client.Services.Filesystem
{
    public interface IFileService
    {
        Task<ServiceResponse<string>> GetLocalLogAsync(string botName, EventSourceActionId id, CancellationToken cancellationToken);
        Task<ServiceResponse<string>> GetRawMarkdownConfigAsync(string botName, EventSourceActionId id, CancellationToken cancellationToken);
        Task<ServiceResponse<FarmerBotConfig>> GetMarkdownConfigAsync(string botName, EventSourceActionId id, CancellationToken cancellationToken);
        Task<ServiceResponse<string>> SetRawMarkdownConfigAsync(string config, string botName, EventSourceActionId id, CancellationToken cancellationToken);
        Task<ServiceResponse<string>> SetMarkdownConfigAsync(FarmerBotConfig config, string botName, EventSourceActionId id, CancellationToken cancellationToken);
        Task<ServiceResponse<string>> GetRawComposeFileAsync(string botName, EventSourceActionId id, CancellationToken cancellationToken);
        Task<ServiceResponse<DockerCompose>> GetComposeFileAsync(string botName, EventSourceActionId id, CancellationToken cancellationToken);
        Task<ServiceResponse<string>> SetRawComposeFileAsync(string compose, string botName, EventSourceActionId id, CancellationToken cancellationToken);
        Task<ServiceResponse<string>> SetComposeFileAsync(DockerCompose compose, string botName, EventSourceActionId id, CancellationToken cancellationToken);
        Task<ServiceResponse<string>> GetRawEnvFileAsync(string botName, EventSourceActionId id, CancellationToken cancellationToken);
        Task<ServiceResponse<EnvFile>> GetEnvFileAsync(string botName, EventSourceActionId id, CancellationToken cancellationToken);
        Task<ServiceResponse<string>> SetRawEnvFileAsync(string env, string botName, EventSourceActionId id, CancellationToken cancellationToken);
        Task<ServiceResponse<string>> SetEnvFileAsync(EnvFile env, string botName, EventSourceActionId id, CancellationToken cancellationToken);
        Task<ServiceResponse<FarmerBot>> GetFarmerBotAsync(string botName, EventSourceActionId id, CancellationToken cancellationToken);
        Task<ServiceResponse<string>> SetFarmerBotAsync(FarmerBot bot, EventSourceActionId id, CancellationToken cancellationToken);
    }
}
