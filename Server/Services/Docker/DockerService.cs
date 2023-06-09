﻿using Docker.DotNet;
using Docker.DotNet.Models;
using FarmerbotWebUI.Shared.BotConfig;
using FarmerBotWebUI.Shared;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace FarmerbotWebUI.Server.Services.Docker
{
    public class DockerService : IDockerService
    {
        private IAppSettings _appSettings;
        private readonly IFileService _fileService;
        public List<FarmerBotStatus> ActualFarmerBotStatus { get; private set; } = new List<FarmerBotStatus>();

        public DockerService(IAppSettings appSettings, IFileService fileService)
        {
            _appSettings = appSettings;
            _fileService = fileService;
            _appSettings.OnAppSettingsChanged += UpdateAppSettings;
        }

        private void UpdateAppSettings(object sender, AppSettings newAppSettings)
        {
            _appSettings = newAppSettings;
        }

        private string GetDockerPipe()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return _appSettings.DockerSettings.DockerEndpointWindows;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return _appSettings.DockerSettings.DockerEndpointLinux;
            }
            else
            {
                throw new PlatformNotSupportedException();
            }
        }

        public async Task<ServiceResponse<FarmerBotStatus>> StartComposeAsync(string botName, CancellationToken cancellationToken)
        {
            var processStartInfo = new ProcessStartInfo("docker", _appSettings.DockerSettings.DockerRunCommand)
            {
                WorkingDirectory = _appSettings.FarmerBotSettings.Bots.FirstOrDefault(b => b.BotName == botName).WorkingDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            string error = "";
            string output = "";
            int exitCode = 0;
            try
            {
                using var process = Process.Start(processStartInfo);
                await process.WaitForExitAsync(cancellationToken);

                output = await process.StandardOutput.ReadToEndAsync();
                error = await process.StandardError.ReadToEndAsync();
                exitCode = process.ExitCode;
                if (output == "" && error == "")
                {
                    exitCode = 1;
                    error = "No exitcode, maybe compose is already down";
                }
            }
            catch (Exception ex)
            {
                exitCode = 1;
                error = ex.Message;
            }

            var result = string.IsNullOrWhiteSpace(output) ? error : output;

            if (cancellationToken.IsCancellationRequested)
            {
                error = "Canceled";
                exitCode = 1;
            }

            return new ServiceResponse<FarmerBotStatus>
            {
                Data = GetComposeStatusAsync(botName, cancellationToken).Result.Data,
                Message = error,
                Success = exitCode > 0 ? false : true
            };
        }

        public async Task<ServiceResponse<FarmerBotStatus>> StopComposeAsync(string botName, CancellationToken cancellationToken)
        {
            var processStartInfo = new ProcessStartInfo("docker", _appSettings.DockerSettings.DockerDownCommand)
            {
                WorkingDirectory = _appSettings.FarmerBotSettings.Bots.FirstOrDefault(b => b.BotName == botName).WorkingDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            string error = "";
            string output = "";
            int exitCode = 0;
            try
            {
                using var process = Process.Start(processStartInfo);
                await process.WaitForExitAsync(cancellationToken);

                output = await process.StandardOutput.ReadToEndAsync();
                error = await process.StandardError.ReadToEndAsync();
                exitCode = process.ExitCode;
                if (output == "" && error == "")
                {
                    exitCode = 1;
                    error = "No exitcode, maybe compose is already down";
                }

            }
            catch (Exception ex)
            {
                exitCode = 1;
                error = ex.Message;
            }

            var result = string.IsNullOrWhiteSpace(output) ? error : output;

            if (cancellationToken.IsCancellationRequested)
            {
                error = "Canceled";
                exitCode = 1;
            }

            return new ServiceResponse<FarmerBotStatus>
            {
                Data = GetComposeStatusAsync(botName, cancellationToken).Result.Data,
                Message = error,
                Success = exitCode > 0 ? false : true
            };
        }

        public async Task<ServiceResponse<FarmerBotStatus>> GetComposeStatusAsync(string botName, CancellationToken cancellationToken)
        {
            string error = "";
            string output = "";
            int exitCode = 0;
            DockerClient? client = null;

            try
            {
                var endpoint = GetDockerPipe(); // Endpoint for the connection to the Docker-Engine
                var dockerConfig = new DockerClientConfiguration(new Uri(endpoint)); // config of the Docker-Clientbibliothek
                client = dockerConfig.CreateClient(); // creation of the Docker-Client-Objekt
            }
            catch (Exception ex)
            {
                exitCode = 1;
                error = ex.Message;
            }

            string fullPath = Path.GetFullPath(_appSettings.FarmerBotSettings.Bots.FirstOrDefault(b => b.BotName == botName).WorkingDirectory).TrimEnd(Path.DirectorySeparatorChar);
            string lastDir = fullPath.Split(Path.DirectorySeparatorChar).Last().ToLower();

            FarmerBotStatus actualStatus = ActualFarmerBotStatus.FirstOrDefault(s => s.Name == botName);
            if (actualStatus == null)
            {
                actualStatus = new FarmerBotStatus()
                {
                    Name = botName
                };
            }
            else
            {
                ActualFarmerBotStatus.Remove(actualStatus);
            }

            try
            {
                var containers = await client.Containers.ListContainersAsync(new ContainersListParameters { All = true }, cancellationToken);
                foreach (var container in _appSettings.FarmerBotSettings.Bots.FirstOrDefault(b => b.BotName == botName).ContainerNames) // TODO: Get ContainerNames from compose.yaml
                {
                    var containerResponse = containers.FirstOrDefault(c => c.Names.Contains($"/{lastDir}-{container}-1"));

                    if (containerResponse == null)
                    {
                        actualStatus.Containers.Add(new ContainerListObject()
                        {
                            Container = containerResponse,
                            Name = container,
                            NoContainer = true,
                            Running = false
                        });
                        exitCode = 1;
                        error += $"Container {container} not found.\n";
                    }
                    else
                    {
                        if (containerResponse.State == "running")
                        {
                            actualStatus.Containers.Add(new ContainerListObject()
                            {
                                Container = containerResponse,
                                Name = container,
                                NoContainer = false,
                                Running = true,
                            });

                            exitCode = 0;
                        }
                        else
                        {
                            actualStatus.Containers.Add(new ContainerListObject()
                            {
                                Container = containerResponse,
                                Name = container,
                                NoContainer = false,
                                Running = false
                            });
                            exitCode = 0;
                        }
                        error = "Sucessfully updated FarmerBot status";
                    }
                }
            }
            catch (Exception ex)
            {
                exitCode = 1;
                error = ex.Message;
                actualStatus.NoStatus = true;
            }

            // try to get also all BotInfos away from Docker
            var bot = await _fileService.GetFarmerBotAsync(botName, cancellationToken);
            if (bot.Data != null)
            {
                if (bot.Data.IsError)
                {
                    error = $"{error}\n{bot.Message}";
                    //exitCode = 1;
                    //actualStatus.NoStatus = true;
                }

                actualStatus.EnvOk = !bot.Data.Env.IsError;
                actualStatus.EnvError = bot.Data.Env.ErrorMessage;
                actualStatus.ConfigOk = !bot.Data.FarmerBotConfig.IsError;
                actualStatus.ConfigError = bot.Data.FarmerBotConfig.ErrorMessage;
                actualStatus.ComposeOk = !bot.Data.DockerCompose.IsError;
                actualStatus.ComposeError = bot.Data.DockerCompose.ErrorMessage;
                if (actualStatus.ConfigOk)
                {
                    actualStatus.BotDefinitionInfos.WakeUpThreshold = bot.Data.FarmerBotConfig.PowerDefinition.WakeUpThreshold;
                    actualStatus.BotDefinitionInfos.PeriodicWakeup = bot.Data.FarmerBotConfig.PowerDefinition.PeriodicWakeup;
                    actualStatus.BotDefinitionInfos.PublicIps = bot.Data.FarmerBotConfig.FarmDefinition.PublicIps;
                    actualStatus.BotDefinitionInfos.Id = bot.Data.FarmerBotConfig.FarmDefinition.Id;
                }
            }
            else
            {
                error = "Bot not found in _fileService.GetFarmerBotAsync(botName, cancellationToken);";
                exitCode = 1;
                actualStatus.NoStatus = true;
            }

            if (cancellationToken.IsCancellationRequested)
            {
                error = "Canceled";
                exitCode = 1;
                actualStatus.NoStatus = true;
            }
            actualStatus.LastUpdate = DateTime.UtcNow;

            ActualFarmerBotStatus.Add(actualStatus);

            return new ServiceResponse<FarmerBotStatus>
            {
                Data = actualStatus,
                Message = error,
                Success = exitCode > 0 ? false : true
            };
        }

        public async Task<ServiceResponse<string>> GetComposeListAsync(string botName, CancellationToken cancellationToken)
        {
            var processStartInfo = new ProcessStartInfo("docker", "compose ls")
            {
                WorkingDirectory = _appSettings.FarmerBotSettings.Bots.FirstOrDefault(b => b.BotName == botName).WorkingDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            string error = "";
            string output = "";
            int exitCode = 0;
            try
            {
                using var process = Process.Start(processStartInfo);
                await process.WaitForExitAsync(cancellationToken);

                output = await process.StandardOutput.ReadToEndAsync();
                error = await process.StandardError.ReadToEndAsync();
                exitCode = process.ExitCode;
            }
            catch (Exception ex)
            {
                exitCode = 1;
                error = ex.Message;
            }

            var result = string.IsNullOrWhiteSpace(output) ? error : output;

            if (cancellationToken.IsCancellationRequested)
            {
                error = "Canceled";
                exitCode = 1;
            }

            return new ServiceResponse<string>
            {
                Data = output,
                Message = error,
                Success = exitCode > 0 ? false : true
            };
        }

        public async Task<ServiceResponse<string>> GetComposeProcessesAsync(string botName, CancellationToken cancellationToken)
        {
            var processStartInfo = new ProcessStartInfo("docker", "compose ps")
            {
                WorkingDirectory = _appSettings.FarmerBotSettings.Bots.FirstOrDefault(b => b.BotName == botName).WorkingDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            string error = "";
            string output = "";
            int exitCode = 0;
            try
            {
                using var process = Process.Start(processStartInfo);
                await process.WaitForExitAsync(cancellationToken);

                output = await process.StandardOutput.ReadToEndAsync();
                error = await process.StandardError.ReadToEndAsync();
            }
            catch (Exception ex)
            {
                exitCode = 1;
                error = ex.Message;
            }

            var result = string.IsNullOrWhiteSpace(output) ? error : output;

            if (cancellationToken.IsCancellationRequested)
            {
                error = "Canceled";
                exitCode = 1;
            }

            return new ServiceResponse<string>
            {
                Data = output,
                Message = error,
                Success = string.IsNullOrWhiteSpace(error)
            };
        }

        public async Task<ServiceResponse<string>> GetComposeLogsAsync(string botName, CancellationToken cancellationToken)
        {
            var processStartInfo = new ProcessStartInfo("docker", _appSettings.DockerSettings.DockerLogCommand)
            {
                WorkingDirectory = _appSettings.FarmerBotSettings.Bots.FirstOrDefault(b => b.BotName == botName).WorkingDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            string error = "";
            string output = "";
            int exitCode = 0;
            try
            {
                using var process = Process.Start(processStartInfo);
                await process.WaitForExitAsync(cancellationToken);

                output = await process.StandardOutput.ReadToEndAsync();
                error = await process.StandardError.ReadToEndAsync();
            }
            catch (Exception ex)
            {
                exitCode = 1;
                error = ex.Message;
            }

            var result = string.IsNullOrWhiteSpace(output) ? error : output;

            if (cancellationToken.IsCancellationRequested)
            {
                error = "Canceled";
                exitCode = 1;
            }

            return new ServiceResponse<string>
            {
                Data = output,
                Message = error,
                Success = string.IsNullOrWhiteSpace(error)
            };
        }

        public async Task<ServiceResponse<List<FarmerBotStatus>>> GetComposeStatusListAsync(CancellationToken cancellationToken)
        {
            List<FarmerBotStatus> stati = new List<FarmerBotStatus>();
            string error = "";

            int i = 0;
            foreach (var bot in _appSettings.FarmerBotSettings.Bots)
            {
                i++;
                if (ActualFarmerBotStatus.Count > i && !ActualFarmerBotStatus.Contains(ActualFarmerBotStatus.Find(s => s.Name == bot.BotName)))
                {
                    //TODO: Settings unsync Error 
                }
                else
                {
                    var status = await GetComposeStatusAsync(bot.BotName, cancellationToken);
                    if (!status.Success || !status.Data.Status()) 
                    {
                        error += $"FarmerBot {status.Data.Name} error: \n";
                        error += $"{status.Message}\n";
                    }
                    stati.Add(status.Data);
                }
            }

            return new ServiceResponse<List<FarmerBotStatus>>
            {
                Data = stati,
                Message = error,
                Success = string.IsNullOrWhiteSpace(error)
            };
        }

        public void Dispose()
        {
            _appSettings.OnAppSettingsChanged -= UpdateAppSettings;
        }

    }
}