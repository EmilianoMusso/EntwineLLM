using EntwineLlm.Enums;
using EntwineLlm.Helpers;
using EntwineLlm.Models;
using Microsoft.VisualStudio.Shell;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace EntwineLlm
{
    internal class RefactoringHelper
    {
        private readonly EntwineLlmOptions _options;
        private readonly AsyncPackage _package;

        public RefactoringHelper(AsyncPackage package)
        {
            _package = package;
            _options = package.GetDialogPage(typeof(EntwineLlmOptions)) as EntwineLlmOptions;
        }

        public async Task RequestCodeSuggestionsAsync(
            string methodCode,
            string activeDocumentPath,
            RequestedCodeType codeType,
            string manualPrompt = "")
        {
            var suggestion = await GetCodeSuggestionsAsync(methodCode, codeType, manualPrompt);
            await ShowRefactoringSuggestionAsync(suggestion, activeDocumentPath);
        }

        private async Task<string> GetCodeSuggestionsAsync(string methodCode, RequestedCodeType codeType, string manualPrompt)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = _options.LlmRequestTimeOut;

                var prompt = string.Empty;

                switch (codeType)
                {
                    case RequestedCodeType.Manual:
                        prompt = PromptHelper.CreateForManualRequest(_options.LlmModel, methodCode, manualPrompt);
                        break;
                    case RequestedCodeType.Refactor:
                        prompt = PromptHelper.CreateForRefactor(_options.LlmModel, methodCode);
                        break;
                    case RequestedCodeType.Test:
                        prompt = PromptHelper.CreateForTests(_options.LlmModel, methodCode);
                        break;
                    default:
                        throw new ArgumentException("Invalid requested code type");
                }

                var content = new StringContent(prompt, Encoding.UTF8, "application/json");

                try
                {
                    var response = await client.PostAsync($"{_options.LlmUrl}/api/chat", content);
                    response.EnsureSuccessStatusCode();

                    var responseContent = await response.Content.ReadAsStringAsync();
                    var code = JObject.Parse(responseContent)["message"]["content"].ToString();

                    return code
                        .Replace("```csharp", "")
                        .Replace("```", "");
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        private async Task ShowRefactoringSuggestionAsync(string suggestion, string activeDocumentPath)
        {
            ToolWindowPane window = await WindowHelper.ShowToolWindowAsync<RefactorSuggestionWindow>(_package);
            var control = (RefactorSuggestionWindowControl)window.Content;
            control.DisplaySuggestion(suggestion, activeDocumentPath);
        }
    }
}
