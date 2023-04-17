using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FarmerBotWebUI.Shared;
using Newtonsoft.Json.Linq;


namespace FarmerbotWebUI.Server.Services.TfApiClient
{
    public interface ITfGraphQLApiClient
    {
        Task<List<Node>> GetNodesByFarmIdAsync(int farmId);
        Task<List<Farm>> GetFarmDetailsAsync(int farmId);
    }
}