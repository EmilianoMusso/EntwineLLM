using EntwineLlm.Extensions;

namespace EntwineLlm.Helpers
{
    internal static class PromptHelper
    {
        public static string CreateForManualRequest(string model, string userCode, string prompt)
        {
            var promptText = PreparePrompt(prompt);
            return ReplacePlaceholders(promptText, model, userCode);
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

        private static string ReplacePlaceholders(string prompt, string model, string code)
        {
            return Properties.Resources.LlmBaseRequest
                    .Replace("{PROMPT}", prompt)
                    .Replace("{MODEL}", model)
                    .Replace("{CODE}", code.EscapeJsonString());
        }
    }
}
