
namespace AIITSM.Application._06_M6_AI.Services
{
    public interface IAIAnalysisService
    {
        Task<int> RequestAnalysis(Contracts.AnalyzeIncidentRequest request);
    }
}
