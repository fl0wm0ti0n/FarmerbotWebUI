using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FarmerBotWebUI.Shared;
using Newtonsoft.Json.Linq;


namespace FarmerbotWebUI.Server.Services.TfApiClient
{
    public interface ITfGraphQLApiClient
    {
        List<Nodes> Nodes { get; }
        Task<ServiceResponse<List<Nodes>>> StartStatusInterval();
        Task<ServiceResponse<Nodes>> GetNodesByFarmIdAsync(int farmId, CancellationToken cancellationToken);
        Task<ServiceResponse<List<Nodes>>> GetNodesListAsync(CancellationToken cancellationToken);
        Task<ServiceResponse<Farms>> GetFarmDetailsAsync(int farmId, CancellationToken cancellationToken);
    }
}