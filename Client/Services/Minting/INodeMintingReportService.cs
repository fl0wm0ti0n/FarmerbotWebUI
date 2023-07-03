using FarmerbotWebUI.Shared.NodeStatus;

namespace FarmerbotWebUI.Client.Services.Minting
{
    public interface INodeMintingReportService
    {
        void StartStatusInterval();

        event Action StatusChanged;
        List<NodeMintingCollection> ActualMintingReportCollectionList { get; }
        Task<ServiceResponse<MintingReport>> GetNodeMintingReportAsync(int nodeId, EventSourceActionId id);
        Task<ServiceResponse<List<NodeMintingCollection>>> GetNodeMintingReportListAsync(EventSourceActionId id);
        Task<ServiceResponse<NodeMintingCollection>> GetNodeMintingReportCollectionAsync(EventSourceActionId id);
    }
}
