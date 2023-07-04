using FarmerbotWebUI.Shared.NodeStatus;

namespace FarmerbotWebUI.Server.Services.NodeStatus
{
    public interface INodeStatusService
    {
        List<NodeStatusCollection> ActualNodeStatusCollectionList { get; }
        Task<ServiceResponse<NodeStatusSet>> GetNodeStatusAsync(int nodeId, CancellationToken cancellationToken);
        Task<ServiceResponse<List<NodeStatusCollection>>> GetNodeStatusListAsync(CancellationToken cancellationToken);
        Task<ServiceResponse<NodeStatusCollection>> GetNodeStatusCollectionAsync(string botName, CancellationToken cancellationToken);
        Task<bool> CheckGridNodesAreThere(CancellationToken cancellationToken);
    }
}
