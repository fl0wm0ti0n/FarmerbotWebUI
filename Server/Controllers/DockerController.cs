using FarmerbotWebUI.Shared;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading;

namespace FarmerbotWebUI.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DockerController : ControllerBase
    {
        private readonly IDockerService _dockerService;

        public DockerController(IDockerService dockerService)
        {
            _dockerService = dockerService;
        }

        [HttpGet("start")]
        public async Task<ActionResult<ServiceResponse<string>>> StartComposeAsync(CancellationToken cancellationToken)
        {
            var output = await _dockerService.StartComposeAsync(cancellationToken);
            return Ok(output);
        }

        [HttpGet("stop")]
        public async Task<ActionResult<ServiceResponse<string>>> StopComposeAsync(CancellationToken cancellationToken)
        {
            var output = await _dockerService.StopComposeAsync(cancellationToken);
            return Ok(output);
        }

        [HttpGet("ps")]
        public async Task<ActionResult<ServiceResponse<string>>> GetComposeProcessesAsync(CancellationToken cancellationToken)
        {
            var output = await _dockerService.GetComposeProcessesAsync(cancellationToken);
            return Ok(output);
        }

        [HttpGet("ls")]
        public async Task<ActionResult<ServiceResponse<string>>> GetComposeListAsync(CancellationToken cancellationToken)
        {
            var output = await _dockerService.GetComposeListAsync(cancellationToken);
            return Ok(output);
        }

        [HttpGet("livelog")]
        public async Task<ActionResult<ServiceResponse<string>>> GetComposeLogsAsync(CancellationToken cancellationToken)
        {
            var output = await _dockerService.GetComposeLogsAsync(cancellationToken);
            return Ok(output);
        }

        [HttpGet("status")]
        public async Task<ActionResult<ServiceResponse<FarmerBotStatus>>> GetComposeStatusAsync(CancellationToken cancellationToken)
        {
            var output = await _dockerService.GetComposeStatusAsync(cancellationToken);
            return Ok(output);
        }
    }
}