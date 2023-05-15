using FarmerbotWebUI.Shared.NodeStatus;

namespace FarmerbotWebUI.Client.Services.NodeStatus
{
    public interface INodeStatusService
    {
        Task<ServiceResponse<List<NodeStatusCollection>>> StartStatusIntervalAsync(CancellationToken cancellationToken);
        Task<ServiceResponse<NodeStatusSet>> GetNodeStatusAsync(int nodeId, CancellationToken cancellationToken);
        Task<ServiceResponse<List<NodeStatusCollection>>> GetNodeStatusListAsync(CancellationToken cancellationToken);
        Task<ServiceResponse<NodeStatusCollection>> GetNodeStatusCollectionAsync(CancellationToken cancellationToken);
    }
}
