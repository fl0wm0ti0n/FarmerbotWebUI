using FarmerbotWebUI.Shared;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using static System.Net.WebRequestMethods;

namespace FarmerbotWebUI.Client.Services.Docker
{
    public class DockerService : IDockerService
    {
        private int cancelTimeout = 300000;
        private readonly HttpClient _httpClient;
        public Dictionary<ActionType, CancellationTokenSource> _cancellationTokens = new Dictionary<ActionType, CancellationTokenSource>();

        public DockerService(HttpClient httpClient)
        {
            _httpClient = httpClient; 
        }

        public Task<bool> CancelOperation(ActionType command)
        {
            try
            {
                _cancellationTokens.FirstOrDefault(c => c.Key == command).Value.Cancel();
                return Task.FromResult(true);
            }
            catch (Exception)
            {
                return Task.FromResult(false);
            }
        }

        public async Task<ServiceResponse<string>> StartComposeAsync()
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(cancelTimeout);
            _cancellationTokens.Add(ActionType.DockerStart, cancellationTokenSource);
            var response = await _httpClient.GetFromJsonAsync<ServiceResponse<string>>("api/docker/start", cancellationTokenSource.Token);
            return response;
        }

        public async Task<ServiceResponse<string>> StopComposeAsync()
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(cancelTimeout);
            _cancellationTokens.Add(ActionType.DockerStop, cancellationTokenSource);
            var response = await _httpClient.GetFromJsonAsync<ServiceResponse<string>>("api/docker/stop", cancellationTokenSource.Token);
            return response;
        }

        public async Task<ServiceResponse<string>> GetComposeListAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<string>> GetComposeLogsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<string>> GetComposeProcessesAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<FarmerBotStatus>> GetComposeStatusAsync()
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(cancelTimeout);
            _cancellationTokens.Add(ActionType.DockerStatus, cancellationTokenSource);
            var response = await _httpClient.GetFromJsonAsync<ServiceResponse<FarmerBotStatus>> ("api/docker/status", cancellationTokenSource.Token);
            return response;
        }
    }
}