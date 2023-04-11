using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading;

namespace FarmerbotWebUI.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesystemController : ControllerBase
    {
        private readonly IFileService _filesystemService;

        public FilesystemController(IFileService filesystemService)
        {
            _filesystemService = filesystemService;
        }

        [HttpGet("log/{path?}")]
        public async Task<ActionResult<ServiceResponse<string>>> GetLocalLogAsync(string path, CancellationToken cancellationToken)
        {
            var output = await _filesystemService.GetLocalLogAsync(path, cancellationToken);
            return Ok(output);
        }

        [HttpPost("compose/get/{path?}")]
        public async Task<ActionResult<ServiceResponse<string>>> GetComposeFileAsync(string path, CancellationToken cancellationToken)
        {
            var output = await _filesystemService.GetComposeFileAsync(path, cancellationToken);
            return Ok(output);
        }

        [HttpPost("compose/set/{path?}")]
        public async Task<ActionResult<ServiceResponse<string>>> SetComposeFileAsync([FromBody] string compose, string path, CancellationToken cancellationToken)
        {
            var output = await _filesystemService.SetComposeFileAsync(compose, path, cancellationToken);
            return Ok(output);
        }

        [HttpPost("markdown/get/{path?}")]
        public async Task<ActionResult<ServiceResponse<string>>> GetMarkdownConfigAsync(string path, CancellationToken cancellationToken)
        {
            var output = await _filesystemService.GetMarkdownConfigAsync(path, cancellationToken);
            return Ok(output);
        }

        [HttpPost("markdown/getraw/{path?}")]
        public async Task<ActionResult<ServiceResponse<string>>> GetRawMarkdownConfigAsync(string path, CancellationToken cancellationToken)
        {
            var output = await _filesystemService.GetRawMarkdownConfigAsync(path, cancellationToken);
            return Ok(output);
        }

        [HttpPost("markdown/set/{path?}")]
        public async Task<ActionResult<ServiceResponse<string>>> SetMarkdownConfigAsync([FromBody] FarmerBotConfig compose, string path, CancellationToken cancellationToken)
        {
            var output = await _filesystemService.SetMarkdownConfigAsync(compose, path, cancellationToken);
            return Ok(output);
        }

        [HttpPost("markdown/setraw/{path?}")]
        public async Task<ActionResult<ServiceResponse<string>>> SetRawMarkdownConfigAsync([FromBody] string compose, string path, CancellationToken cancellationToken)
        {
            var output = await _filesystemService.SetMarkdownConfigAsync(compose, path, cancellationToken);
            return Ok(output);
        }
    }
}