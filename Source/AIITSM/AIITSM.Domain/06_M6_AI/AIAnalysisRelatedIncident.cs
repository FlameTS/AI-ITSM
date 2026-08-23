
namespace AIITSM.Domain._06_M6_AI
{
    public class AIAnalysisRelatedIncident
    {
        public int AIAnalysisRelatedIncidentId { get; set; }

        public int AIAnalysisId { get; set; }

        public int RelatedIncidentId { get; set; }

        public AIIncidentRelationshipType RelationshipType { get; set; }

        public double SimilarityScore { get; set; }
    }
}
