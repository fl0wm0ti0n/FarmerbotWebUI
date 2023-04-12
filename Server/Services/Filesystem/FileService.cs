using Docker.DotNet;
using Docker.DotNet.Models;
using FarmerbotWebUI.Shared;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace FarmerbotWebUI.Server.Services.Filesystem
{
    public class FileService : IFileService
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

        public FileService(IConfiguration configuration, CancellationToken cancellationToken)
        {
            _config = configuration;
            _workingDirectory = _config.GetValue<string>("FarmerBotSettings:WorkingDirectory");
            _dockerComposeFile = _config.GetValue<string>("FarmerBotSettings:ComposeFile");
            _threefoldConfigFile = _config.GetValue<string>("FarmerBotSettings:ThreefoldNetworkFile");
            _farmerBotConfigFile = _config.GetValue<string>("FarmerBotSettings:FarmerBotConfigFile");
            _farmerBotLogFile = _config.GetValue<string>("FarmerBotSettings:FarmerBotLogFile");
        }

        public async Task<ServiceResponse<string>> GetLocalLogAsync(string path, CancellationToken cancellationToken)
        {
            // If path is empty use workingdir + farmerbot.log
            if (path == string.Empty || string.IsNullOrWhiteSpace(path))
            {
                path = $"{_workingDirectory}{Path.DirectorySeparatorChar}{_farmerBotLogFile}";
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

        public async Task<ServiceResponse<string>> GetComposeFileAsync(string path, CancellationToken cancellationToken)
        {
            // If path is empty use workingdir + docker-compose.yaml
            if (path == string.Empty || string.IsNullOrWhiteSpace(path))
            {
                path = $"{_workingDirectory}{Path.DirectorySeparatorChar}{_dockerComposeFile}";
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

        public async Task<ServiceResponse<string>> SetComposeFileAsync(string compose, string path, CancellationToken cancellationToken)
        {
            // If path is empty use workingdir + docker-compose.yaml
            if (path == string.Empty || string.IsNullOrWhiteSpace(path))
            {
                path = $"{_workingDirectory}{Path.DirectorySeparatorChar}{_dockerComposeFile}";
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

        public async Task<ServiceResponse<FarmerBotConfig>> GetMarkdownConfigAsync(string path, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<string>> GetRawMarkdownConfigAsync(string path, CancellationToken cancellationToken)
        {
            // If path is empty use workingdir + config.md
            if (path == string.Empty || string.IsNullOrWhiteSpace(path))
            {
                path = $"{_workingDirectory}{Path.DirectorySeparatorChar}{_dockerConfig}";
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

    public async Task<ServiceResponse<string>> SetMarkdownConfigAsync(FarmerBotConfig config, string path, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<string>> SetMarkdownConfigAsync(string config, string path, CancellationToken cancellationToken)
        {
            // If path is empty use workingdir + config.md
            if (path == string.Empty || string.IsNullOrWhiteSpace(path))
            {
                path = $"{_workingDirectory}{Path.DirectorySeparatorChar}{_dockerConfig}";
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

        public Task<ServiceResponse<string>> GetRawComposeFileAsync(string path, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<ServiceResponse<FarmerBotServices>> IFileService.GetComposeFileAsync(string path, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<string>> SetComposeFileAsync(FarmerBotServices compose, string path, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<string>> GetEnvFileAsync(string path, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<string>> SetEnvFileAsync(string env, string path, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}