using FarmerbotWebUI.Shared.NodeStatus;
using FarmerBotWebUI.Shared;
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
            List<Nodes> nodes = new List<Nodes>();

            if (_tfGraphQLApiClient.Nodes.Count == 0)
            {
                var apiResult = await _tfGraphQLApiClient.GetNodesListAsync(cancellationToken);
                if (apiResult.Success)
                {
                    nodes = apiResult.Data;
                }
                else
                {
                    return new ServiceResponse<NodeStatusCollection>()
                    {
                        Data = new NodeStatusCollection(),
                        Message = apiResult.Message,
                        Success = apiResult.Success,
                    };
                }
            }
            else
            {
                nodes = _tfGraphQLApiClient.Nodes;

                return new ServiceResponse<NodeStatusCollection>()
                {
                    Data = _tfGraphQLApiClient.Nodes,
                    Message = "",
                    Success = true,
                };
            }
        }

        public async Task<ServiceResponse<NodeStatusSet>> GetNodeStatusAsync(int nodeId, CancellationToken cancellationToken)
        {
            new NodeStatusSet()
            {
                BotNode = new BotNode() { },
                GridNode = new Node() { },
                Farm = new Farm() { },
                LastUpdate = DateTime.UtcNow,
                NoStatus = false,
                IsError = false,
                ErrorMessage = "",
            };
        }

        private async Task<NodeStatusCollection> AssembleNodeStatusCollection(List<Nodes> listOfNodeLists, CancellationToken cancellationToken)
        {
            foreach (Nodes rawNodeResult in listOfNodeLists)
            {
                var nodes = rawNodeResult.Data.Nodes;
                var fileResult = await _fileService.GetMarkdownConfigAsync(cancellationToken);
            }

            NodeStatusCollection nodeStatusCollection = new NodeStatusCollection()
            {
                NodeStatusSets = 
                LastUpdate = DateTime.UtcNow,
                NoStatus = false
            };
        }
    }
}
