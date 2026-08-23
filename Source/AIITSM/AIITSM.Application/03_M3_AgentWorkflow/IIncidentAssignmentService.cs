namespace AIITSM.Application._03_M3_AgentWorkflow
{
    public interface IIncidentAssignmentService
    {
        Task<int?> GetAssignedAgentAsync(
            int incidentId,
            CancellationToken cancellationToken = default);

        Task AssignAgentAsync(
            int incidentId,
            int? assignedTo,
            CancellationToken cancellationToken = default);
    }
}