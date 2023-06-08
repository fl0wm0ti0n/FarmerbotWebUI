using FarmerbotWebUI.Shared.BotConfig;
using FarmerbotWebUI.Shared.NodeStatus;
using FarmerBotWebUI.Shared;
using System.Text;

namespace FarmerbotWebUI.Server.Services.Filesystem
{
    public class FileService : IFileService
    {
        private IAppSettings _appSettings;

        public FileService(IAppSettings appSettings)
        {
            _appSettings = appSettings;
            _appSettings.OnAppSettingsChanged += UpdateAppSettings;
        }

        #region Private

        private void UpdateAppSettings(object sender, AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        private async Task<ServiceResponse<string>> HandleFileOperationAsync(string botName, Func<BotSetting, string> fileNameFunc, Func<string, CancellationToken, Task<string>> fileOperationFunc, CancellationToken cancellationToken)
        {
            string error = "";
            string output = "";

            if (!string.IsNullOrWhiteSpace(botName))
            {
                var bot = _appSettings.FarmerBotSettings.Bots.FirstOrDefault(b => b.BotName == botName);
                if (bot != null)
                {
                    var path = $"{bot.WorkingDirectory}{Path.DirectorySeparatorChar}{fileNameFunc(bot)}";
                    try
                    {
                        // Execute the provided file operation (read or write)

                        output = await fileOperationFunc(path, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        // return error
                        error = ex.Message;
                        output = error;
                    }
                }
                else
                {
                    error = $"Bot {botName} not found!";
                    output = error;
                    // TODO: Send it to Logservice
                }
            }
            else
            {
                error = $"Botname is empty!";
                output = error;
            }

            if (cancellationToken.IsCancellationRequested)
            {
                error = "Operation cancelled!";
                output = error;
            }

            ServiceResponse<string> response = new ServiceResponse<string>
            {
                Data = output,
                Message = error,
                Success = string.IsNullOrWhiteSpace(error)
            };

            return response;
        }

        private async Task<EnvFile> ParseFromEnvFile(string path, CancellationToken ct)
        {
            EnvFile env = new EnvFile();
            try
            {
                string text = await File.ReadAllTextAsync(path, ct);
                env.Deserialize(text);
                env.IsError = false;
                return env;
            }
            catch (Exception ex)
            {
                env.IsError = true;
                env.ErrorMessage = ex.Message;
                return env;
                // TODO: send to logservice
            }
        }
        private async Task<bool> WriteToEnvFile(string path, EnvFile env, CancellationToken ct)
        {
            try
            {
                var text = env.Serialize();
                await File.WriteAllTextAsync(path, text, ct);
                return true;
            }
            catch (Exception ex)
            {
                // TODO: send to logservice
                return false;
            }
        }

        private async Task<DockerCompose> ParseFromComposeFile(string path, CancellationToken ct)
        {
            DockerCompose dockerCompose = new DockerCompose();
            try
            {
                string text = await File.ReadAllTextAsync(path, ct);
                dockerCompose.DeserializeYaml(text);
                dockerCompose.IsError = false;
                return dockerCompose;
            }
            catch (Exception ex)
            {
                dockerCompose.IsError = true;
                dockerCompose.ErrorMessage = ex.Message;
                return dockerCompose;
            }
        }

        private async Task<bool> WriteToComposeFile(string path, DockerCompose compose, CancellationToken ct)
        {
            try
            {
                var text = compose.SerializeYaml();
                await File.WriteAllTextAsync(path, text, ct);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private async Task<FarmerBotConfig> ParseFromMarkdownFile(string path, CancellationToken ct)
        {
            FarmerBotConfig botConfig = new FarmerBotConfig();
            try
            {
                string text = await File.ReadAllTextAsync(path, ct);
                botConfig.Deserialize(text);
                botConfig.IsError = false;
                return botConfig;
            }
            catch (Exception ex)
            {
                botConfig.IsError = true;
                botConfig.ErrorMessage = ex.Message;
                return botConfig;
            }
        }

        private async Task<bool> WriteToMarkdownFile(string path, FarmerBotConfig botConfig, CancellationToken ct)
        {
            try
            {
                var text = botConfig.Serialize();
                await File.WriteAllTextAsync(path, text, ct);
                return true;
            }
            catch (Exception ex)
            {
                // TODO: send to logservice
                return false;
            }
        }

        #endregion Private

        #region Misc
        public async Task<ServiceResponse<string>> GetLocalLogAsync(string botName, CancellationToken cancellationToken)
        {
            return await HandleFileOperationAsync(botName, bot => bot.FarmerBotLogFile, (path, ct) => File.ReadAllTextAsync(path, ct), cancellationToken);
        }
        #endregion Misc

        #region Markdown
        public async Task<ServiceResponse<string>> GetRawMarkdownConfigAsync(string botName, CancellationToken cancellationToken)
        {
            return await HandleFileOperationAsync(botName, bot => bot.FarmerBotConfigFile, (path, ct) => File.ReadAllTextAsync(path, ct), cancellationToken);
        }

        public async Task<ServiceResponse<FarmerBotConfig>> GetMarkdownConfigAsync(string botName, CancellationToken cancellationToken)
        {
            string error = "";
            FarmerBotConfig farmerBotConfig = new FarmerBotConfig();

            if (!string.IsNullOrWhiteSpace(botName))
            {
                var botConfig = _appSettings.FarmerBotSettings.Bots.FirstOrDefault(b => b.BotName == botName);
                if (botConfig != null)
                {
                    var ConfigPath = $"{botConfig.WorkingDirectory}{Path.DirectorySeparatorChar}{botConfig.FarmerBotConfigFile}";

                    try
                    {
                        farmerBotConfig = await ParseFromMarkdownFile(ConfigPath, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        error = ex.Message;
                    }
                }
                else
                {
                    error = $"Bot {botName} not found!";
                }
            }
            else
            {
                error = $"Botname is empty!";
            }

            if (cancellationToken.IsCancellationRequested)
            {
                error = "Operation cancelled!";
            }

            if (farmerBotConfig.IsError)
            {
                error = farmerBotConfig.ErrorMessage;
            }

            if (error != null)
            {
                //TODO: Send it to Logservice
            }

            ServiceResponse<FarmerBotConfig> response = new ServiceResponse<FarmerBotConfig>
            {
                Data = farmerBotConfig,
                Message = error,
                Success = string.IsNullOrWhiteSpace(error)
            };

            return response;
        }

        public async Task<ServiceResponse<string>> SetRawMarkdownConfigAsync(string compose, string botName, CancellationToken cancellationToken)
        {
            return await HandleFileOperationAsync(botName, bot => bot.FarmerBotConfigFile, async (path, ct) =>
            {
                await File.WriteAllTextAsync(path, compose, ct);
                return "File written successfully!";
            }, cancellationToken);
        }

        public async Task<ServiceResponse<string>> SetMarkdownConfigAsync(FarmerBotConfig config, string botName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        #endregion Markdown

        #region Compose
        public async Task<ServiceResponse<string>> GetRawComposeFileAsync(string botName, CancellationToken cancellationToken)
        {
            return await HandleFileOperationAsync(botName, bot => bot.ComposeFile, (path, ct) => File.ReadAllTextAsync(path, ct), cancellationToken);
        }

        public Task<ServiceResponse<DockerCompose>> GetComposeFileAsync(string botName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<string>> SetRawComposeFileAsync(string compose, string botName, CancellationToken cancellationToken)
        {
            return await HandleFileOperationAsync(botName, bot => bot.ComposeFile, async (path, ct) =>
            {
                await File.WriteAllTextAsync(path, compose, ct);
                return "File written successfully!";
            }, cancellationToken);
        }

        public Task<ServiceResponse<string>> SetComposeFileAsync(DockerCompose compose, string botName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        #endregion Compose

        #region Env
        public Task<ServiceResponse<string>> GetRawEnvFileAsync(string botName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<EnvFile>> GetEnvFileAsync(string botName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<string>> SetRawEnvFileAsync(string env, string botName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<string>> SetEnvFileAsync(EnvFile env, string botName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        #endregion Env

        #region FarmerBot

        public async Task<ServiceResponse<List<FarmerBot>>> GetFarmerBotListAsync(CancellationToken cancellationToken)
        {
            bool success = true;
            StringBuilder sb = new StringBuilder();

            List<FarmerBot> farmerBotList = new List<FarmerBot>();
            foreach (var bot in _appSettings.FarmerBotSettings.Bots)
            {
                var farmerBot = await GetFarmerBotAsync(bot.BotName, cancellationToken);
                if (farmerBot.Success)
                {
                    farmerBotList.Add(farmerBot.Data);
                }
                else
                {
                    sb.AppendLine(farmerBot.Message);
                    success = false;
                }
            }

            ServiceResponse<List<FarmerBot>> response = new ServiceResponse<List<FarmerBot>>
            {
                Data = farmerBotList,
                Message = sb.ToString(),
                Success = success,
            };

            return response;
        }

        public async Task<ServiceResponse<FarmerBot>> GetFarmerBotAsync(string botName, CancellationToken cancellationToken)
        {
            string error = "";
            FarmerBot farmerBot = new FarmerBot();

            if (!string.IsNullOrWhiteSpace(botName))
            {
                var botConfig = _appSettings.FarmerBotSettings.Bots.FirstOrDefault(b => b.BotName == botName);
                if (botConfig != null)
                {
                    var composePath = $"{botConfig.WorkingDirectory}{Path.DirectorySeparatorChar}{botConfig.ComposeFile}";
                    var EnvPath = $"{botConfig.WorkingDirectory}{Path.DirectorySeparatorChar}{botConfig.ThreefoldNetworkFile}";
                    var ConfigPath = $"{botConfig.WorkingDirectory}{Path.DirectorySeparatorChar}{botConfig.FarmerBotConfigFile}";

                    try
                    {
                        farmerBot.DockerCompose = await ParseFromComposeFile(composePath, cancellationToken);
                        farmerBot.EnvFile = await ParseFromEnvFile(EnvPath, cancellationToken);
                        farmerBot.FarmerBotConfig = await ParseFromMarkdownFile(ConfigPath, cancellationToken);
                        farmerBot.Name = botName;
                    }
                    catch (Exception ex)
                    {
                        error = ex.Message;
                    }
                }
                else
                {
                    error = $"Bot {botName} not found!";
                }
            }
            else
            {
                error = $"Botname is empty!";
            }

            if (cancellationToken.IsCancellationRequested)
            {
                error = "Operation cancelled!";
            }

            if (farmerBot.IsError)
            {
                error = farmerBot.ErrorMessage;
            }

            if (error != null)
            {
                //TODO: Send it to Logservice
            }

            ServiceResponse<FarmerBot> response = new ServiceResponse<FarmerBot>
            {
                Data = farmerBot,
                Message = error,
                Success = string.IsNullOrWhiteSpace(error)
            };

            return response;
        }

        public async Task<ServiceResponse<string>> SetFarmerBotAsync(FarmerBot bot, CancellationToken cancellationToken)
        {
            string error = "";
            string output = "File written successfully!";
            FarmerBot farmerBot = new FarmerBot();

            if (!string.IsNullOrWhiteSpace(bot.Name))
            {
                var botConfig = _appSettings.FarmerBotSettings.Bots.FirstOrDefault(b => b.BotName == bot.Name);
                if (botConfig != null)
                {
                    var composePath = $"{botConfig.WorkingDirectory}{Path.DirectorySeparatorChar}{botConfig.ComposeFile}";
                    var EnvPath = $"{botConfig.WorkingDirectory}{Path.DirectorySeparatorChar}{botConfig.ThreefoldNetworkFile}";
                    var ConfigPath = $"{botConfig.WorkingDirectory}{Path.DirectorySeparatorChar}{botConfig.FarmerBotConfigFile}";

                    try
                    {
                        var success = await WriteToComposeFile(composePath, bot.DockerCompose, cancellationToken);
                        if (success)
                        {
                            success = await WriteToEnvFile(EnvPath, bot.EnvFile, cancellationToken);
                            if (success)
                            {
                                success = await WriteToMarkdownFile(ConfigPath, bot.FarmerBotConfig, cancellationToken);
                                if (success)
                                {
                                    error = "Bot updated successfully!";
                                }
                                else
                                {
                                    error = "Error while updating FarmerBotConfig!";
                                }
                            }
                            else
                            {
                                error = "Error while updating EnvFile!";
                            }
                        }
                        else
                        {
                            error = "Error while updating DockerCompose!";
                        }   
                    }
                    catch (Exception ex)
                    {
                        error = ex.Message;
                    }
                }
                else
                {
                    error = $"Bot {bot.Name} not found!";
                }
            }
            else
            {
                error = $"Botname is empty!";
            }

            if (cancellationToken.IsCancellationRequested)
            {
                error = "Operation cancelled!";
            }

            if (farmerBot.IsError)
            {
                error = farmerBot.ErrorMessage;
            }

            if (error != "")
            {
                //TODO: Send it to Logservice
                output = error;
            }

            ServiceResponse<string> response = new ServiceResponse<string>
            {
                Data = output,
                Message = error,
                Success = string.IsNullOrWhiteSpace(error)
            };

            return response;
        }
        #endregion FarmerBot

        public void Dispose()
        {
            _appSettings.OnAppSettingsChanged -= UpdateAppSettings;
        }
    }
}