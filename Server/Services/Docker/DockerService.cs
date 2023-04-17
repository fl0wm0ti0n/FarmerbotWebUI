using Docker.DotNet;
using Docker.DotNet.Models;
using FarmerbotWebUI.Client.Services.Docker;
using FarmerbotWebUI.Shared;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace FarmerbotWebUI.Server.Services.Docker
{
    public class DockerService : IDockerService
    {
        private readonly IConfiguration _config;
        private readonly string _workingDirectory;
        private readonly string _dockerComposeFile;
        private readonly string _threefoldConfigFile;
        private readonly string _farmerBotConfigFile;
        private readonly string _farmerBotLogFile;
        private readonly int _farmerBotStatusInterval;
        private List<string> _containerNames = new List<string>() { "farmerbot", "redis", "rmbpeer", "grid3_client" }; // TODO: Get from config

        public FarmerBotStatus ActualFarmerBotStatus { get; private set; } = new FarmerBotStatus { NoStatus = false };
        public FarmerBotServices FarmerBotServices { get; private set; } = new FarmerBotServices();

        public DockerService(IConfiguration configuration)
        {
            _config = configuration;
            _workingDirectory = _config.GetValue<string>("FarmerBotSettings:WorkingDirectory");
            _dockerComposeFile = _config.GetValue<string>("FarmerBotSettings:ComposeFile");
            _threefoldConfigFile = _config.GetValue<string>("FarmerBotSettings:ThreefoldNetworkFile");
            _farmerBotConfigFile = _config.GetValue<string>("FarmerBotSettings:FarmerBotConfigFile");
            _farmerBotLogFile = _config.GetValue<string>("FarmerBotSettings:FarmerBotLogFile");
            _farmerBotStatusInterval = _config.GetValue<int>("FarmerBotSettings:FarmerBotStatusInterval");
        }

        private string GetDockerPipe()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return _config.GetValue<string>("DockerSettings:DockerEndpointWindows");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return _config.GetValue<string>("DockerSettings:DockerEndpointLinux");
            }
            else
            {
                throw new PlatformNotSupportedException();
            }
        }

        public async Task<ServiceResponse<FarmerBotStatus>> StartComposeAsync(CancellationToken cancellationToken)
        {
            var processStartInfo = new ProcessStartInfo("docker", "compose up -d")
            {
                WorkingDirectory = _workingDirectory,
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
                Data = GetComposeStatusAsync(cancellationToken).Result.Data,
                Message = error,
                Success = exitCode > 0 ? false : true
            };
            return response;
        }

        public async Task<ServiceResponse<FarmerBotStatus>> StopComposeAsync(CancellationToken cancellationToken)
        {
            var processStartInfo = new ProcessStartInfo("docker", "compose down")
            {
                WorkingDirectory = _workingDirectory,
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
                Data = GetComposeStatusAsync(cancellationToken).Result.Data,
                Message = error,
                Success = exitCode > 0 ? false : true
            };
            return response;
        }

        public async Task<ServiceResponse<FarmerBotStatus>> GetComposeStatusAsync(CancellationToken cancellationToken)
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

            string fullPath = Path.GetFullPath(_workingDirectory).TrimEnd(Path.DirectorySeparatorChar);
            string lastDir = fullPath.Split(Path.DirectorySeparatorChar).Last().ToLower();

            try
            {
                var containers = await client.Containers.ListContainersAsync(new ContainersListParameters { All = true }, cancellationToken);
                foreach (var container in _containerNames)
                {
                    var containerResponse = containers.FirstOrDefault(c => c.Names.Contains($"/{lastDir}-{container}-1"));

                    if (containerResponse == null)
                    {
                        ActualFarmerBotStatus.Containers.Add(new ContainerListObject()
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
                            ActualFarmerBotStatus.Containers.Add(new ContainerListObject()
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
                            ActualFarmerBotStatus.Containers.Add(new ContainerListObject()
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
                ActualFarmerBotStatus.NoStatus = true;
            }

            if (ActualFarmerBotStatus.Status())
            {
                ActualFarmerBotStatus.ComposeOk = true;
                ActualFarmerBotStatus.EnvOk = true;
                ActualFarmerBotStatus.ConfigOk = true;
            }
            ActualFarmerBotStatus.LastUpdate = DateTime.UtcNow;

            if (cancellationToken.IsCancellationRequested)
            {
                error = "Canceled";
                exitCode = 1;
                ActualFarmerBotStatus.NoStatus = true;
            }
            ServiceResponse<FarmerBotStatus> response = new ServiceResponse<FarmerBotStatus>
            {
                Data = ActualFarmerBotStatus,
                Message = error,
                Success = exitCode > 0 ? false : true
            };
            return response;
        }

        public async Task<ServiceResponse<string>> GetComposeListAsync(CancellationToken cancellationToken)
        {
            var processStartInfo = new ProcessStartInfo("docker", "compose ls")
            {
                WorkingDirectory = _workingDirectory,
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

        public async Task<ServiceResponse<string>> GetComposeProcessesAsync(CancellationToken cancellationToken)
        {
            var processStartInfo = new ProcessStartInfo("docker", "compose ps")
            {
                WorkingDirectory = _workingDirectory,
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

        public async Task<ServiceResponse<string>> GetComposeLogsAsync(CancellationToken cancellationToken)
        {
            var processStartInfo = new ProcessStartInfo("docker", "compose logs -f")
            {
                WorkingDirectory = _workingDirectory,
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
    }
}