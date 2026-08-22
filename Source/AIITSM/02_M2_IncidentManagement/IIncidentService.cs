namespace AIITSM.Application._02_M2_IncidentManagement
{
    public interface IIncidentService
    {
        // Creates the incident, forcing Status = Open and
        // CreatedBy = currentUserId server-side. Returns the new IncidentId.
        Task<int> CreateIncidentAsync(
            CreateIncidentRequest request,
            int currentUserId,
            CancellationToken cancellationToken = default);

        // Only the incidents created by currentUserId — this is the
        // "My Incidents" requirement, enforced here so no controller
        // or view can accidentally leak someone else's incidents.
        Task<IReadOnlyList<IncidentSummaryDto>> GetMyIncidentsAsync(
            int currentUserId,
            CancellationToken cancellationToken = default);

        Task<IncidentDetailsDto?> GetIncidentDetailsAsync(
            int incidentId,
            CancellationToken cancellationToken = default);

        // Needed to populate the Category dropdown on the Create form.
        Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync(
            CancellationToken cancellationToken = default);
    }
}
