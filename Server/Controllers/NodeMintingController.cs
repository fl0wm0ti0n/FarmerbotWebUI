using FarmerbotWebUI.Server.Services.Minting;
using FarmerbotWebUI.Server.Services.NodeStatus;
using FarmerbotWebUI.Shared.BotConfig;
using FarmerbotWebUI.Shared.NodeStatus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FarmerbotWebUI.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NodeMintingController : ControllerBase
    {
        private readonly INodeMintingReportService _nodeMintingReportService;

        public NodeMintingController(INodeMintingReportService nodeMintingReportService)
        {
            _nodeMintingReportService = nodeMintingReportService;
        }

        [HttpGet("nodes/list")]
        public async Task<ActionResult<ServiceResponse<List<NodeStatusCollection>>>> GetNodeStatusListAsync(CancellationToken cancellationToken)
        {
            var output = await _nodeMintingReportService.GetNodeMintingReportListAsync(cancellationToken);
            return Ok(output);
        }

        [HttpGet("nodes")]
        public async Task<ActionResult<ServiceResponse<NodeStatusCollection>>> GetNodeStatusCollectionAsync(string botName, CancellationToken cancellationToken)
        {
            var output = await _nodeMintingReportService.GetNodeMintingReportCollectionAsync(botName, cancellationToken);
            return Ok(output);
        }

        [HttpGet("nodes/{nodeId}")]
        public async Task<ActionResult<ServiceResponse<NodeStatusSet>>> GetNodeStatusAsync(int nodeId, CancellationToken cancellationToken)
        {
            var output = await _nodeMintingReportService.GetNodeMintingReportAsync(nodeId, cancellationToken);
            return Ok(output);
        }
    }
}
