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

        [HttpGet("start/{botName}")]
        public async Task<ActionResult<ServiceResponse<FarmerBotStatus>>> StartComposeAsync(string botName, CancellationToken cancellationToken)
        {
            var output = await _dockerService.StartComposeAsync(botName, cancellationToken);
            return Ok(output);
        }

        [HttpGet("stop/{botName}")]
        public async Task<ActionResult<ServiceResponse<FarmerBotStatus>>> StopComposeAsync(string botName, CancellationToken cancellationToken)
        {
            var output = await _dockerService.StopComposeAsync(botName, cancellationToken);
            return Ok(output);
        }

        [HttpGet("status/{botName}")]
        public async Task<ActionResult<ServiceResponse<FarmerBotStatus>>> GetComposeStatusAsync(string botName, CancellationToken cancellationToken)
        {
            var output = await _dockerService.GetComposeStatusAsync(botName, cancellationToken);
            return Ok(output);
        }

        [HttpGet("ps")]
        public async Task<ActionResult<ServiceResponse<string>>> GetComposeProcessesAsync(string botName, CancellationToken cancellationToken)
        {
            var output = await _dockerService.GetComposeProcessesAsync(botName, cancellationToken);
            return Ok(output);
        }

        [HttpGet("ls")]
        public async Task<ActionResult<ServiceResponse<string>>> GetComposeListAsync(string botName, CancellationToken cancellationToken)
        {
            var output = await _dockerService.GetComposeListAsync(botName, cancellationToken);
            return Ok(output);
        }

        [HttpGet("livelog")]
        public async Task<ActionResult<ServiceResponse<string>>> GetComposeLogsAsync(string botName, CancellationToken cancellationToken)
        {
            var output = await _dockerService.GetComposeLogsAsync(botName, cancellationToken);
            return Ok(output);
        }
    }
}