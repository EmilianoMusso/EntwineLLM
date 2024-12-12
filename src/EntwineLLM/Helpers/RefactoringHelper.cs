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
        private readonly GeneralOptions _generalOptions;
        private readonly ModelsOptions _modelsOptions;
        private readonly AsyncPackage _package;

        public RefactoringHelper(AsyncPackage package)
        {
            _package = package;
            _generalOptions = package.GetDialogPage(typeof(GeneralOptions)) as GeneralOptions;
            _modelsOptions = package.GetDialogPage(typeof(ModelsOptions)) as ModelsOptions;
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
                case CodeType.Documentation:
                case CodeType.Review:
                    await ShowMarkdownWindowAsync(suggestion.Code);
                    break;

                default:
                    await ShowSuggestionWindowAsync(suggestion.Code, activeDocumentPath);
                    break;
            }
        }

        private async Task<CodeSuggestionResponse> GetCodeSuggestionsAsync(string methodCode, CodeType codeType, string manualPrompt)
        {
            using var client = new HttpClient();
            client.Timeout = _generalOptions.LlmRequestTimeOut;

            var prompt = codeType switch
            {
                CodeType.Manual => PromptHelper.CreateForManualRequest(_modelsOptions.LlmFollowUp, methodCode, manualPrompt),
                CodeType.Refactor => PromptHelper.CreateForRefactor(_modelsOptions.LlmRefactor, methodCode),
                CodeType.Test => PromptHelper.CreateForTests(_modelsOptions.LlmUnitTests, methodCode),
                CodeType.Documentation => PromptHelper.CreateForDocumentation(_modelsOptions.LlmDocumentation, methodCode),
                CodeType.Review => PromptHelper.CreateForReview(_modelsOptions.LlmReview, methodCode),
                _ => throw new ArgumentException("Invalid requested code type"),
            };
            var content = new StringContent(prompt, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync($"{_generalOptions.LlmUrl}/api/chat", content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var code = JObject.Parse(responseContent)["message"]["content"].ToString();

                const string pattern = "```csharp(.*?)```";
                var matches = Regex.Matches(code, pattern, RegexOptions.Singleline);

                if (MustReturnFullResponse(matches, codeType))
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
            catch
            {
                return CodeSuggestionResponse.Failure();
            }
        }

        private bool MustReturnFullResponse(MatchCollection matches, CodeType codeType)
        {
            return matches.Count == 0 || codeType == CodeType.Documentation || codeType == CodeType.Review;
        }

        private async Task ShowSuggestionWindowAsync(string suggestion, string activeDocumentPath)
        {
            ToolWindowPane window = await WindowHelper.ShowToolWindowAsync<RefactorSuggestionWindow>(_package);
            var control = (RefactorSuggestionWindowControl)window.Content;
            control.DisplaySuggestion(suggestion, activeDocumentPath);
        }

        private async Task ShowMarkdownWindowAsync(string suggestion)
        {
            ToolWindowPane window = await WindowHelper.ShowToolWindowAsync<MarkdownViewerWindow>(_package);
            var control = (MarkdownViewer)window.Content;
            control.DisplaySuggestion(suggestion);
        }
    }
}
