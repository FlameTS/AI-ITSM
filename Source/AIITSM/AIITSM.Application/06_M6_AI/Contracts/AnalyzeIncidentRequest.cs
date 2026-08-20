
namespace AIITSM.Application._06_M6_AI.Contracts
{
    public class AnalyzeIncidentRequest
    {
        public int IncidentId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
    }
}
