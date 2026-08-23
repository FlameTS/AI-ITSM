using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AIITSM.Application._02_M2_IncidentManagement_2.Communication
{
    public interface IIncidentCommentService
    {
        Task<IReadOnlyList<IncidentCommentDto>> GetCommentsAsync(
            int incidentId,
            CancellationToken cancellationToken = default);

        Task AddCommentAsync(
            int incidentId,
            int userId,
            string commentText,
            CancellationToken cancellationToken = default);
    }
}