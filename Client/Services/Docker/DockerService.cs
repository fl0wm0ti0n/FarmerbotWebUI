using FarmerbotWebUI.Shared;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using static System.Net.WebRequestMethods;

namespace FarmerbotWebUI.Client.Services.Docker
{
    public class DockerService : IDockerService
    {

        private readonly HttpClient _httpClient;

        public DockerService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ServiceResponse<string>> GetComposeFileAsync(string path)
        {
            throw new NotImplementedException();
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

        public async Task<ServiceResponse<string>> GetLocalLogAsync(string path)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<FarmerBotConfig>> GetMarkdownConfigAsync(string path)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<string>> GetRawMarkdownConfigAsync(string path)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<string>> SetComposeFileAsync(string compose, string path)
        {
            var content = new StringContent(compose, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"api/compose/set/{path}", content);

            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ServiceResponse<string>>(responseContent);

            return result;
        }

        public async Task<ServiceResponse<string>> SetMarkdownConfigAsync(FarmerBotConfig config, string path)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<string>> SetMarkdownConfigAsync(string config, string path)
        {
            var content = new StringContent(config, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"api/markdown/set/{path}", content);

            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ServiceResponse<string>>(responseContent);
            
            return result;
        }

        public async Task<ServiceResponse<string>> StartComposeAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<ServiceResponse<string>>("api/docker/start");
            return response;
        }

        public async Task<ServiceResponse<string>> StopComposeAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<ServiceResponse<string>>("api/docker/stop");
            return response;
        }
    }
}