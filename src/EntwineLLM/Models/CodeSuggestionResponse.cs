using EntwineLlm.Enums;

namespace EntwineLlm.Models
{
    internal class CodeSuggestionResponse
    {
        public CodeType Type { get; set; }
        public string Code { get; set; }

        public static CodeSuggestionResponse Success(CodeType type, string code)
        {
            return new CodeSuggestionResponse
            {
                Type = type,
                Code = code
            };
        }

        public static CodeSuggestionResponse Failure()
        {
            return new CodeSuggestionResponse
            {
                Type = CodeType.Undefined,
                Code = string.Empty
            };
        }
    }
}
