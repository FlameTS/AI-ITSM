namespace AIITSM.Application._02_M2_IncidentManagement_2.Feedback
{
    public interface IIncidentFeedbackService
    {
        Task<IncidentFeedbackDto?> GetFeedbackAsync(
            int incidentId,
            int userId,
            CancellationToken cancellationToken = default);

        Task AddFeedbackAsync(
            int incidentId,
            int userId,
            string? feedbackText,
            CancellationToken cancellationToken = default);
    }
}