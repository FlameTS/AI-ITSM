namespace AIITSM.Application._06_M6_AI.Providers
{
    public interface IAIProvider
    {
        Task<AIProviderResult> AnalyzeIncidentAsync(
            AIProviderRequest request);
    }
}