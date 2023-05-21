using FarmerbotWebUI.Shared.NodeStatus;

namespace FarmerbotWebUI.Client.Services.NodeStatus
{
    public interface INodeStatusService
    {
        void StartStatusInterval();

        event Action StatusChanged;
        List<NodeStatusCollection> ActualNodeStatusCollectionList { get; }
        Task<ServiceResponse<NodeStatusSet>> GetNodeStatusAsync(int nodeId, EventSourceActionId id);
        Task<ServiceResponse<List<NodeStatusCollection>>> GetNodeStatusListAsync(EventSourceActionId id);
        Task<ServiceResponse<NodeStatusCollection>> GetNodeStatusCollectionAsync(EventSourceActionId id);
    }
}
