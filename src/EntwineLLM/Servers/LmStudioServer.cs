using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EntwineLlm.Models
{
    public class LMStudioServer() : LLMServerBase("LM Studio", new Uri("http://localhost:1234"))
    {
        public override async Task<string[]> GetModelListAsync()
        {
            List<string> modelList = [];
            using var client = new HttpClient();
            client.Timeout = RequestTimeOut;

            var response = await client.GetAsync($"{BaseUrl}v1/models");
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var models = JObject.Parse(responseContent)["data"];
            foreach (var model in models)
            {
                modelList.Add(model["id"].ToString());
            }

            return [.. modelList];
        }

        public override async Task<string> GetChatCompletionAsync(StringContent content)
        {
            using var client = new HttpClient();
            client.Timeout = RequestTimeOut;

            var response = await client.PostAsync($"{BaseUrl}v1/chat/completions", content);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var choices = JObject.Parse(responseContent)["choices"];
            if (choices.Any())
                return choices[0]["message"]["content"].ToString();
            return string.Empty;
        }
    }
}
