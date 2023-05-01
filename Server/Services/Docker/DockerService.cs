using Docker.DotNet;
using Docker.DotNet.Models;
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
        //public FarmerBotServices FarmerBotServices { get; private set; } = new FarmerBotServices();

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

            ServiceResponse<FarmerBotStatus> response = new ServiceResponse<FarmerBotStatus>
            {
                Data = GetComposeStatusAsync(botName, cancellationToken).Result.Data,
                Message = error,
                Success = exitCode > 0 ? false : true
            };
            return response;
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

            ServiceResponse<FarmerBotStatus> response = new ServiceResponse<FarmerBotStatus>
            {
                Data = GetComposeStatusAsync(botName, cancellationToken).Result.Data,
                Message = error,
                Success = exitCode > 0 ? false : true
            };
            return response;
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

            if (actualStatus.Status())
            {
                actualStatus.ComposeOk = true;
                actualStatus.EnvOk = true;
                actualStatus.ConfigOk = true;
            }
            actualStatus.LastUpdate = DateTime.UtcNow;

            if (cancellationToken.IsCancellationRequested)
            {
                error = "Canceled";
                exitCode = 1;
                actualStatus.NoStatus = true;
            }

            ActualFarmerBotStatus.Add(actualStatus);

            ServiceResponse<FarmerBotStatus> response = new ServiceResponse<FarmerBotStatus>
            {
                Data = actualStatus,
                Message = error,
                Success = exitCode > 0 ? false : true
            };
            return response;
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

            ServiceResponse<string> response = new ServiceResponse<string>
            {
                Data = output,
                Message = error,
                Success = exitCode > 0 ? false : true
            };

            return response;
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

            ServiceResponse<string> response = new ServiceResponse<string>
            {
                Data = output,
                Message = error,
                Success = string.IsNullOrWhiteSpace(error)
            };

            return response;
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

            ServiceResponse<string> response = new ServiceResponse<string>
            {
                Data = output,
                Message = error,
                Success = string.IsNullOrWhiteSpace(error)
            };

            return response;
        }

        public async Task<ServiceResponse<List<FarmerBotStatus>>> GetComposeStatusListAsync(CancellationToken cancellationToken)
        {
            List<FarmerBotStatus> stati = new List<FarmerBotStatus>();
            string error = "";

            foreach (var bot in _appSettings.FarmerBotSettings.Bots)
            {
                if (!ActualFarmerBotStatus.Contains(ActualFarmerBotStatus.Find(s => s.Name == bot.BotName)))
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

            ServiceResponse<List<FarmerBotStatus>> response = new ServiceResponse<List<FarmerBotStatus>>
            {
                Data = stati,
                Message = error,
                Success = string.IsNullOrWhiteSpace(error)
            };

            return response;
        }

        public void Dispose()
        {
            _appSettings.OnAppSettingsChanged -= UpdateAppSettings;
        }

    }
}