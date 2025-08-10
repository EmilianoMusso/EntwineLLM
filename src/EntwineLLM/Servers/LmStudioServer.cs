using EntwineLlm.Servers.Abstractions;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace EntwineLlm.Servers
{
    public class LMStudioServer() : LlmServer("LM Studio", "http://localhost:1234")
    {
        public override async Task<string[]> GetModelListAsync()
        {
            try
            {
                List<string> modelList = [];
                using var client = new HttpClient();
                client.Timeout = RequestTimeOut;

                var response = await client.GetAsync($"{BaseUrl}/v1/models");
                if (!string.IsNullOrEmpty(BearerToken))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", BearerToken);
                }
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var models = JObject.Parse(responseContent)["data"];
                foreach (var model in models)
                {
                    modelList.Add(model["id"].ToString());
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

            var response = await client.PostAsync($"{BaseUrl}/v1/chat/completions", content);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var choices = JObject.Parse(responseContent)["choices"];
            if (!choices.Any())
            {
                return string.Empty;
            }
            return choices[0]["message"]["content"].ToString();
        }
    }
}
