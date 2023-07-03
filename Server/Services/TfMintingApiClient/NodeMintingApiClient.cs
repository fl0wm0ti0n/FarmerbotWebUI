using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FarmerbotWebUI.Client.Services.EventConsole;
using FarmerbotWebUI.Shared.NodeStatus;
using FarmerBotWebUI.Shared;
using FarmerBotWebUI.Shared.NodeStatus;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Swashbuckle.AspNetCore.SwaggerGen;
using YamlDotNet.Core.Tokens;


namespace FarmerbotWebUI.Server.Services.TfApiClient
{
    public class NodeMintingApiClient : INodeMintingApiClient
    {
        private readonly HttpClient _client;
        private IAppSettings _appSettings;
        private SemaphoreSlim _statusSemaphore = new SemaphoreSlim(1);
        private bool _lockInterval = false;
        public Dictionary<string, List<MintingReport>> RawApiData { get; private set; } = new Dictionary<string, List<MintingReport>>();

        public NodeMintingApiClient(IAppSettings appSettings)
        {
            _appSettings = appSettings;
            _appSettings.OnAppSettingsChanged += UpdateAppSettings;
            _client = new HttpClient();
        }

        private void UpdateAppSettings(object sender, AppSettings newAppSettings)
        {
            _appSettings = newAppSettings;
        }

        public async Task<ServiceResponse<Dictionary<string, List<MintingReport>>>> StartStatusInterval()
        {
            TaskCompletionSource<ServiceResponse<Dictionary<string, List<MintingReport>>>> taskCompletionSource = new TaskCompletionSource<ServiceResponse<Dictionary<string, List<MintingReport>>>>();
            SemaphoreSlim firstExecutionSemaphore = new SemaphoreSlim(0, 1);

            Timer timer = new Timer(async (e) =>
            {
              if (!_lockInterval)
                {
                    await _statusSemaphore.WaitAsync();
                    try
                    {
                        EventSourceActionId eventSourceActionId = new EventSourceActionId { Action = EventAction.GetGridNodeStatus, Source = EventSource.TfGraphQlApiClient, Typ = EventTyp.ServerJob };
                        var nodeResponse = await GetMintingOfaNodeListAsync(CancellationToken.None);
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

        public async Task<ServiceResponse<Dictionary<string, List<MintingReport>>>> GetMintingOfaNodeListAsync(CancellationToken cancellationToken)
        {
            string errorMessage = "";
            bool error = false;
            foreach (var bot in _appSettings.FarmerBotSettings.Bots)
            {
                var nodes = await GetMintingOfaNodeAsync(cancellationToken);
                if (!nodes.Success)
                {
                    errorMessage += $"Nodes From FarmerBot {bot.BotName} error: \n";
                    errorMessage += $"{nodes.Message}\n";
                    error = true;
                }
                RawApiData.Add(bot.BotName, nodes.Data);
            }

            return new ServiceResponse<Dictionary<string, List<MintingReport>>>
            {
                Data = RawApiData,
                Message = errorMessage,
                Success = !error
            };
        }

        public async Task<ServiceResponse<List<MintingReport>>> GetMintingOfaNodeAsync(CancellationToken cancellationToken)
        {
            int farmId = 158;
            // TODO: Get Nodes from FarmerBot over graphapi and iterate thru them
            try
            {
                var url = _appSettings.ThreefoldApiSettings.FirstOrDefault(g => g.Net == _appSettings.FarmerBotSettings.Bots.FirstOrDefault(b => b.FarmId == farmId).Network.ToString()).NodeMintingApi;
                var response = await _client.GetAsync(url, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var nodes = JsonConvert.DeserializeObject<List<MintingReport>>(result);
                    return new ServiceResponse<List<MintingReport>>
                    {
                        Data = nodes,
                        Message = "Successfully retrieved nodes",
                        Success = true
                    };
                }
                else
                {
                    return new ServiceResponse<List<MintingReport>>
                    {
                        Data = new List<MintingReport>(),
                        Message = $"Request failed with status code {(int)response.StatusCode}: {response.ReasonPhrase}",
                        Success = false
                    };
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<MintingReport>>
                {
                    Data = new List<MintingReport>(),
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