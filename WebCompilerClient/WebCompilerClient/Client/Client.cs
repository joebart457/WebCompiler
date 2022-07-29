using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCompilerClient.Extensions;
using WebCompilerClient.Models;
using WebCompilerClient.Models.Enums;
using WebCompilerClient.Services;

namespace WebCompilerClient.Client
{
    internal static class Client
    {
        private static readonly string? _baseUrl = ConfigService.GetConfig("baseUrl");
        private static async Task<Ty> SendGetRequestAsync<Ty>(string resourceUri)
        {
            HttpClient httpClient = new HttpClient();
            return await httpClient.DownloadToJson<Ty>(new Uri($"{_baseUrl}{resourceUri}"));
        }

        private static async Task SendPostRequestAsync<Ty>(string resourceUri, Ty data)
        {
            HttpClient httpClient = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{_baseUrl}{resourceUri}"),
                Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json"),
            };

            var response = await httpClient.SendAsync(request).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }

        private static async Task<Ty2> SendPostRequestExpectResponseAsync<Ty1, Ty2>(string resourceUri, Ty1 data)
        {
            HttpClient httpClient = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{_baseUrl}{resourceUri}"),
                Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json"),
            };

            var response = await httpClient.SendAsync(request).ConfigureAwait(false);
            if (response != null && response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                Ty2? finalObject = JsonConvert.DeserializeObject<Ty2>(result);
                if (finalObject == null) throw new Exception($"unable to convert response to valid package object. Response was {result}");
                return finalObject;
            }
            throw new Exception($"received [Status:{response?.StatusCode}] when attempting to retrieve resource at {resourceUri}");
        }

        private static async Task SendUploadRequestAsync(string resourceUri, string filePath)
        {
            HttpClient httpClient = new HttpClient();
            await httpClient.Upload($"{_baseUrl}{resourceUri}", filePath);
        }

        private static async Task SendDeleteRequestAsync(string resourceUri)
        {
            HttpClient httpClient = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri($"{_baseUrl}{resourceUri}"),
            };

            var response = await httpClient.SendAsync(request).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }

        private static async Task SendDownloadRequestAsync(string resourceUri, string filePath)
        {
            HttpClient httpClient = new HttpClient();
            await httpClient.DownloadFileTaskAsync(new Uri($"{_baseUrl}{resourceUri}"), filePath);
        }

        public static async Task<BuildResult> BuildAsync(string guid, Runtimes runtime)
        {
            return await SendGetRequestAsync<BuildResult>($"/projects/{guid}/build/{runtime}");
        }

        public static async Task<UserProject> GetAsync(string guid)
        {
            return await SendGetRequestAsync<UserProject>($"/Projects/{guid}");
        }

        public static async Task AddFileAsync(string guid, string filePath)
        {
            await SendUploadRequestAsync($"/Code/{guid}", filePath);
        }

        public static async Task<UserProject> CreateProjectAsync(ProjectManifest manifest)
        {
            return await SendPostRequestExpectResponseAsync<ProjectManifest, UserProject>($"/Projects", manifest);
        }

        public static async Task DownloadResultsAsync(string guid, string filePath)
        {
            await SendDownloadRequestAsync($"/download/{guid}", filePath);
        }

        public static async Task DeleteAsync(string guid)
        {
            await SendDeleteRequestAsync($"/projects/{guid}");
        }

    }
}
