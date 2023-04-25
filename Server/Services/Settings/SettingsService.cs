using FarmerBotWebUI.Shared;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Runtime.CompilerServices;
using YamlDotNet.Core.Tokens;

namespace FarmerbotWebUI.Server.Services.Settings
{
    public class SettingsService : ISettingsService
    {
        private readonly string _configPath;
        private IAppSettings _appSettings;

        public IConfiguration Configuration { get; set; }
        public AppSettings AppSetting { get; private set; } = new AppSettings();

        public SettingsService(IAppSettings appSettings)
        {
            _configPath = "appsettings.json";
            _appSettings = appSettings;
            //try
            //{
            //    ReadConfiguration();
            //    // TODO: send to event service
            //}
            //catch (Exception ex)
            //{
            //    // TODO: send to event service
            //}
        }

        private async Task ReadConfiguration()
        {
            await Task.Run(() => {
                Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(_configPath, optional: false, reloadOnChange: true)
                .Build();
                _appSettings.SaveSettings(Configuration.Get<AppSettings>());
                //_appSettings = Configuration.Get<AppSettings>();
                AppSetting = Configuration.Get<AppSettings>();
                _appSettings.InvokeOnAppSettingsChanged(AppSetting);
            });
        }

        public async Task<ServiceResponse<IConfiguration>> ReloadConfiguration()
        {
            try
            {
                await ReadConfiguration();
                return new ServiceResponse<IConfiguration>
                {
                    Data = Configuration,
                    Message = "Configuration reloaded",
                    Success = true,
                };
                // TODO: send to event service
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IConfiguration>
                {
                    Data = Configuration,
                    Message = ex.Message,
                    Success = false,
                };
                // TODO: send to event service
            }
        }

        public async Task<ServiceResponse<string>> UpdateConfiguration(string key, string value)
        {
            try
            {
                var json = File.ReadAllText(_configPath);
                var jsonObj = JObject.Parse(json);
                jsonObj[key] = value;
                File.WriteAllText(_configPath, jsonObj.ToString());
                ReloadConfiguration();
                return new ServiceResponse<string>
                {
                    Data = value,
                    Message = "New configuration entry saved",
                    Success = true,
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>
                {
                    Data = value,
                    Message = ex.Message,
                    Success = false,
                };
            }
        }

        public ServiceResponse<AppSettings> GetConfigurationObject() 
        {
            try
            {
                ReloadConfiguration();
                return new ServiceResponse<AppSettings>
                {
                    Data = AppSetting,
                    Message = "Success getting configuration",
                    Success = true,
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<AppSettings>
                {
                    Data = AppSetting, 
                    Message = ex.Message,
                    Success = false,
                };
            }
        }

        public ServiceResponse<AppSettings> SetConfigurationObject(AppSettings appSettings)
        {
            try
            {
                var json = Serialize.ToJson(appSettings);
                var jsonObj = JObject.Parse(json);
                File.WriteAllText(_configPath, jsonObj.ToString());
                ReloadConfiguration();
                return new ServiceResponse<AppSettings>
                {
                    Data = AppSetting,
                    Message = "New configuration saved",
                    Success = true,
                };
            }
            catch (Exception ex)
            {
                // TODO: log error
                return new ServiceResponse<AppSettings>
                {
                    Data = AppSetting,
                    Message = ex.Message,
                    Success = false,
                };
            }
        }
    }
}