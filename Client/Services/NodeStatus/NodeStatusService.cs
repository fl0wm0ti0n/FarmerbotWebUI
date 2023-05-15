using FarmerbotWebUI.Shared.NodeStatus;

namespace FarmerbotWebUI.Client.Services.NodeStatus
{
    public class NodeStatusService : INodeStatusService
    {
        public Task<ServiceResponse<NodeStatusSet>> GetNodeStatusAsync(int nodeId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<NodeStatusCollection>> GetNodeStatusCollectionAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<List<NodeStatusCollection>>> GetNodeStatusListAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<List<NodeStatusCollection>>> StartStatusIntervalAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
