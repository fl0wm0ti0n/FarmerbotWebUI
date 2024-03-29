﻿using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FarmerbotWebUI.Client.Services.EventConsole;
using FarmerbotWebUI.Server.Services.NodeStatus;
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
        private readonly INodeStatusService _nodeStatusService;
        private SemaphoreSlim _statusSemaphore = new SemaphoreSlim(1);
        private bool _lockInterval = false;
        public Dictionary<string, List<NodeMintingCollection>> RawApiData { get; private set; } = new Dictionary<string, List<NodeMintingCollection>>();

        public NodeMintingApiClient(IAppSettings appSettings, INodeStatusService nodeStatusService)
        {
            _appSettings = appSettings;
            _nodeStatusService = nodeStatusService;
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

        public async Task<ServiceResponse<Dictionary<string, List<NodeMintingCollection>>>> GetMintingCollectionListAsync(CancellationToken cancellationToken)
        {
            string errorMessage = "";
            bool error = false;
            foreach (var bot in _appSettings.FarmerBotSettings.Bots)
            {
                var nodes = await GetMintingCollectionAsync(bot.BotName, cancellationToken);
                if (!nodes.Success)
                {
                    errorMessage += $"Nodes From FarmerBot {bot.BotName} error: \n";
                    errorMessage += $"{nodes.Message}\n";
                    error = true;
                }
                RawApiData.Add(bot.BotName, new List<NodeMintingCollection> { nodes.Data });
            }

            return new ServiceResponse<Dictionary<string, List<NodeMintingCollection>>>
            {
                Data = RawApiData,
                Message = errorMessage,
                Success = !error
            };
        }

        public async Task<ServiceResponse<NodeMintingCollection>> GetMintingCollectionAsync(string botName, CancellationToken cancellationToken)
        {
            string errorMessage = "";
            bool error = false;
            NodeMintingCollection nodeMintingReportsDict = new NodeMintingCollection();

            if (await _nodeStatusService.CheckGridNodesAreThere(cancellationToken))
            {
                var rawNodeStatusList = await _nodeStatusService.GetNodeStatusListAsync(cancellationToken);
                if (rawNodeStatusList.Success)
                {
                    foreach (var node in rawNodeStatusList.Data)
                    {
                        var nodeId = node.NodeStatusSets.FirstOrDefault(i => i.BotName == botName).GridNode.NodeId;
                        var farmId = node.NodeStatusSets.FirstOrDefault(i => i.BotName == botName).GridNode.FarmId;
                        var rawNodeMintingReportList = await GetMintingOfaNodeAsync(nodeId, farmId, cancellationToken);

                        if (!rawNodeMintingReportList.Success)
                        {
                            errorMessage += $"Getting MintingReports From Node {nodeId} error: \n";
                            errorMessage += $"{rawNodeMintingReportList.Message}\n";
                            error = true;
                        }
                        nodeMintingReportsDict.MintingReports.Add(nodeId, rawNodeMintingReportList.Data);
                    }
                }
            }

            return new ServiceResponse<NodeMintingCollection>
            {
                Data = nodeMintingReportsDict,
                Message = errorMessage,
                Success = !error
            };
        }

        public async Task<ServiceResponse<List<MintingReport>>> GetMintingOfaNodeAsync(int nodeId, int farmId, CancellationToken cancellationToken)
        {
            // TODO: Get Nodes from FarmerBot over graphapi and iterate thru them

            try
            {
                string urlString = _appSettings.ThreefoldApiSettings.FirstOrDefault(g => g.Net == _appSettings.FarmerBotSettings.Bots.FirstOrDefault(b => b.FarmId == farmId).Network.ToString()).NodeMintingApi.ToString();
                Uri url = new Uri(urlString.Replace("{nodeId}", nodeId.ToString()));
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

        public Task<ServiceResponse<Dictionary<string, List<MintingReport>>>> GetMintingOfaNodeListAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<List<MintingReport>>> GetMintingOfaNodeAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}