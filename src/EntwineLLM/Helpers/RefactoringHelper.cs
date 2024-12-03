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
            CodeType codeType,
            string manualPrompt = "")
        {
            var suggestion = await GetCodeSuggestionsAsync(methodCode, codeType, manualPrompt);

            switch (suggestion.Type)
            {
                // TO DO: other windows
                default:
                    await ShowSuggestionWindowAsync(suggestion.Code, activeDocumentPath);
                    break;
            }
        }

        private async Task<CodeSuggestionResponse> GetCodeSuggestionsAsync(string methodCode, CodeType codeType, string manualPrompt)
        {
            using var client = new HttpClient();
            client.Timeout = _options.LlmRequestTimeOut;

            var prompt = codeType switch
            {
                CodeType.Manual => PromptHelper.CreateForManualRequest(_options.LlmModel, methodCode, manualPrompt),
                CodeType.Refactor => PromptHelper.CreateForRefactor(_options.LlmModel, methodCode),
                CodeType.Test => PromptHelper.CreateForTests(_options.LlmModel, methodCode),
                CodeType.Documentation => PromptHelper.CreateForDocumentation(_options.LlmModel, methodCode),
                _ => throw new ArgumentException("Invalid requested code type"),
            };
            var content = new StringContent(prompt, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync($"{_options.LlmUrl}/api/chat", content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var code = JObject.Parse(responseContent)["message"]["content"].ToString();

                const string pattern = "```csharp(.*?)```";
                var matches = Regex.Matches(code, pattern, RegexOptions.Singleline);

                if (matches.Count == 0)
                {
                    return CodeSuggestionResponse.Success(codeType, code);
                }

                var extractedCode = new StringBuilder();
                foreach (Match match in matches)
                {
                    extractedCode.AppendLine(match.Groups[1].Value.Trim());
                }

                return CodeSuggestionResponse.Success(codeType, extractedCode.ToString());
            }
            catch (Exception ex)
            {
                return CodeSuggestionResponse.Failure();
            }
        }

        private async Task ShowSuggestionWindowAsync(string suggestion, string activeDocumentPath)
        {
            ToolWindowPane window = await WindowHelper.ShowToolWindowAsync<RefactorSuggestionWindow>(_package);
            var control = (RefactorSuggestionWindowControl)window.Content;
            control.DisplaySuggestion(suggestion, activeDocumentPath);
        }
    }
}
