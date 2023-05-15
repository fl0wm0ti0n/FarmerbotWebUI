using FarmerbotWebUI.Server.Services.NodeStatus;
using FarmerbotWebUI.Shared.BotConfig;
using FarmerbotWebUI.Shared.NodeStatus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FarmerbotWebUI.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NodeStatusController : ControllerBase
    {
        private readonly INodeStatusService _nodeStatusService;

        public NodeStatusController(INodeStatusService nodeStatusService)
        {
            _nodeStatusService = nodeStatusService;
        }

        [HttpGet("status/list")]
        public async Task<ActionResult<ServiceResponse<List<NodeStatusCollection>>>> GetNodeStatusListAsync(CancellationToken cancellationToken)
        {
            var output = await _nodeStatusService.GetNodeStatusListAsync(cancellationToken);
            return Ok(output);
        }

        [HttpGet("status")]
        public async Task<ActionResult<ServiceResponse<NodeStatusCollection>>> GetNodeStatusCollectionAsync(string botName, CancellationToken cancellationToken)
        {
            var output = await _nodeStatusService.GetNodeStatusCollectionAsync(botName, cancellationToken);
            return Ok(output);
        }

        [HttpGet("status/{nodeId}")]
        public async Task<ActionResult<ServiceResponse<NodeStatusSet>>> GetNodeStatusAsync(int nodeId, CancellationToken cancellationToken)
        {
            var output = await _nodeStatusService.GetNodeStatusAsync(nodeId, cancellationToken);
            return Ok(output);
        }

    }
}
