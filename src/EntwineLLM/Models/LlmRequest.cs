using Newtonsoft.Json;
using System.Collections.Generic;

namespace EntwineLlm.Models
{
    internal class LlmRequest
    {
        [JsonProperty(PropertyName = "model")]
        public string Model { get; set; }

        [JsonProperty(PropertyName = "stream")]
        public bool Stream { get; set; }

        [JsonProperty(PropertyName = "messages")]
        public List<LlmMessage> Messages { get; set; }

        public static LlmRequest Create(string model, string prompt, string userCode, string manualRequest = "")
        {
            var llmRequest = new LlmRequest()
            {
                Model = model,
                Stream = false,
                Messages = new List<LlmMessage>()
                {
                    LlmMessage.CreateAssistantMessage(prompt),
                    LlmMessage.CreateUserMessage("[CODE]: " + userCode)
                }
            };

            if (!string.IsNullOrEmpty(manualRequest))
            {
                var request = LlmMessage.CreateUserMessage("[REQUEST]: " + manualRequest);
                llmRequest.Messages.Add(request);
            }

            return llmRequest;
        }
    }

    internal class LlmMessage
    {
        [JsonProperty(PropertyName = "role")]
        public string Role { get; set; }

        [JsonProperty(PropertyName = "content")]
        public string Content { get; set; }

        public static LlmMessage CreateAssistantMessage(string content)
        {
            return new LlmMessage()
            {
                Role = "assistant",
                Content = content
            };
        }

        public static LlmMessage CreateUserMessage(string content)
        {
            return new LlmMessage()
            {
                Role = "user",
                Content = content
            };
        }
    }
}
