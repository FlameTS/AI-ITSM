using System.Text.Json;
using AIITSM.Application._06_M6_AI.Providers;
using Google.GenAI;
using Google.GenAI.Types;
using GenAIType = Google.GenAI.Types.Type;

namespace AIITSM.Infrastructure._06_M6_AI.Providers
{
    public class GeminiProvider : IAIProvider
    {
        private readonly Client _client;

        public GeminiProvider()
        {
            _client = new Client();
        }

        public async Task<AIProviderResult> AnalyzeIncidentAsync(
            AIProviderRequest request)
        {
            var prompt = $"""
                Analyze the following IT service desk incident.

                Incident ID: {request.IncidentId}
                Title: {request.Title}
                Description: {request.Description}

                Provide:
                - the most appropriate IT incident category
                - the appropriate priority
                - a concise recommended resolution
                - a confidence score between 0 and 1
                """;

            var schema = new Schema
            {
                Type = Google.GenAI.Types.Type.Object,

                Properties = new Dictionary<string, Schema>
                {
                    {
                        "suggestedCategory",
                        new Schema
                        {
                            Type = Google.GenAI.Types.Type.String
                        }
                    },
                    {
                        "suggestedPriority",
                        new Schema
                        {
                            Type = Google.GenAI.Types.Type.String
                        }
                    },
                    {
                        "suggestedResolution",
                        new Schema
                        {
                            Type = Google.GenAI.Types.Type.String
                        }
                    },
                    {
                        "confidenceScore",
                        new Schema
                        {
                            Type = Google.GenAI.Types.Type.Number
                        }
                    }
                },

                Required = new List<string>
                {
                    "suggestedCategory",
                    "suggestedPriority",
                    "suggestedResolution",
                    "confidenceScore"
                },

                PropertyOrdering = new List<string>
                {
                    "suggestedCategory",
                    "suggestedPriority",
                    "suggestedResolution",
                    "confidenceScore"
                }
            };

            var config = new GenerateContentConfig
            {
                ResponseMimeType = "application/json",
                ResponseSchema = schema                
            };

            var response = await _client.Models.GenerateContentAsync(
                model: "gemini-3.5-flash-lite",
                contents: prompt,
                config: config);

            var text = response.Candidates?
                .FirstOrDefault()?
                .Content?
                .Parts?
                .FirstOrDefault()?
                .Text;

            if (string.IsNullOrWhiteSpace(text))
            {
                throw new InvalidOperationException(
                    "Gemini returned an empty response.");
            }

            var result = JsonSerializer.Deserialize<AIProviderResult>(
                text,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (result == null)
            {
                throw new InvalidOperationException(
                    "Gemini returned an invalid AI analysis result.");
            }

            return result;
        }
    }
}