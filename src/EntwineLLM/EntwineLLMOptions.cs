using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using static System.ComponentModel.TypeConverter;

namespace EntwineLlm.Models
{
    public class GeneralOptions : DialogPage
    {
        private class LlmServerConverter : TypeConverter
        {
            private static readonly LLMServerBase[] Servers = [.. Assembly.GetExecutingAssembly().GetTypes()
                .Where(type => type.IsSubclassOf(typeof(LLMServerBase)) && !type.IsAbstract)
                .Select(type => (LLMServerBase)Activator.CreateInstance(type))];

            public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;

            public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => true;

            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) => new(Servers);

            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
                => destinationType == typeof(string);

            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
                => destinationType == typeof(string)
                ? ((LLMServerBase)value).Name
                : base.ConvertTo(context, culture, value, destinationType);

            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
                => sourceType == typeof(string);

            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
                => value is string stringValue
                ? Servers.FirstOrDefault(server => server.Name == stringValue)
                : base.ConvertFrom(context, culture, value);
        }

        private LLMServerBase llmServer = new OllamaServer();
        [Category("Configuration")]
        [DisplayName("Select LLM Server")]
        [Description("Choose from available LLM Server")]
        [TypeConverter(typeof(LlmServerConverter))]
        public LLMServerBase LlmServer{
            get => llmServer;
            set
            {
                llmServer = value;
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
