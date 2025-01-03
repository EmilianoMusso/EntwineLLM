﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EntwineLlm.Models
{
    public class OllamaServer() : LLMServerBase ("Ollama", new Uri("http://localhost:11434"))
    {
        public override async Task<string[]> GetModelListAsync()
        {
            List<string> modelList = [];
            using var client = new HttpClient();
            client.Timeout = RequestTimeOut;

            var response = await client.GetAsync($"{BaseUrl}api/tags");
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var models = JObject.Parse(responseContent)["models"];
            foreach (var model in models)
            {
                modelList.Add(model["name"].ToString());
            }

            return [.. modelList];
        }

        public override async Task<string> GetChatCompletionAsync(StringContent content)
        {
            using var client = new HttpClient();
            client.Timeout = RequestTimeOut;

            var response = await client.PostAsync($"{BaseUrl}api/chat", content);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var code = JObject.Parse(responseContent)["message"]["content"].ToString();
            return code;
        }

    }
}
