using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FarmerbotWebUI.Shared.NodeStatus;
using FarmerBotWebUI.Shared;
using Newtonsoft.Json.Linq;


namespace FarmerbotWebUI.Server.Services.TfApiClient
{
    public interface INodeMintingApiClient
    {
        Dictionary<string, List<NodeMintingCollection>> RawApiData { get; }
        Task<ServiceResponse<Dictionary<string, List<MintingReport>>>> StartStatusInterval();
        Task<ServiceResponse<Dictionary<string, List<MintingReport>>>> GetMintingOfaNodeListAsync(CancellationToken cancellationToken);
        Task<ServiceResponse<List<MintingReport>>> GetMintingOfaNodeAsync(CancellationToken cancellationToken);
    }
}