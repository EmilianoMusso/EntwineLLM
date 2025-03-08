using EntwineLlm.Converters;
using EntwineLlm.Servers;
using EntwineLlm.Servers.Abstractions;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using System;
using System.ComponentModel;

namespace EntwineLlm.Models
{
    public class GeneralOptions : DialogPage
    {
        private static LlmServer currentServer;
        public static LlmServer CurrentServer => currentServer ??= new OllamaServer();

        private LlmServer llmServer;
        [Category("Configuration")]
        [DisplayName("Select LLM Server")]
        [Description("Choose from available LLM Server")]
        [TypeConverter(typeof(LlmServerConverter))]
        public LlmServer LlmServer
        {
            get => llmServer;
            set
            {
                llmServer = value ?? currentServer;
                currentServer = value;
                LlmUrl = value.BaseUrl;
                LlmRequestTimeOut = value.RequestTimeOut;
            }
        }

        [Category("Configuration")]
        [DisplayName("Large Language Model Base Url")]
        [Description("Sets the base URL for local LLM")]
        public string LlmUrl
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

        [Category("Configuration")]
        [DisplayName("LLM response language")]
        [Description("Set the language in which the LLM must answer")]
        [TypeConverter(typeof(LanguageConverter))]
        public string Language { get; set; } = "English";
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
