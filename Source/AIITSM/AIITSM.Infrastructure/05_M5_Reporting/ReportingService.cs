using AIITSM.Application.Reporting;
using AIITSM.Domain._02_M2_IncidentManagement;
using AIITSM.Infrastructure._06_M6_AI;
using Microsoft.EntityFrameworkCore;


namespace AIITSM.Infrastructure._05_M5_Reporting;

public class ReportingService : IReportingService
{
    private readonly AIITSMDbContext _dbContext;

    public ReportingService(AIITSMDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IncidentStatisticsDto> GetIncidentStatisticsAsync(
    CancellationToken cancellationToken = default)
    {
        var totalIncidents = await _dbContext.Incidents
            .AsNoTracking()
            .CountAsync(cancellationToken);

        var openIncidents = await _dbContext.Incidents
            .AsNoTracking()
            .CountAsync(
                x => x.Status == IncidentStatus.Open,
                cancellationToken);

        var resolvedIncidents = await _dbContext.Incidents
            .AsNoTracking()
            .CountAsync(
                x => x.ResolvedAt != null,
                cancellationToken);

        return new IncidentStatisticsDto
        {
            TotalIncidents = totalIncidents,
            OpenIncidents = openIncidents,
            ResolvedIncidents = resolvedIncidents,

            // Escalation persistence will be integrated with M7.
            EscalatedIncidents = 0
        };
    }

    public async Task<IReadOnlyList<UnresolvedIncidentDto>> GetUnresolvedIncidentsAsync(
    CancellationToken cancellationToken = default)
    {
        return await _dbContext.Incidents
            .AsNoTracking()
            .Where(x =>
                x.Status == IncidentStatus.Open ||
                x.Status == IncidentStatus.InProgress)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new UnresolvedIncidentDto
            {
                IncidentId = x.IncidentId,
                Title = x.Title,
                Priority = x.Priority.ToString(),
                Status = x.Status.ToString(),
                CreatedAt = x.CreatedAt
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SupportTeamPerformanceDto>> GetTeamPerformanceAsync(
    CancellationToken cancellationToken = default)
    {
        return await _dbContext.Database
            .SqlQuery<SupportTeamPerformanceDto>($"""
            SELECT
                U.UserId AS UserId,
                U.Name AS UserName,

                COUNT(DISTINCT IA.IncidentId) AS AssignedIncidents,

                COUNT(DISTINCT CASE
                    WHEN I.ResolvedAt IS NOT NULL
                    THEN IA.IncidentId
                END) AS ResolvedIncidents,

                COUNT(DISTINCT CASE
                    WHEN I.Status IN ('Open', 'InProgress')
                    THEN IA.IncidentId
                END) AS OpenIncidents

            FROM Users U
            INNER JOIN IncidentAssignments IA
                ON U.UserId = IA.AssignedTo
            INNER JOIN Incidents I
                ON IA.IncidentId = I.IncidentId

            GROUP BY
                U.UserId,
                U.Name

            ORDER BY
                U.Name
            """)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<RecurringIncidentPatternDto>> GetRecurringIncidentPatternsAsync(
    CancellationToken cancellationToken = default)
    {
        var incidents = await _dbContext.Incidents
            .AsNoTracking()
            .Select(i => new
            {
                i.CategoryId,
                i.Title
            })
            .ToListAsync(cancellationToken);

        var stopWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "the",
        "a",
        "an",
        "to",
        "of",
        "and",
        "is",
        "not",
        "unable",
        "cannot",
        "can't",
        "working",
        "with",
        "for",
        "on",
        "in"
    };

        var keywordPatterns = incidents
            .GroupBy(i => i.CategoryId)
            .SelectMany(categoryGroup =>
                categoryGroup
                    .SelectMany(i => i.Title
                        .ToLowerInvariant()
                        .Replace("wi-fi","wifi")
                        .Split(
                            new[] { ' ', '-', '_', '/', '\\', '.', ',', ':', ';' },
                            StringSplitOptions.RemoveEmptyEntries)
                        .Where(word => word.Length >= 3 && !stopWords.Contains(word))
                        .Distinct()
                        .Select(word => new
                        {
                            categoryGroup.Key,
                            Word = word
                        }))
                    .GroupBy(x => new
                    {
                        CategoryId = x.Key,
                        x.Word
                    })
                    .Where(g => g.Count() > 1)
                    .Select(g => new RecurringIncidentPatternDto
                    {
                        CategoryId = g.Key.CategoryId,
                        Pattern = g.Key.Word,
                        IncidentCount = g.Count()
                    }))
            .OrderByDescending(x => x.IncidentCount)
            .ThenBy(x => x.Pattern)
            .ToList();

        return keywordPatterns;
    }
}