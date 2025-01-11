using EntwineLlm.Extensions;
using EntwineLlm.Models;
using Newtonsoft.Json;

namespace EntwineLlm.Helpers
{
    internal class PromptHelper
    {
        private readonly string _language;

        public PromptHelper(string language)
        {
            _language = language;
        }

        public string CreateForManualRequest(string model, string userCode, string prompt)
            => CreatePrompt(Properties.Resources.PromptForManual, model, userCode, prompt);

        public string CreateForRefactor(string model, string userCode)
            => CreatePrompt(Properties.Resources.PromptForRefactor, model, userCode);

        public string CreateForTests(string model, string userCode)
            => CreatePrompt(Properties.Resources.PromptForTests, model, userCode);

        public string CreateForDocumentation(string model, string userCode)
            => CreatePrompt(Properties.Resources.PromptForDocumentation, model, userCode);

        public string CreateForReview(string model, string userCode)
            => CreatePrompt(Properties.Resources.PromptForReview, model, userCode);

        private string CreatePrompt(string promptModel, string model, string userCode, string manualRequest = "")
        {
            var promptText = PreparePrompt(promptModel);
            return ReplacePlaceholders(promptText, model, userCode, manualRequest);
        }

        private static string PreparePrompt(string prompt)
        {
            return prompt
                .EscapeJsonString()
                .ReduceMultipleSpaces();
        }

        private string ReplacePlaceholders(string prompt, string model, string code, string manualRequest = "")
        {
            code = code.EscapeJsonString();
            manualRequest = manualRequest.EscapeJsonString();

            var languagePrompt = Properties.Resources.SetLanguagePrompt.Replace("{LANGUAGE}", _language);

            var llmRequest = LlmRequest.Create(model, languagePrompt, prompt, code, manualRequest);
            return JsonConvert.SerializeObject(llmRequest, Formatting.Indented, new JsonSerializerSettings()
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            });
        }
    }
}
