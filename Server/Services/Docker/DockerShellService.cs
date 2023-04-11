using FarmerbotWebUI.Shared;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace FarmerbotWebUI.Server.Services.Docker
{
    public class DockerShellService
    {
        private readonly string _workingDirectory;
        public FarmerBotStatus ActualFarmerBotStatus { get; private set; }

        public DockerShellService(string workingDirectory)
        {
            _workingDirectory = workingDirectory;
        }

        public async Task<ServiceResponse<string>> StartComposeAsync()
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
                await process.WaitForExitAsync();

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

            ServiceResponse<string> response = new ServiceResponse<string>
            {
                Data = result,
                Message = error,
                Success = exitCode > 0 ? false : true
            };
            return response;
        }

        public async Task<ServiceResponse<string>> StopComposeAsync()
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
                await process.WaitForExitAsync();
                output = await process.StandardOutput.ReadToEndAsync();
                error = await process.StandardError.ReadToEndAsync();
                process.WaitForExit();
                exitCode = process.ExitCode;
            }
            catch (Exception ex)
            {
                exitCode = 1;
                error = ex.Message;
            }

            var result = string.IsNullOrWhiteSpace(output) ? error : output;

            ServiceResponse<string> response = new ServiceResponse<string>
            {
                Data = result,
                Message = error,
                Success = exitCode > 0 ? false : true
            };
            return response;
        }

        public async Task<ServiceResponse<string>> GetComposeListAsync()
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
                await process.WaitForExitAsync();

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

            ServiceResponse<string> response = new ServiceResponse<string>
            {
                Data = output,
                Message = error,
                Success = exitCode > 0 ? false : true
            };

            return response;
        }

        public async Task<ServiceResponse<string>> GetComposeProcessesAsync()
        {
            var processStartInfo = new ProcessStartInfo("docker", "compose ps")
            {
                WorkingDirectory = _workingDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            string error = "";
            string output = "";
            try
            {
                using var process = Process.Start(processStartInfo);
                await process.WaitForExitAsync();

                output = await process.StandardOutput.ReadToEndAsync();
                error = await process.StandardError.ReadToEndAsync();
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            var result = string.IsNullOrWhiteSpace(output) ? error : output;

            ServiceResponse<string> response = new ServiceResponse<string>
            {
                Data = output,
                Message = error,
                Success = string.IsNullOrWhiteSpace(error)
            };

            return response;
        }

        public async Task<ServiceResponse<string>> GetComposeLogsAsync()
        {
            var processStartInfo = new ProcessStartInfo("docker", "compose logs -f")
            {
                WorkingDirectory = _workingDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            string error = "";
            string output = "";
            try
            {
                using var process = Process.Start(processStartInfo);
                await process.WaitForExitAsync();

                output = await process.StandardOutput.ReadToEndAsync();
                error = await process.StandardError.ReadToEndAsync();
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            var result = string.IsNullOrWhiteSpace(output) ? error : output;

            ServiceResponse<string> response = new ServiceResponse<string>
            {
                Data = output,
                Message = error,
                Success = string.IsNullOrWhiteSpace(error)
            };

            return response;
        }

        public async Task<ServiceResponse<string>> GetLocalLogAsync(string path)
        {
            // If path is empty use workingdir + farmerbot.log
            if (path == string.Empty || string.IsNullOrWhiteSpace(path))
            {
                path = $"{_workingDirectory}{Path.DirectorySeparatorChar}config{Path.DirectorySeparatorChar}farmerbot.log";
            }

            string error = "";
            string output = "";
            try
            {
                // read file
                output = await File.ReadAllTextAsync(path);
            }
            catch (Exception ex)
            {
                // return error
                error = ex.Message;
            }

            ServiceResponse<string> response = new ServiceResponse<string>
            {
                Data = output,
                Message = error,
                Success = string.IsNullOrWhiteSpace(error)
            };

            return response;
        }

        public async Task<ServiceResponse<string>> GetComposeFileAsync(string path)
        {
            // If path is empty use workingdir + docker-compose.yaml
            if (path == string.Empty || string.IsNullOrWhiteSpace(path))
            {
                path = $"{_workingDirectory}{Path.DirectorySeparatorChar}docker-compose.yaml";
            }

            string error = "";
            string output = "";
            try
            {
                // read file
                output = await File.ReadAllTextAsync(path);
            }
            catch (Exception ex)
            {
                // return error
                error = ex.Message;
            }

            ServiceResponse<string> response = new ServiceResponse<string>
            {
                Data = output,
                Message = error,
                Success = string.IsNullOrWhiteSpace(error)
            };

            return response;
        }

        public async Task<ServiceResponse<string>> SetComposeFileAsync(string compose, string path)
        {
            // If path is empty use workingdir + docker-compose.yaml
            if (path == string.Empty || string.IsNullOrWhiteSpace(path))
            {
                path = $"{_workingDirectory}{Path.DirectorySeparatorChar}docker-compose.yaml";
            }

            string error = "";
            string output = "";
            try
            {
                // read file
                await File.WriteAllTextAsync(path, compose);
            }
            catch (Exception ex)
            {
                // return error
                error = ex.Message;
            }

            ServiceResponse<string> response = new ServiceResponse<string>
            {
                Data = output,
                Message = error,
                Success = string.IsNullOrWhiteSpace(error)
            };

            return response;
        }

        public async Task<ServiceResponse<FarmerBotConfig>> GetMarkdownConfigAsync(string path)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<string>> GetRawMarkdownConfigAsync(string path)
        {
            // If path is empty use workingdir + config.md
            if (path == string.Empty || string.IsNullOrWhiteSpace(path))
            {
                path = $"{_workingDirectory}{Path.DirectorySeparatorChar}config{Path.DirectorySeparatorChar}config.md";
            }

            string error = "";
            string output = "";
            try
            {
                // read file
                output = await File.ReadAllTextAsync(path);
            }
            catch (Exception ex)
            {
                // return error
                error = ex.Message;
            }

            ServiceResponse<string> response = new ServiceResponse<string>
            {
                Data = output,
                Message = error,
                Success = string.IsNullOrWhiteSpace(error)
            };

            return response;
        }

    public async Task<ServiceResponse<string>> SetMarkdownConfigAsync(FarmerBotConfig config, string path)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<string>> SetMarkdownConfigAsync(string config, string path)
        {
            // If path is empty use workingdir + config.md
            if (path == string.Empty || string.IsNullOrWhiteSpace(path))
            {
                path = $"{_workingDirectory}{Path.DirectorySeparatorChar}config{Path.DirectorySeparatorChar}config.md";
            }

            string error = "";
            string output = "";
            try
            {
                // read file
                await File.WriteAllTextAsync(path, config);
            }
            catch (Exception ex)
            {
                // return error
                error = ex.Message;
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