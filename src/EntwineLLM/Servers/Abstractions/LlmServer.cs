using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace EntwineLlm.Servers.Abstractions
{
    public abstract class LlmServer(string name, string baseUri)
    {
        public string Name { get; } = name;
        public string BaseUrl { get; set; } = baseUri;
        public string BearerToken { get; set; } = "";
        public TimeSpan RequestTimeOut { get; set; } = new TimeSpan(0, 10, 0);

        public abstract Task<string[]> GetModelListAsync();
        public abstract Task<string> GetChatCompletionAsync(StringContent content);

        public override string ToString() => Name;
    }
}
