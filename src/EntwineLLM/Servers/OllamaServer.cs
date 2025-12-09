using EntwineLlm.Servers.Abstractions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace EntwineLlm.Servers
{
    public class OllamaServer() : LlmServer (NAME, "http://localhost:11434")
    {
        public const string NAME = "Ollama";

        public override async Task<string[]> GetModelListAsync()
        {
            try
            {
                List<string> modelList = [];
                using var client = new HttpClient();
                if (!string.IsNullOrEmpty(BearerToken))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", BearerToken);
                }
                client.Timeout = RequestTimeOut;

                var response = await client.GetAsync($"{BaseUrl}/api/tags");
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var models = JObject.Parse(responseContent)["models"];
                foreach (var model in models)
                {
                    modelList.Add(model["name"].ToString());
                }

                return [.. modelList];
            }
            catch
            {
                return [];
            }
        }

        public override async Task<string> GetChatCompletionAsync(StringContent content)
        {
            using var client = new HttpClient();
            if (!string.IsNullOrEmpty(BearerToken))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", BearerToken);
            }
            client.Timeout = RequestTimeOut;

            var response = await client.PostAsync($"{BaseUrl}/api/chat", content);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return JObject
                .Parse(responseContent)["message"]["content"]
                .ToString()
                .Replace("\r\n", Environment.NewLine);
        }
    }
}
