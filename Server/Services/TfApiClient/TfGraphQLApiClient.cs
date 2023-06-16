using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FarmerbotWebUI.Client.Services.EventConsole;
using FarmerBotWebUI.Shared;
using FarmerBotWebUI.Shared.NodeStatus;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using YamlDotNet.Core.Tokens;


namespace FarmerbotWebUI.Server.Services.TfApiClient
{
    public class TfGraphQLApiClient : ITfGraphQLApiClient
    {
        private readonly HttpClient _client;
        private IAppSettings _appSettings;
        private SemaphoreSlim _statusSemaphore = new SemaphoreSlim(1);
        private bool _lockInterval = false;
        public List<Nodes> RawApiData { get; private set; } = new List<Nodes>();

        public TfGraphQLApiClient(IAppSettings appSettings)
        {
            _appSettings = appSettings;
            _appSettings.OnAppSettingsChanged += UpdateAppSettings;
            _client = new HttpClient();
        }

        private void UpdateAppSettings(object sender, AppSettings newAppSettings)
        {
            _appSettings = newAppSettings;
        }

        public async Task<ServiceResponse<List<Nodes>>> StartStatusInterval()
        {
            TaskCompletionSource<ServiceResponse<List<Nodes>>> taskCompletionSource = new TaskCompletionSource<ServiceResponse<List<Nodes>>>();
            SemaphoreSlim firstExecutionSemaphore = new SemaphoreSlim(0, 1);

            Timer timer = new Timer(async (e) =>
            {
              if (!_lockInterval)
                {
                    await _statusSemaphore.WaitAsync();
                    try
                    {
                        EventSourceActionId eventSourceActionId = new EventSourceActionId { Action = EventAction.GetGridNodeStatus, Source = EventSource.TfGraphQlApiClient, Typ = EventTyp.ServerJob };
                        var nodeResponse = await GetNodesListAsync(CancellationToken.None);
                        //var farmResponse = await GetFarmDetailsAsync(farmId);

                        // Setzen Sie das Ergebnis des TaskCompletionSource, wenn die Semaphore noch nicht freigegeben wurde
                        if (firstExecutionSemaphore.CurrentCount == 0)
                        {
                            taskCompletionSource.SetResult(nodeResponse);
                            firstExecutionSemaphore.Release();
                        }
                    }
                    finally
                    {
                        _statusSemaphore.Release();
                    }
                }
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(_appSettings.GeneralSettings.ApiCallInterval));

            // Warten Sie auf das Ergebnis des TaskCompletionSource, bevor Sie die Antwort zurückgeben
            return await taskCompletionSource.Task;
        }

        public async Task<ServiceResponse<List<Nodes>>> GetNodesListAsync(CancellationToken cancellationToken)
        {
            string errorMessage = "";
            bool error = false;
            foreach (var bot in _appSettings.FarmerBotSettings.Bots)
            {
                var nodes = await GetNodesByFarmIdAsync(bot.FarmId, cancellationToken);
                if (!nodes.Success)
                {
                    errorMessage += $"Nodes From FarmerBot {bot.BotName} error: \n";
                    errorMessage += $"{nodes.Message}\n";
                    error = true;
                }
                nodes.Data.FarmId = bot.FarmId;
                RawApiData.Add(nodes.Data);
            }

            return new ServiceResponse<List<Nodes>>
            {
                Data = RawApiData,
                Message = errorMessage,
                Success = !error
            };
        }

        public async Task<ServiceResponse<Nodes>> GetNodesByFarmIdAsync(int farmId, CancellationToken cancellationToken)
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

            try
            {
                var content = new StringContent(JObject.FromObject(requestBody).ToString(), Encoding.UTF8, "application/json");
                var url = _appSettings.ThreefoldApiSettings.FirstOrDefault(g => g.Net == _appSettings.FarmerBotSettings.Bots.FirstOrDefault(b => b.FarmId == farmId).Network.ToString()).GraphQl;
                var response = await _client.PostAsync(url, content, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var nodes = JsonConvert.DeserializeObject<Nodes>(result);
                    return new ServiceResponse<Nodes>
                    {
                        Data = nodes,
                        Message = "Successfully retrieved nodes",
                        Success = true
                    };
                }
                else
                {
                    return new ServiceResponse<Nodes>
                    {
                        Data = new Nodes(),
                        Message = $"Request failed with status code {(int)response.StatusCode}: {response.ReasonPhrase}",
                        Success = false
                    };
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<Nodes>
                {
                    Data = new Nodes(),
                    Message = ex.Message,
                    Success = false
                };
            }

        }

        public async Task<ServiceResponse<Farms>> GetFarmDetailsAsync(int farmId, CancellationToken cancellationToken)
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

            try
            {
                var content = new StringContent(JObject.FromObject(requestBody).ToString(), Encoding.UTF8, "application/json");
                var url = _appSettings.ThreefoldApiSettings.FirstOrDefault(g => g.Net == _appSettings.FarmerBotSettings.Bots.FirstOrDefault(b => b.FarmId == farmId).Network.ToString()).GraphQl;
                var response = await _client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var farms = JsonConvert.DeserializeObject<Farms>(result);
                    return new ServiceResponse<Farms>
                    {
                        Data = farms,
                        Message = "Successfully retrieved farm details",
                        Success = true
                    };
                }
                else
                {
                    return new ServiceResponse<Farms>
                    {
                        Data = new Farms(),
                        Message = $"Request failed with status code {(int)response.StatusCode}: {response.ReasonPhrase}",
                        Success = false
                    };
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<Farms>
                {
                    Data = new Farms(),
                    Message = ex.Message,
                    Success = false
                };
            }
        }
        public void Dispose()
        {
            _appSettings.OnAppSettingsChanged -= UpdateAppSettings;
        }
    }
}