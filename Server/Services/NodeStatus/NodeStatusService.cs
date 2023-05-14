using FarmerbotWebUI.Shared.NodeStatus;

namespace FarmerbotWebUI.Server.Services.NodeStatus
{
    public class NodeStatusService : INodeStatusService
    {
        public Task<ServiceResponse<List<NodeStatusCollection>>> GetNodesStatusListAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<NodeStatusSet>> GetNodeStatusAsync(int nodeId)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<List<NodeStatusCollection>>> StartStatusIntervalAsync()
        {
            throw new NotImplementedException();
        }
    }
}
