using EntwineLlm.Enums;
using EntwineLlm.Models;
using System;
using System.Threading.Tasks;

namespace EntwineLlm.Clients.Interfacs
{
    internal interface ILlmClient
    {
        void SetBaseUrl(string baseUrl);
        string GetBaseUrl();
        void SetTimeOut(TimeSpan timeOut);
        TimeSpan GetTimeOut();
        Task<string[]> GetModelListAsync();
        Task<CodeSuggestionResponse> GetCodeSuggestionsAsync(CodeType codeType, string prompt);
    }
}
