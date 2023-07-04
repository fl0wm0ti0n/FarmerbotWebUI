using FarmerbotWebUI.Shared.NodeStatus;

namespace FarmerbotWebUI.Server.Services.Minting
{
    public interface INodeMintingReportService
    {
        List<NodeMintingCollection> ActualMintingReportCollectionList { get; }
        Task<ServiceResponse<MintingReport>> GetNodeMintingReportAsync(int nodeId, CancellationToken cancellationToken);
        Task<ServiceResponse<List<NodeMintingCollection>>> GetNodeMintingReportListAsync(CancellationToken cancellationToken);
        Task<ServiceResponse<NodeMintingCollection>> GetNodeMintingReportCollectionAsync(string botName, CancellationToken cancellationToken);
        Task<bool> CheckMintingReportsAreThere(CancellationToken cancellationToken);
    }
}
