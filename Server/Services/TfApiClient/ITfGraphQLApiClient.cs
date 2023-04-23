using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FarmerBotWebUI.Shared;
using Newtonsoft.Json.Linq;


namespace FarmerbotWebUI.Server.Services.TfApiClient
{
    public interface ITfGraphQLApiClient
    {
        Task<ServiceResponse<Nodes>> StartStatusInterval();
        Task<ServiceResponse<Nodes>> GetNodesByFarmIdAsync(int farmId);
        Task<ServiceResponse<Farms>> GetFarmDetailsAsync(int farmId);
    }
}