using AIITSM.Application._03_M3_AgentWorkflow;
using AIITSM.Domain._03_M3_AgentWorkflow;
using AIITSM.Infrastructure._06_M6_AI;
using Microsoft.EntityFrameworkCore;

namespace AIITSM.Infrastructure._03_M3_AgentWorkflow
{
    public class IncidentAssignmentService : IIncidentAssignmentService
    {
        private readonly AIITSMDbContext _dbContext;

        public IncidentAssignmentService(AIITSMDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int?> GetAssignedAgentAsync(
            int incidentId,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.IncidentAssignments
                .AsNoTracking()
                .Where(x => x.IncidentId == incidentId)
                .OrderByDescending(x => x.AssignedAt)
                .Select(x => (int?)x.AssignedTo)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task AssignAgentAsync(
            int incidentId,
            int? assignedTo,
            CancellationToken cancellationToken = default)
        {
            var incidentExists = await _dbContext.Incidents
                .AsNoTracking()
                .AnyAsync(x => x.IncidentId == incidentId, cancellationToken);

            if (!incidentExists)
            {
                throw new InvalidOperationException(
                    "The specified incident does not exist.");
            }

            if (assignedTo.HasValue)
            {
                // Reassignment: keep prior rows as history, just add
                // the new current assignment. GetAssignedAgentAsync
                // already reads the latest by AssignedAt.
                _dbContext.IncidentAssignments.Add(new IncidentAssignment
                {
                    IncidentId = incidentId,
                    AssignedTo = assignedTo.Value,
                    AssignedAt = DateTime.UtcNow
                });
            }
            else
            {
                // Unassign: AssignedTo is NOT NULL in the database, so
                // "no agent" cannot be stored as a row. Remove existing
                // rows for this incident to represent the unassigned state.
                var existingAssignments = await _dbContext.IncidentAssignments
                    .Where(x => x.IncidentId == incidentId)
                    .ToListAsync(cancellationToken);

                _dbContext.IncidentAssignments.RemoveRange(existingAssignments);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}