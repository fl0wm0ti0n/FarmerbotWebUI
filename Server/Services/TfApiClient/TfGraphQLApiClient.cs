using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FarmerbotWebUI.Client.Services.EventConsole;
using FarmerBotWebUI.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace FarmerbotWebUI.Server.Services.TfApiClient
{
    public class TfGraphQLApiClient : ITfGraphQLApiClient
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _config;
        private readonly int _NodeStatusInterval = 60; // TODO: get from config
        private SemaphoreSlim _statusSemaphore = new SemaphoreSlim(1);
        private bool _lockInterval = false;

        public TfGraphQLApiClient(HttpClient client, IConfiguration configuration)
        {
            _client = client;
            _config = configuration;
            StartStatusInterval();
        }

        private void StartStatusInterval()
        {
            Timer timer = new Timer(async (e) =>
            {
                if (!_lockInterval)
                {
                    await _statusSemaphore.WaitAsync();
                    try
                    {
                        var farmId = _config.GetValue<int>("ThreefoldFarmSedttings:FarmId");
                        EventSourceActionId eventSourceActionId = new EventSourceActionId { Action = EventAction.GetGridNodeStatus, Source = EventSource.TfGraphQlApiClient, Typ = EventTyp.ServerJob};
                        var nodeResponse = await GetNodesByFarmIdAsync(farmId);
                        var farmResponse = await GetFarmDetailsAsync(farmId);
                    }
                    finally
                    {
                        _statusSemaphore.Release();
                    }
                }
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(_NodeStatusInterval));
        }

        public async Task<List<Node>> GetNodesByFarmIdAsync(int farmId)
        {
            var query = @"
                query GetNodesByFarmId($farmId: Int!) {
                    nodes(where: { farmID_eq: $farmId }) {
                        id
                        nodeID
                        uptime
                        virtualized
                        updatedAt
                        twinID
                        secure
                        serialNumber
                        resourcesTotal {
                          sru
                          mru
                          id
                          hru
                          cru
                        }
                        publicConfig {
                          ipv6
                          ipv4
                          id
                          gw6
                          gw4
                          domain
                        }
                        power {
                          target
                          state
                        }
                        location {
                          id
                          latitude
                          longitude
                        }
                        interfaces {
                          name
                          mac
                          ips
                          id
                        }
                        gridVersion
                        farmingPolicyId
                        farmID
                        createdAt
                        created
                        country
                        connectionPrice
                        city
                        certification
                    }
                }
            ";

            var variables = new
            {
                farmId
            };

            var requestBody = new
            {
                query,
                variables
            };

            var content = new StringContent(JObject.FromObject(requestBody).ToString(), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("https://graphql.qa.grid.tf/graphql", content);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var parsedResult = JsonConvert.DeserializeObject<Nodes>(result);
                var nodes = parsedResult.Data.Nodes;
                return nodes;
            }
            else
            {
                throw new HttpRequestException($"Request failed with status code {(int)response.StatusCode}: {response.ReasonPhrase}");
            }
        }

        public async Task<List<Farm>> GetFarmDetailsAsync(int farmId)
        {
            var query = @"
                query GetFarmsByFarmId($farmId: Int!) {
                  farms(where: {farmID_eq: $farmId}) {
                    certification
                    dedicatedFarm
                    farmID
                    gridVersion
                    id
                    name
                    pricingPolicyID
                    stellarAddress
                    twinID
                    publicIPs {
                      ip
                      id
                      gateway
                      contractId
                    }
                }
            ";

            var variables = new
            {
                farmId
            };

            var requestBody = new
            {
                query,
                variables
            };

            var content = new StringContent(JObject.FromObject(requestBody).ToString(), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("https://graphql.qa.grid.tf/graphql", content);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var parsedResult = JsonConvert.DeserializeObject<Farms>(result);
                var farms = parsedResult.Data.Farms;
                return farms;
            }
            else
            {
                throw new HttpRequestException($"Request failed with status code {(int)response.StatusCode}: {response.ReasonPhrase}");
            }
        }
    }
}