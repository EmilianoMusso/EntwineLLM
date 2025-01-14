using EntwineLlm.Enums;
using EntwineLlm.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EntwineLlm.Clients
{
    internal class LlmClient : IDisposable
    {
        private readonly string _baseUrl;
        private readonly TimeSpan _timeOut;
        private bool disposedValue;

        public LlmClient(string baseUrl, TimeSpan timeOut)
        {
            _baseUrl = baseUrl;
            _timeOut = timeOut;
        }

        public async Task<CodeSuggestionResponse> GetCodeSuggestionsAsync(CodeType codeType, string prompt)
        {
            using var client = new HttpClient();
            client.Timeout = _timeOut;

            var content = new StringContent(prompt, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync($"{_baseUrl}/api/chat", content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var code = JObject
                    .Parse(responseContent)["message"]["content"]
                    .ToString()
                    .Replace("\r\n", Environment.NewLine);

                const string pattern = @"```(?:([a-zA-Z0-9+#]*)\n)?(.*?)```";
                var matches = Regex.Matches(code, pattern, RegexOptions.Singleline);

                if (MustReturnFullResponse(matches, codeType))
                {
                    return CodeSuggestionResponse.Success(codeType, code);
                }

                var extractedCode = new StringBuilder();
                foreach (Match match in matches)
                {
                    extractedCode.AppendLine(match.Groups[2].Value.Trim());
                }

                return CodeSuggestionResponse.Success(codeType, extractedCode.ToString());
            }
            catch
            {
                return CodeSuggestionResponse.Failure();
            }
        }

        private static bool MustReturnFullResponse(MatchCollection matches, CodeType codeType)
        {
            return matches.Count == 0 || codeType == CodeType.Documentation || codeType == CodeType.Review;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
