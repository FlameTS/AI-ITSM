using AIITSM.Domain._02_M2_IncidentManagement;

namespace AIITSM.Application._02_M2_IncidentManagement
{
    public interface IIncidentService
    {
        Task<int> CreateIncidentAsync(
            CreateIncidentRequest request,
            int currentUserId,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<IncidentSummaryDto>> GetMyIncidentsAsync(
            int currentUserId,
            CancellationToken cancellationToken = default);

        Task<IncidentDetailsDto?> GetIncidentDetailsAsync(
            int incidentId,
            CancellationToken cancellationToken = default);

        Task UpdateStatusAsync(
            int incidentId,
            IncidentStatus status,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync(
            CancellationToken cancellationToken = default);
    }
}