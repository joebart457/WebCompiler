using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCompilerClient.Extensions
{
    public static class HttpClientExtensions
    {
        public static async Task DownloadFileTaskAsync(this HttpClient client, Uri uri, string fileName)
        {
            using (var s = await client.GetStreamAsync(uri))
            {

                using (var fs = new FileStream(fileName, FileMode.Create))
                {
                    await s.CopyToAsync(fs);
                }
            }
        }

        public static async Task<Ty> DownloadToJson<Ty>(this HttpClient client, Uri uri)
        {
            var response = await client.GetAsync(uri);

            if (response != null && response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                Ty? finalObject = JsonConvert.DeserializeObject<Ty>(result);
                if (finalObject == null) throw new Exception($"unable to convert response to valid package object. Response was {result}");
                return finalObject;
            }
            throw new Exception($"received [Status:{response?.StatusCode}] when attempting to retrieve resource at {uri.OriginalString}");
        }

        public static async Task<Ty> DownloadToJson<Ty>(this HttpClient client, string uri, HttpContent content)
        {
            var response = await client.PostAsync(uri, content);

            if (response != null && response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                Ty? finalObject = JsonConvert.DeserializeObject<Ty>(result);
                if (finalObject == null) throw new Exception($"unable to convert response to valid package object. Response was {result}");
                return finalObject;
            }
            throw new Exception($"received [Status:{response?.StatusCode}] when attempting to retrieve resource at {uri}");
        }

        public static async Task Upload(this HttpClient client, string uri, string filePath)
        {
            var fs = File.OpenRead(filePath);
            HttpContent fileStreamContent = new StreamContent(fs);
            
            using (var formData = new MultipartFormDataContent())
            {
                formData.Add(fileStreamContent, Path.GetFileName(filePath), Path.GetFileName(filePath));
                var response = await client.PostAsync(uri, formData);
                response.EnsureSuccessStatusCode();
            }
        }
    }
}
