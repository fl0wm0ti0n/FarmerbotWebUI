using FarmerbotWebUI.Shared;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using static System.Net.WebRequestMethods;

namespace FarmerbotWebUI.Client.Services.Filesystem
{
    public class FilesService : IFileService
    {

        private readonly HttpClient _httpClient;

        public FilesService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Task<ServiceResponse<string>> GetComposeFileAsync(string path)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<string>> GetEnvFileAsync(string path)
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

        public Task<ServiceResponse<string>> GetRawComposeFileAsync(string path)
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

        public Task<ServiceResponse<string>> SetComposeFileAsync(FarmerBotServices compose, string path)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<string>> SetEnvFileAsync(string env, string path)
        {
            throw new NotImplementedException();
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

        Task<ServiceResponse<FarmerBotServices>> IFileService.GetComposeFileAsync(string path)
        {
            throw new NotImplementedException();
        }
    }
}