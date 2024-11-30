using EntwineLlm.Extensions;
using EntwineLlm.Models;
using Newtonsoft.Json;

namespace EntwineLlm.Helpers
{
    internal static class PromptHelper
    {
        public static string CreateForManualRequest(string model, string userCode, string prompt)
        {
            var promptText = PreparePrompt(Properties.Resources.PromptForManual);
            return ReplacePlaceholders(promptText, model, userCode, prompt);
        }

        public static string CreateForRefactor(string model, string userCode)
        {
            var promptText = PreparePrompt(Properties.Resources.PromptForRefactor);
            return ReplacePlaceholders(promptText, model, userCode);
        }

        public static string CreateForTests(string model, string userCode)
        {
            var promptText = PreparePrompt(Properties.Resources.PromptForTests);
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
