using EntwineLlm.Enums;
using EntwineLlm.Helpers;
using EntwineLlm.Models;
using Microsoft.VisualStudio.Shell;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
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
            using var client = new HttpClient();
            client.Timeout = _options.LlmRequestTimeOut;

            var prompt = codeType switch
            {
                RequestedCodeType.Manual => PromptHelper.CreateForManualRequest(_options.LlmModel, methodCode, manualPrompt),
                RequestedCodeType.Refactor => PromptHelper.CreateForRefactor(_options.LlmModel, methodCode),
                RequestedCodeType.Test => PromptHelper.CreateForTests(_options.LlmModel, methodCode),
                _ => throw new ArgumentException("Invalid requested code type"),
            };
            var content = new StringContent(prompt, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync($"{_options.LlmUrl}/api/chat", content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var code = JObject.Parse(responseContent)["message"]["content"].ToString();

                const string pattern = @"```csharp(.*?)```";
                var matches = Regex.Matches(code, pattern, RegexOptions.Singleline);

                var extractedCode = new StringBuilder();
                foreach (Match match in matches)
                {
                    extractedCode.AppendLine(match.Groups[1].Value.Trim());
                }

                return extractedCode.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
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
