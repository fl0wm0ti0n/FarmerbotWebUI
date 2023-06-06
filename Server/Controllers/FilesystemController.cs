using FarmerbotWebUI.Shared.BotConfig;
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

        #region Misc
        [HttpGet("log/{botName}")]
        public async Task<ActionResult<ServiceResponse<string>>> GetLocalLogAsync(string botName, CancellationToken cancellationToken)
        {
            var output = await _filesystemService.GetLocalLogAsync(botName, cancellationToken);
            return Ok(output);
        }
        #endregion Misc

        #region Compose
        [HttpGet("compose/getraw/{botName}")]
        public async Task<ActionResult<ServiceResponse<string>>> GetRawComposeFileAsync(string botName, CancellationToken cancellationToken)
        {
            var output = await _filesystemService.GetRawComposeFileAsync(botName, cancellationToken);
            return Ok(output);
        }

        [HttpGet("compose/get/{botName}")]
        public async Task<ActionResult<ServiceResponse<DockerCompose>>> GetComposeFileAsync(string botName, CancellationToken cancellationToken)
        {
            var output = await _filesystemService.GetComposeFileAsync(botName, cancellationToken);
            return Ok(output);
        }

        [HttpPost("compose/setraw/{botName}")]
        public async Task<ActionResult<ServiceResponse<string>>> SetRawComposeFileAsync([FromBody] string compose, string botName, CancellationToken cancellationToken)
        {
            var output = await _filesystemService.SetRawComposeFileAsync(compose, botName, cancellationToken);
            return Ok(output);
        }

        [HttpPost("compose/set/{botName}")]
        public async Task<ActionResult<ServiceResponse<string>>> SetComposeFileAsync([FromBody] DockerCompose compose, string botName, CancellationToken cancellationToken)
        {
            var output = await _filesystemService.SetComposeFileAsync(compose, botName, cancellationToken);
            return Ok(output);
        }
        #endregion Compose

        #region Config
        [HttpGet("markdown/getraw/{botName}")]
        public async Task<ActionResult<ServiceResponse<string>>> GetRawMarkdownConfigAsync(string botName, CancellationToken cancellationToken)
        {
            var output = await _filesystemService.GetRawMarkdownConfigAsync(botName, cancellationToken);
            return Ok(output);
        }

        [HttpGet("markdown/get/{botName}")]
        public async Task<ActionResult<ServiceResponse<FarmerBotConfig>>> GetMarkdownConfigAsync(string botName, CancellationToken cancellationToken)
        {
            var output = await _filesystemService.GetMarkdownConfigAsync(botName, cancellationToken);
            return Ok(output);
        }

        [HttpPost("markdown/setraw/{botName}")]
        public async Task<ActionResult<ServiceResponse<string>>> SetRawMarkdownConfigAsync([FromBody] string compose, string botName, CancellationToken cancellationToken)
        {
            var output = await _filesystemService.SetRawMarkdownConfigAsync(compose, botName, cancellationToken);
            return Ok(output);
        }

        [HttpPost("markdown/set/{botName}")]
        public async Task<ActionResult<ServiceResponse<string>>> SetMarkdownConfigAsync([FromBody] FarmerBotConfig compose, string botName, CancellationToken cancellationToken)
        {
            var output = await _filesystemService.SetMarkdownConfigAsync(compose, botName, cancellationToken);
            return Ok(output);
        }
        #endregion Config
        #region Env
        [HttpGet("env/getraw/{botName}")]
        public async Task<ActionResult<ServiceResponse<string>>> GetRawEnvFileAsync(string botName, CancellationToken cancellationToken)
        {
            var output = await _filesystemService.GetRawEnvFileAsync(botName, cancellationToken);
            return Ok(output);
        }

        [HttpGet("env/get/{botName}")]
        public async Task<ActionResult<ServiceResponse<EnvFile>>> GetEnvFileAsync(string botName, CancellationToken cancellationToken)
        {
            var output = await _filesystemService.GetEnvFileAsync(botName, cancellationToken);
            return Ok(output);
        }

        [HttpPost("env/setraw/{botName}")]
        public async Task<ActionResult<ServiceResponse<string>>> SetRawEnvFileAsync([FromBody] string env, string botName, CancellationToken cancellationToken)
        {
            var output = await _filesystemService.SetRawEnvFileAsync(env, botName, cancellationToken);
            return Ok(output);
        }

        [HttpPost("env/set/{botName}")]
        public async Task<ActionResult<ServiceResponse<string>>> SetEnvFileAsync([FromBody] EnvFile env, string botName, CancellationToken cancellationToken)
        {
            var output = await _filesystemService.SetEnvFileAsync(env, botName, cancellationToken);
            return Ok(output);
        }
        #endregion Env
        #region FarmerBot
        [HttpGet("bot/get")]
        public async Task<ActionResult<ServiceResponse<List<FarmerBot>>>> GetFarmerBotListAsync(CancellationToken cancellationToken)
        {
            var output = await _filesystemService.GetFarmerBotListAsync(cancellationToken);
            return Ok(output);
        }

        [HttpGet("bot/get/{botName}")]
        public async Task<ActionResult<ServiceResponse<FarmerBot>>> GetFarmerBotAsync(string botName, CancellationToken cancellationToken)
        {
            var output = await _filesystemService.GetFarmerBotAsync(botName, cancellationToken);
            return Ok(output);
        }

        [HttpPost("bot/set")]
        public async Task<ActionResult<ServiceResponse<string>>> SetFarmerBotAsync([FromBody] FarmerBot bot, CancellationToken cancellationToken)
        {
            var output = await _filesystemService.SetFarmerBotAsync(bot, cancellationToken);
            return Ok(output);
        }
        #endregion FarmerBot
    }
}