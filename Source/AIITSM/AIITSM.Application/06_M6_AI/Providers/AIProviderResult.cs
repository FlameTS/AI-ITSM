namespace AIITSM.Application._06_M6_AI.Providers
{
    public class AIProviderResult
    {
        public string SuggestedCategory { get; set; } = string.Empty;

        public string SuggestedPriority { get; set; } = string.Empty;

        public string SuggestedResolution { get; set; } = string.Empty;

        public decimal ConfidenceScore { get; set; }
    }
}