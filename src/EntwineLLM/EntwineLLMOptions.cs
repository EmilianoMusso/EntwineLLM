using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel;

namespace EntwineLlm.Models
{
    public class GeneralOptions : DialogPage
    {
        [Category("Configuration")]
        [DisplayName("Large Language Model Base Url")]
        [Description("Sets the base URL for local LLM")]
        public string LlmUrl { get; set; } = "http://localhost:11434";

        [Category("Configuration")]
        [DisplayName("Requests timeout")]
        [Description("Sets timeout for HTTP requests")]

        public TimeSpan LlmRequestTimeOut { get; set; } = new TimeSpan(0, 10, 0);
    }

    public class ModelsOptions : DialogPage
    {
        [Category("Models")]
        [DisplayName("Refactor queries")]
        [Description("Sets the model to be used when querying LLM for refactor")]
        public string LlmRefactor { get; set; } = "llama3.2";

        [Category("Models")]
        [DisplayName("Unit tests generation")]
        [Description("Sets the model to be used when querying LLM for unit tests generation")]
        public string LlmUnitTests { get; set; } = "llama3.2";

        [Category("Models")]
        [DisplayName("Documentation generation")]
        [Description("Sets the model to be used when querying LLM for code documentation generation")]
        public string LlmDocumentation { get; set; } = "llama3.2";

        [Category("Models")]
        [DisplayName("Code review query")]
        [Description("Sets the model to be used when querying LLM for code review")]
        public string LlmReview { get; set; } = "llama3.2";

        [Category("Models")]
        [DisplayName("Follow-up query")]
        [Description("Sets the model to be used when querying LLM for follow-up prompts")]
        public string LlmFollowUp { get; set; } = "llama3.2";
    }
}
