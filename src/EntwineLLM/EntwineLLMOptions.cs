using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel;

namespace EntwineLlm.Models
{
    public class EntwineLlmOptions : DialogPage
    {
        [Category("Configuration")]
        [DisplayName("Large Language Model Base Url")]
        [Description("Sets the base URL for local LLM")]
        public string LlmUrl { get; set; } = "http://localhost:11434";

        [Category("Configuration")]
        [DisplayName("Requests timeout")]
        [Description("Sets timeout for HTTP requests")]

        public TimeSpan LlmRequestTimeOut { get; set; } = new TimeSpan(0, 10, 0);

        [Category("Configuration")]
        [DisplayName("LLM Model")]
        [Description("Sets the model to be used when querying LLM")]
        public string LlmModel { get; set; } = "llama3.2";
    }
}
