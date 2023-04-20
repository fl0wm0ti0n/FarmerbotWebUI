using FarmerBotWebUI.Shared;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using YamlDotNet.Core.Tokens;

namespace FarmerbotWebUI.Server.Services.Settings
{
    public class SettingsService : ISettingsService
    {
        private readonly string _configPath;
        public IConfiguration Configuration { get; set; }
        public AppSettings AppSetting { get; private set; }

        public SettingsService(string configPath)
        {
            _configPath = configPath;
        }

        public ServiceResponse<IConfiguration> ReloadConfiguration()
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(_configPath, optional: false, reloadOnChange: true)
                .Build();
            AppSetting = AppSettings.FromJson(Configuration.ToString());
            return Configuration;
        }

        public ServiceResponse<bool> UpdateConfiguration(string key, string value)
        {
            var json = File.ReadAllText(_configPath);
            var jsonObj = JObject.Parse(json);
            jsonObj[key] = value;
            File.WriteAllText(_configPath, jsonObj.ToString());
            ReloadConfiguration();
            return true;
        }

        public ServiceResponse<AppSettings> GetConfigurationObject() 
        {
            ReloadConfiguration();
            return AppSetting;
        }

        public ServiceResponse<bool> SetConfigurationObject(AppSettings appSettings)
        {
            try
            {
                var json = Serialize.ToJson(appSettings);
                var jsonObj = JObject.Parse(json);
                File.WriteAllText(_configPath, jsonObj.ToString());
            }
            catch (Exception ex)
            {
                // TODO: log error
                return new ServiceResponse<bool>
                {
                    Data = false,
                    Message = ex.Message,
                    Success = false,
                };
            }
            ReloadConfiguration();
            return new ServiceResponse<bool>
            {
                Data = true,
                Message = "New configuration saved",
                Success = true,
            };
        }
    }
}