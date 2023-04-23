using FarmerBotWebUI.Shared;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading;

namespace FarmerbotWebUI.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private readonly ISettingsService _settingsService;

        public SettingsController(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        [HttpGet("getsettings")]
        public async Task<ActionResult<ServiceResponse<AppSettings>>> GetConfigurationObjectAsync()
        {
            var output = _settingsService.GetConfigurationObject();
            return Ok(output);
        }

        [HttpPost("setsettings")]
        public async Task<ActionResult<ServiceResponse<AppSettings>>> SetConfigurationObjectAsync(AppSettings appSettings)
        {
            var output = _settingsService.SetConfigurationObject(appSettings);
            return Ok(output);
        }
    }
}