using AIITSM.Application._02_M2_IncidentManagement;
using AIITSM.Domain._02_M2_IncidentManagement;
using AIITSM.Infrastructure._06_M6_AI;
using Microsoft.EntityFrameworkCore;

namespace AIITSM.Infrastructure._02_M2_IncidentManagement
{
    public class IncidentService : IIncidentService
    {
        private readonly AIITSMDbContext _dbContext;

        public IncidentService(AIITSMDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> CreateIncidentAsync(
            CreateIncidentRequest request,
            int currentUserId,
            CancellationToken cancellationToken = default)
        {
            var incident = new Incident
            {
                Title = request.Title.Trim(),
                Description = request.Description.Trim(),
                CategoryId = request.CategoryId,
                Priority = request.Priority,

                // Requirement: initial status is always Open — never
                // taken from client input.
                Status = IncidentStatus.Open,

                // Requirement: creator is always the logged-in employee —
                // never taken from client input.
                CreatedBy = currentUserId,

                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Incidents.Add(incident);
            await _dbContext.SaveChangesAsync(cancellationToken);

            // Incident number handling: we don't store a separate
            // "incident number" column. The DB already gives us a
            // guaranteed-unique, auto-incrementing IncidentId (identity
            // column), so the human-friendly number is just a formatted
            // view of it (INC-000001, INC-000002, ...), computed on read
            // in MapToSummary/MapToDetails below. This avoids a second
            // source of truth that could drift from IncidentId.
            return incident.IncidentId;
        }

        public async Task<IReadOnlyList<IncidentSummaryDto>> GetMyIncidentsAsync(
            int currentUserId,
            CancellationToken cancellationToken = default)
        {
            var incidents = await _dbContext.Incidents
                .AsNoTracking()
                .Include(x => x.Category)
                .Where(x => x.CreatedBy == currentUserId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);

            return incidents.Select(MapToSummary).ToList();
        }

        public async Task<IncidentDetailsDto?> GetIncidentDetailsAsync(
            int incidentId,
            CancellationToken cancellationToken = default)
        {
            var incident = await _dbContext.Incidents
                .AsNoTracking()
                .Include(x => x.Category)
                .FirstOrDefaultAsync(x => x.IncidentId == incidentId, cancellationToken);

            return incident is null ? null : MapToDetails(incident);
        }

        public async Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync(
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.Categories
                .AsNoTracking()
                .OrderBy(x => x.CategoryName)
                .Select(x => new CategoryDto
                {
                    CategoryId = x.CategoryId,
                    CategoryName = x.CategoryName
                })
                .ToListAsync(cancellationToken);
        }

        private static string FormatIncidentNumber(int incidentId) => $"INC-{incidentId:D6}";

        private static IncidentSummaryDto MapToSummary(Incident incident) => new()
        {
            IncidentId = incident.IncidentId,
            IncidentNumber = FormatIncidentNumber(incident.IncidentId),
            Title = incident.Title,
            CategoryName = incident.Category?.CategoryName ?? string.Empty,
            Priority = incident.Priority,
            Status = incident.Status,
            CreatedAt = incident.CreatedAt
        };

        private static IncidentDetailsDto MapToDetails(Incident incident) => new()
        {
            IncidentId = incident.IncidentId,
            IncidentNumber = FormatIncidentNumber(incident.IncidentId),
            Title = incident.Title,
            Description = incident.Description,
            CategoryName = incident.Category?.CategoryName ?? string.Empty,
            Priority = incident.Priority,
            Status = incident.Status,
            CreatedBy = incident.CreatedBy,
            CreatedAt = incident.CreatedAt,
            ResolvedAt = incident.ResolvedAt
        };
    }
}
