using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using System;
using System.ComponentModel;

namespace EntwineLlm.Models
{
    public class GeneralOptions : DialogPage
    {

        // TODO : Decide if it's better to use a static field or not
        private static LLMServerBase currentServer;
        public static LLMServerBase CurrentServer => currentServer ??= new OllamaServer();

        private LLMServerBase llmServer = currentServer;
        [Category("Configuration")]
        [DisplayName("Select LLM Server")]
        [Description("Choose from available LLM Server")]
        [TypeConverter(typeof(LlmServerConverter))]
        public LLMServerBase LlmServer
        {
            get => llmServer;
            set
            {
                llmServer = value;
                currentServer = value;
                LlmUrl = value.BaseUrl;
                LlmRequestTimeOut = value.RequestTimeOut;
            }
        }

        [Category("Configuration")]
        [DisplayName("Large Language Model Base Url")]
        [Description("Sets the base URL for local LLM")]
        public Uri LlmUrl
        {
            get => LlmServer?.BaseUrl;
            set => LlmServer.BaseUrl = value;
        }

            [Category("Configuration")]
        [DisplayName("Requests timeout")]
        [Description("Sets timeout for HTTP requests")]
        public TimeSpan LlmRequestTimeOut
        {
            get => LlmServer?.RequestTimeOut ?? new TimeSpan(0, 10, 0);
            set => LlmServer.RequestTimeOut = value;
        }
    }

    public class ModelsOptions : DialogPage
    {
        public class LlmModelConverter : StringConverter
        {
            public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;

            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
                => new(ThreadHelper.JoinableTaskFactory.Run(async () => await GeneralOptions.CurrentServer?.GetModelListAsync()));

            public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => true;
        }

        [Category("Models")]
        [DisplayName("Refactor queries")]
        [Description("Sets the model to be used when querying LLM for refactor")]
        [TypeConverter(typeof(LlmModelConverter))]
        public string LlmRefactor { get; set; } = "llama3.2";

        [Category("Models")]
        [DisplayName("Unit tests generation")]
        [Description("Sets the model to be used when querying LLM for unit tests generation")]
        [TypeConverter(typeof(LlmModelConverter))]
        public string LlmUnitTests { get; set; } = "llama3.2";

        [Category("Models")]
        [DisplayName("Documentation generation")]
        [Description("Sets the model to be used when querying LLM for code documentation generation")]
        [TypeConverter(typeof(LlmModelConverter))]
        public string LlmDocumentation { get; set; } = "llama3.2";

        [Category("Models")]
        [DisplayName("Code review query")]
        [Description("Sets the model to be used when querying LLM for code review")]
        [TypeConverter(typeof(LlmModelConverter))]
        public string LlmReview { get; set; } = "llama3.2";

        [Category("Models")]
        [DisplayName("Follow-up query")]
        [Description("Sets the model to be used when querying LLM for follow-up prompts")]
        [TypeConverter(typeof(LlmModelConverter))]
        public string LlmFollowUp { get; set; } = "llama3.2";
    }
}
