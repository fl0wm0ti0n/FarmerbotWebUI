using FarmerbotWebUI.Shared;
using Microsoft.AspNetCore.Mvc;
using System.IO;

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
        public async Task<ActionResult<ServiceResponse<string>>> StartComposeAsync()
        {
            var output = await _dockerService.StartComposeAsync();
            return Ok(output);
        }

        [HttpGet("stop")]
        public async Task<ActionResult<ServiceResponse<string>>> StopComposeAsync()
        {
            var output = await _dockerService.StopComposeAsync();
            return Ok(output);
        }

        [HttpGet("ps")]
        public async Task<ActionResult<ServiceResponse<string>>> GetComposeProcessesAsync()
        {
            var output = await _dockerService.GetComposeProcessesAsync();
            return Ok(output);
        }

        [HttpGet("ls")]
        public async Task<ActionResult<ServiceResponse<string>>> GetComposeListAsync()
        {
            var output = await _dockerService.GetComposeListAsync();
            return Ok(output);
        }

        [HttpGet("livelog")]
        public async Task<ActionResult<ServiceResponse<string>>> GetComposeLogsAsync()
        {
            var output = await _dockerService.GetComposeLogsAsync();
            return Ok(output);
        }

        [HttpGet("log/{path?}")]
        public async Task<ActionResult<ServiceResponse<string>>> GetLocalLogAsync(string path)
        {
            var output = await _dockerService.GetLocalLogAsync(path);
            return Ok(output);
        }

        [HttpPost("compose/get/{path?}")]
        public async Task<ActionResult<ServiceResponse<string>>> GetComposeFileAsync(string path)
        {
            var output = await _dockerService.GetComposeFileAsync(path);
            return Ok(output);
        }

        [HttpPost("compose/set/{path?}")]
        public async Task<ActionResult<ServiceResponse<string>>> SetComposeFileAsync([FromBody] string compose, string path)
        {
            var output = await _dockerService.SetComposeFileAsync(compose, path);
            return Ok(output);
        }

        [HttpPost("markdown/get/{path?}")]
        public async Task<ActionResult<ServiceResponse<string>>> GetMarkdownConfigAsync(string path)
        {
            var output = await _dockerService.GetMarkdownConfigAsync(path);
            return Ok(output);
        }

        [HttpPost("markdown/getraw/{path?}")]
        public async Task<ActionResult<ServiceResponse<string>>> GetRawMarkdownConfigAsync(string path)
        {
            var output = await _dockerService.GetRawMarkdownConfigAsync(path);
            return Ok(output);
        }

        [HttpPost("markdown/set/{path?}")]
        public async Task<ActionResult<ServiceResponse<string>>> SetMarkdownConfigAsync([FromBody] FarmerBotConfig compose, string path)
        {
            var output = await _dockerService.SetMarkdownConfigAsync(compose, path);
            return Ok(output);
        }

        [HttpPost("markdown/setraw/{path?}")]
        public async Task<ActionResult<ServiceResponse<string>>> SetRawMarkdownConfigAsync([FromBody] string compose, string path)
        {
            var output = await _dockerService.SetMarkdownConfigAsync(compose, path);
            return Ok(output);
        }
    }
}