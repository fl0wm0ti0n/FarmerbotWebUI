using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FarmerBotWebUI.Shared;
using Newtonsoft.Json.Linq;


namespace FarmerbotWebUI.Server.Services.TfApiClient
{
    public interface ITfGraphQLApiClient
    {
        Task<ServiceResponse<List<Nodes>>> StartStatusInterval();
        Task<ServiceResponse<Nodes>> GetNodesByFarmIdAsync(int farmId);
        Task<ServiceResponse<List<Nodes>>> GetNodesListAsync();
        Task<ServiceResponse<Farms>> GetFarmDetailsAsync(int farmId);
    }
}