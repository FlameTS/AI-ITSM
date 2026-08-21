namespace AIITSM.Application._06_M6_AI.Providers
{
    public class AIProviderRequest
    {
        public int IncidentId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
    }
}