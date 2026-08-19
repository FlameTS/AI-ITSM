
namespace AIITSM.Domain._06_M6_AI
{
    public class AIAnalysis
    {
        public int AIAnalysisId { get; set; }

        public int IncidentId { get; set; }
                
        public AIAnalysisStatus Status { get; set; }

        public string? SuggestedCategory { get; set; }

        public string? SuggestedPriority { get; set; }

        public string? SuggestedResolution { get; set; }
             
        public decimal? ConfidenceScore { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
