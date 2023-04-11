using Docker.DotNet;
using Docker.DotNet.Models;
using FarmerbotWebUI.Client.Services.Docker;
using FarmerbotWebUI.Shared;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
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
        private readonly DockerClientConfiguration _dockerConfig;
        private readonly string _endpoint;
        private readonly DockerClient _client;
        public FarmerBotStatus ActualFarmerBotStatus { get; private set; }

        public DockerService(IConfiguration configuration)
        {
            _config = configuration;
            _workingDirectory = _config.GetValue<string>("FarmerBotSettings:WorkingDirectory");
            _dockerComposeFile = _config.GetValue<string>("FarmerBotSettings:ComposeFile");
            _threefoldConfigFile = _config.GetValue<string>("FarmerBotSettings:ThreefoldNetworkFile");
            _farmerBotConfigFile = _config.GetValue<string>("FarmerBotSettings:FarmerBotConfigFile");
            _farmerBotLogFile = _config.GetValue<string>("FarmerBotSettings:FarmerBotLogFile");
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

        public async Task<ServiceResponse<string>> StartComposeAsync(CancellationToken cancellationToken)
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
                Data = result,
                Message = error,
                Success = exitCode > 0 ? false : true
            };
            return response;
        }

        public async Task<ServiceResponse<string>> StopComposeAsync(CancellationToken cancellationToken)
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
                Data = result,
                Message = error,
                Success = exitCode > 0 ? false : true
            };
            return response;
        }

        public async Task<ServiceResponse<FarmerBotStatus>> GetComposeStatusAsync(CancellationToken cancellationToken)
        {
            var endpoint = GetDockerPipe(); // Endpoint für die Verbindung zur Docker-Engine
            var dockerConfig = new DockerClientConfiguration(new Uri(_endpoint)); // Konfiguration für die Docker-Clientbibliothek
            var client = _dockerConfig.CreateClient(); // Docker-Client-Objekt erstellen

            var containerName = "mein-container"; // Name des Containers, dessen Status abgefragt werden soll

            var containers = await _client.Containers.ListContainersAsync(new ContainersListParameters { All = true }); // Alle Container abrufen

            var container = containers.FirstOrDefault(c => c.Names.Contains($"/{containerName}")); // Den Container anhand des Namens finden

            if (container == null)
            {
                Console.WriteLine($"Container {containerName} nicht gefunden.");
            }
            else
            {
                var containerStatus = container.State; // Container-Status abrufen

                Console.WriteLine($"Container-Status von {containerName}: {containerStatus}");
            }


            string error = "";
            string output = "";
            int exitCode = 0;
            try
            {

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
                Data = result,
                Message = error,
                Success = exitCode > 0 ? false : true
            };
            throw new NotImplementedException();
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