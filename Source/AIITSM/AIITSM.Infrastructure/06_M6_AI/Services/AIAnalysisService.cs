using AIITSM.Application._06_M6_AI.Contracts;
using AIITSM.Application._06_M6_AI.Providers;
using AIITSM.Application._06_M6_AI.Services;
using AIITSM.Domain._06_M6_AI;

namespace AIITSM.Infrastructure._06_M6_AI.Services
{
    public class AIAnalysisService : IAIAnalysisService
    {
        private readonly AIITSMDbContext _dbContext;
        private readonly IAIProvider _aiProvider;

        public AIAnalysisService(
            AIITSMDbContext dbContext,
            IAIProvider aiProvider)
        {
            _dbContext = dbContext;
            _aiProvider = aiProvider;
        }

        public async Task<int> RequestAnalysis(
            AnalyzeIncidentRequest request)
        {
            var analysis = new AIAnalysis
            {
                IncidentId = request.IncidentId,
                Status = AIAnalysisStatus.Pending
            };

            _dbContext.AIAnalyses.Add(analysis);

            await _dbContext.SaveChangesAsync();

            var providerRequest = new AIProviderRequest
            {
                IncidentId = request.IncidentId,
                Title = request.Title,
                Description = request.Description
            };

            try
            {
                var result = await _aiProvider.AnalyzeIncidentAsync(
                    providerRequest);

                analysis.SuggestedCategory = result.SuggestedCategory;
                analysis.SuggestedPriority = result.SuggestedPriority;
                analysis.SuggestedResolution = result.SuggestedResolution;
                analysis.ConfidenceScore = result.ConfidenceScore;

                analysis.Status = AIAnalysisStatus.Completed;
            }
            catch
            {
                analysis.Status = AIAnalysisStatus.Failed;

                await _dbContext.SaveChangesAsync();

                throw;
            }

            await _dbContext.SaveChangesAsync();

            return analysis.AIAnalysisId;
        }
    }
}