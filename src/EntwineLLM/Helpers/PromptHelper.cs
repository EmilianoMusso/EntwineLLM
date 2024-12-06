using EntwineLlm.Extensions;
using EntwineLlm.Models;
using Newtonsoft.Json;

namespace EntwineLlm.Helpers
{
    internal static class PromptHelper
    {
        public static string CreateForManualRequest(string model, string userCode, string prompt)
            => CreatePrompt(Properties.Resources.PromptForManual, model, userCode);

        public static string CreateForRefactor(string model, string userCode)
            => CreatePrompt(Properties.Resources.PromptForRefactor, model, userCode);

        public static string CreateForTests(string model, string userCode)
            => CreatePrompt(Properties.Resources.PromptForTests, model, userCode);

        public static string CreateForDocumentation(string model, string userCode)
            => CreatePrompt(Properties.Resources.PromptForDocumentation, model, userCode);

        public static string CreateForReview(string model, string userCode)
            => CreatePrompt("", model, userCode);

        private static string CreatePrompt(string promptModel, string model, string userCode)
        {
            var promptText = PreparePrompt(promptModel);
            return ReplacePlaceholders(promptText, model, userCode);
        }

        private static string PreparePrompt(string prompt)
        {
            return prompt
                .EscapeJsonString()
                .ReduceMultipleSpaces();
        }

        private static string ReplacePlaceholders(string prompt, string model, string code, string manualRequest = "")
        {
            code = code.EscapeJsonString();
            manualRequest = manualRequest.EscapeJsonString();

            var llmRequest = LlmRequest.Create(model, prompt, code, manualRequest);
            return JsonConvert.SerializeObject(llmRequest, Formatting.Indented, new JsonSerializerSettings()
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            });
        }
    }
}
