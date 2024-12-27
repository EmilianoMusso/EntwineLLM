using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace EntwineLlm.Models
{
    public abstract class LLMServerBase(string name, Uri baseUri)
    {
        public string Name { get; } = name;
        public Uri BaseUrl { get; set; } = baseUri;
        public TimeSpan RequestTimeOut { get; set; } = new TimeSpan(0, 10, 0);

        public abstract Task<string[]> GetModelListAsync();
        public abstract Task<string> GetChatCompletionAsync(StringContent prompt);

        public override string ToString() => Name;
    }
}
