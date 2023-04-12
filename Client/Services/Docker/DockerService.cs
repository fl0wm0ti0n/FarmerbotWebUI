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
        private readonly int cancelTimeout = 300000; // TODO: get from config
        private readonly HttpClient _httpClient;
        private readonly int _farmerBotStatusInterval = 60; // TODO: get from config
        public Dictionary<ActionType, CancellationTokenSource> CancellationTokens = new Dictionary<ActionType, CancellationTokenSource>();
        public FarmerBotStatus ActualFarmerBotStatus { get; private set; }
        private SemaphoreSlim _statusSemaphore = new SemaphoreSlim(1);

        public DockerService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            StartStatusInterval();
        }

        private void StartStatusInterval()
        {
            Timer timer = new Timer(async (e) =>
            {   
                // Versuche, die SemaphoreSlim zu betreten. Wenn sie bereits gesperrt ist, wird hier gewartet, bis sie freigegeben wird.
                await _statusSemaphore.WaitAsync();
                try
                {
                    // Hier wird der eigentliche Code ausgeführt, während der Timer pausiert wird
                    var status = await GetComposeStatusAsync();
                    if (status.Success && status.Data != null)
                    {
                        ActualFarmerBotStatus = status.Data;
                    }
                }
                finally
                {
                    // Gib die SemaphoreSlim wieder frei, um den Timer fortzusetzen
                    _statusSemaphore.Release();
                }
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(_farmerBotStatusInterval));
        }

        public Task<bool> CancelOperation(ActionType command)
        {
            try
            {
                CancellationTokens.FirstOrDefault(c => c.Key == command).Value.Cancel();
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
            CancellationTokens.Add(ActionType.DockerStart, cancellationTokenSource);
            var response = await _httpClient.GetFromJsonAsync<ServiceResponse<string>>("api/docker/start", cancellationTokenSource.Token);
            return response;
        }

        public async Task<ServiceResponse<string>> StopComposeAsync()
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(cancelTimeout);
            CancellationTokens.Add(ActionType.DockerStop, cancellationTokenSource);
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
            CancellationTokens.Add(ActionType.DockerStatus, cancellationTokenSource);
            var response = await _httpClient.GetFromJsonAsync<ServiceResponse<FarmerBotStatus>> ("api/docker/status", cancellationTokenSource.Token);
            return response;
        }
    }
}