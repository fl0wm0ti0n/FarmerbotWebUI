
using FarmerbotWebUI.Shared.NodeStatus;

namespace FarmerbotWebUI.Server.Services.NodeStatus
{
    public interface INodeStatusService
    {
        Task<ServiceResponse<List<NodeStatusCollection>>> StartStatusIntervalAsync();
        Task<ServiceResponse<NodeStatusSet>> GetNodeStatusAsync(int nodeId);
        Task<ServiceResponse<List<NodeStatusCollection>>> GetNodesStatusListAsync();
    }
}
