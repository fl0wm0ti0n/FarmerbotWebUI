using FarmerbotWebUI.Shared.NodeStatus;
using FarmerBotWebUI.Shared;
using Markdig.Syntax.Inlines;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading;

namespace FarmerbotWebUI.Server.Services.NodeStatus
{
    public class NodeStatusService : INodeStatusService
    {
        private IAppSettings _appSettings;
        private readonly ITfGraphQLApiClient _tfGraphQLApiClient;
        private readonly IFileService _fileService;

        public NodeStatusService(IAppSettings appSettings, ITfGraphQLApiClient tfGraphQLApiClient, IFileService fileService)
        {
            _appSettings = appSettings;
            _tfGraphQLApiClient = tfGraphQLApiClient;
            _fileService = fileService;
            _appSettings.OnAppSettingsChanged += UpdateAppSettings;
        }

        private void UpdateAppSettings(object sender, AppSettings newAppSettings)
        {
            _appSettings = newAppSettings;
        }

        public async Task<ServiceResponse<List<NodeStatusCollection>>> GetNodeStatusListAsync(CancellationToken cancellationToken)
        {
            bool success = true;
            StringBuilder sb = new StringBuilder();

            List<NodeStatusCollection> nodeStatusCollection = new List<NodeStatusCollection>();
            foreach (var bot in _appSettings.FarmerBotSettings.Bots)
            {
                var nodeCollection = await GetNodeStatusCollectionAsync(bot.BotName, cancellationToken);
                if (nodeCollection.Success)
                {
                    nodeStatusCollection.Add(nodeCollection.Data);
                }
                else
                {
                    sb.AppendLine(nodeCollection.Message);
                    success = false;
                }
            }

            return new ServiceResponse<List<NodeStatusCollection>>()
            {
                Data = nodeStatusCollection,
                Message = sb.ToString(),
                Success = success,
            };
        }

        public async Task<ServiceResponse<NodeStatusCollection>> GetNodeStatusCollectionAsync(string botName, CancellationToken cancellationToken)
        {
            bool error = false;
            string errorMessage = "";
            bool noStatus = false;
            List<NodeStatusSet> nodeStatusSets = new List<NodeStatusSet>();

            BotSetting bot = _appSettings.FarmerBotSettings.Bots.FirstOrDefault(x => x.BotName == botName);
            if (bot == null)
            {
                error = true;
                noStatus = true;
                errorMessage = $"Didn't find Bot {botName} in BotConfig.";
            }
            else if (await CheckGridNodesAreThere(cancellationToken))
            {
                // get the right Node-list with farmId which is present in botconfig
                Nodes rawNodeResult = _tfGraphQLApiClient.RawApiData.FirstOrDefault(r => r.FarmId == bot.FarmId);


                if (rawNodeResult == null)
                {
                    error = true;
                    noStatus = true;
                    errorMessage = $"Didn't find Nodes on Grid with given FarmId {bot.FarmId} from Bot {bot.BotName}.";
                    // TODO: Send to LogService
                }
                else
                {
                    var gridNodes = rawNodeResult.Data.Nodes;

                    // get the right nodeDefinitions with botName which is present in botconfig
                    var result = await _fileService.GetMarkdownConfigAsync(bot.BotName, cancellationToken);
                    if (!result.Success)
                    {
                        error = true;
                        errorMessage = $"No MarkdownConfig for Bot {bot.BotName}.";
                    }
                    else
                    {
                        var markdownConfig = result.Data;
                        // TODO: add also Live node status and errors from farmerbot.log or 

                        // build the NodeStatusSet
                        foreach (var gridNode in gridNodes)
                        {
                            NodeDefinition nodeDefinition = markdownConfig.NodeDefinitions.FirstOrDefault(x => x.Id == gridNode.NodeId);
                            // if nodeDefinition is null, then the node is not configured in the markdown config
                            bool notConfigured = false;
                            if (nodeDefinition == null)
                            {
                                notConfigured = true;
                            }

                            nodeStatusSets.Add(new NodeStatusSet()
                            {
                                BotNode = new BotNode()
                                {
                                    NodeId = gridNode.NodeId,
                                    NodeDefinition = nodeDefinition,
                                    Status = Status.unknown, // TODO: check status from farmerbot.log
                                    UpdatedAt = DateTime.UtcNow
                                },
                                GridNode = gridNode,
                                Farm = new Farm()
                                {
                                    FarmId = gridNode.FarmId, // TODO: get Farm also from apiService
                                },
                                BotName = bot.BotName,
                                NotConfigured = notConfigured,
                                LastUpdate = DateTime.UtcNow,
                                NoStatus = false,
                                IsError = false,
                                ErrorMessage = ""
                            });
                        }
                    }
                }
            }
            else
            {
                error = true;
                noStatus = true;
                errorMessage = "GridNodes are not available";
                // TODO: Send to LogService
            }

            return new ServiceResponse<NodeStatusCollection>()
            {
                Data = new NodeStatusCollection()
                {
                    BotName = bot.BotName,
                    Farm = new Farm() 
                    { 
                        FarmId = bot.FarmId
                    },
                    NodeStatusSets = nodeStatusSets,
                    LastUpdate = DateTime.UtcNow,
                    NoStatus = noStatus
                },
                Message = "",
                Success = true,
            };
        }

        public async Task<ServiceResponse<NodeStatusSet>> GetNodeStatusAsync(int nodeId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private async Task<bool> CheckGridNodesAreThere (CancellationToken cancellationToken)
        {
            if (_tfGraphQLApiClient.RawApiData.Count == 0)
            {
                var apiResult = await _tfGraphQLApiClient.GetNodesListAsync(cancellationToken);
                if (apiResult.Success)
                {
                    return true;
                }
                else 
                { 
                    // TODO: Send to LogService
                    return false; 
                }
            }
            else
            {
                return true;
            }
        }

        public void Dispose()
        {
            _appSettings.OnAppSettingsChanged -= UpdateAppSettings;
        }
    }
}
