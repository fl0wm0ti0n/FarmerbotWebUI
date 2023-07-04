using FarmerbotWebUI.Shared.NodeStatus;
using FarmerBotWebUI.Shared;
using Markdig.Syntax.Inlines;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading;

namespace FarmerbotWebUI.Server.Services.Minting
{
    public class NodeMintingReportService : INodeMintingReportService
    {
        private IAppSettings _appSettings;
        private readonly INodeMintingApiClient _nodeMintingApiClient;
        private readonly IFileService _fileService;
        public List<NodeMintingCollection> ActualMintingReportCollectionList { get; private set; } = new List<NodeMintingCollection>();

        public NodeMintingReportService(IAppSettings appSettings, INodeMintingApiClient nodeMintingApiClient, IFileService fileService)
        {
            _appSettings = appSettings;
            _nodeMintingApiClient = nodeMintingApiClient;
            _fileService = fileService;
            _appSettings.OnAppSettingsChanged += UpdateAppSettings;
        }

        private void UpdateAppSettings(object sender, AppSettings newAppSettings)
        {
            _appSettings = newAppSettings;
        }

        public async Task<ServiceResponse<List<NodeMintingCollection>>> GetNodeMintingReportListAsync(CancellationToken cancellationToken)
        {
            bool success = true;
            StringBuilder sb = new StringBuilder();

            List<NodeMintingCollection> nodeStatusCollection = new List<NodeMintingCollection>();
            foreach (var bot in _appSettings.FarmerBotSettings.Bots)
            {
                var nodeCollection = await GetNodeMintingReportCollectionAsync(bot.BotName, cancellationToken);
                if (nodeCollection.Success)
                {
                    nodeStatusCollection.Add(nodeCollection.Data);
                    ActualMintingReportCollectionList.Remove(nodeCollection.Data);
                    ActualMintingReportCollectionList.Add(nodeCollection.Data);
                }
                else
                {
                    sb.AppendLine(nodeCollection.Message);
                    success = false;
                }
            }

            return new ServiceResponse<List<NodeMintingCollection>>()
            {
                Data = nodeStatusCollection,
                Message = sb.ToString(),
                Success = success,
            };
        }

        public async Task<ServiceResponse<NodeMintingCollection>> GetNodeMintingReportCollectionAsync(string botName, CancellationToken cancellationToken)
        {
            bool error = false;
            string errorMessage = "";
            bool noStatus = false;
            List<MintingReport> mintingReportList = new List<MintingReport>();

            BotSetting bot = _appSettings.FarmerBotSettings.Bots.FirstOrDefault(x => x.BotName == botName);
            if (bot == null)
            {
                error = true;
                noStatus = true;
                errorMessage = $"Didn't find Bot {botName} in BotConfig.";
            }
            else if (await CheckMintingReportsAreThere(cancellationToken))
            {
                // get the right MintingResult-list with botName which is present in botconfig
                var mintingResultList = _nodeMintingApiClient.RawApiData.FirstOrDefault(r => r.Key.Equals(botName)).Value;

                if (mintingResultList == null)
                {
                    error = true;
                    noStatus = true;
                    errorMessage = $"Didn't find Nodes on Grid with given FarmId {bot.FarmId} from Bot {bot.BotName}.";
                    // TODO: Send to LogService
                }
                else
                {
                    foreach (var node in mintingResultList)
                    {
                        mintingReportList.Add(new MintingReport()
                        {
                            Hash = node.Hash,
                            Receipt = node.Receipt,
                            BotName = bot.BotName,
                            LastUpdate = DateTime.UtcNow,
                            NoStatus = false,
                            IsError = false,
                            ErrorMessage = ""
                        });
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

            return new ServiceResponse<NodeMintingCollection>()
            {
                Data = new NodeMintingCollection()
                {
                    BotName = bot.BotName,
                    Farm = new Farm() 
                    { 
                        FarmId = bot.FarmId
                    },
                    NodeStatusSets = mintingReportList,
                    LastUpdate = DateTime.UtcNow,
                    NoStatus = noStatus
                },
                Message = errorMessage,
                Success = !error,
            };
        }

        public async Task<ServiceResponse<MintingReport>> GetNodeMintingReportAsync(int nodeId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> CheckMintingReportsAreThere (CancellationToken cancellationToken)
        {
            if (_nodeMintingApiClient.RawApiData.Count == 0)
            {
                var apiResult = await _nodeMintingApiClient.GetMintingOfaNodeListAsync(cancellationToken);
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
