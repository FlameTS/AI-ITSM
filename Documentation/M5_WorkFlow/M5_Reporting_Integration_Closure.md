# AI-ITSM — M5 Reporting & Monitoring Integration Closure

## 1. Module

**M5 — Reporting & Monitoring**

Project: **AI-Powered IT Service Management and Incident Management System (AI-ITSM)**

M5 was integrated into the existing main repository after M1, M2, M3, M4, and M6 had already been integrated.

## 2. Integration Objective

The objective was to integrate the M5 teammate implementation into the existing codebase with the smallest safe change set.

The integration explicitly followed these principles:

- Preserve existing M1/M2/M3/M4/M6 ownership.
- Do not redesign existing modules.
- Do not rebuild M5 from scratch.
- Reuse the existing application architecture and database.
- Avoid duplicate authentication, databases, entities, or services where unnecessary.
- Do not modify `Database.sql` unless a proven requirement exists.
- Keep M7 integration possible without redesigning M5.
- Do not invent unspecified requirements.
- Resolve implementation decisions from the actual repository, supplied requirements, supplied M5 implementation, and live database behavior.

## 3. M5 Scope

The M5 responsibility is Reporting & Monitoring / Manager Dashboard and includes the reporting requirements:

- FR-23 — Incident statistics
- FR-24 — Unresolved incidents
- FR-25 — Escalated incidents
- FR-26 — Support-team / agent performance
- FR-27 — Incident and support reports
- FR-28 — Recurring incident patterns

## 4. Final Requirement Status

| Requirement | Status | Notes |
|---|---|---|
| FR-23 | COMPLETED | Incident statistics endpoint implemented and verified |
| FR-24 | COMPLETED | Unresolved incident endpoint implemented and verified |
| FR-25 | DEFERRED TO M7 | Escalation functionality is part of the later M7 integration decision |
| FR-26 | COMPLETED | Team performance implemented using existing main database relationships |
| FR-27 | CLARIFICATION GAP | Requirement says incident/support reports, but supplied M5 implementation does not define report format, data composition, export, endpoint, or UI. No functionality was invented. |
| FR-28 | COMPLETED | Recurring incident pattern detection implemented and verified |

## 5. Completed FR-23 — Incident Statistics

The existing M5 reporting service was integrated to calculate:

- Total incidents
- Open incidents
- Resolved incidents
- Escalated incidents

Final verified response:

```json
{
  "totalIncidents": 5,
  "openIncidents": 3,
  "resolvedIncidents": 1,
  "escalatedIncidents": 0
}
```

No database schema change was required.

## 6. Completed FR-24 — Unresolved Incidents

The unresolved-incident report was integrated using the existing `Incidents` data.

Final verified result contained four unresolved incidents:

- Incident 5 — Unable to connect to office WiFi
- Incident 4 — wifi not working
- Incident 1 — Laptop not starting
- Incident 2 — Wi-Fi not working

The endpoint correctly excluded the resolved incident.

## 7. Completed FR-26 — Team Performance

The existing M3 `IncidentAssignment` model contains:

```csharp
public int IncidentId { get; set; }
public int AssignedTo { get; set; }
```

The live main database was verified to contain the relationship:

```text
Users.UserId
      ↑
      │
IncidentAssignments.AssignedTo
```

M5 therefore uses the existing main ITServiceDesk database data rather than attempting to join M1's ASP.NET Identity `ApplicationUser.Id`.

The final verified team-performance result was:

```json
[
  {
    "userId": 3,
    "userName": "Arjun Verma",
    "assignedIncidents": 3,
    "resolvedIncidents": 1,
    "openIncidents": 2
  },
  {
    "userId": 4,
    "userName": "Neha Kapoor",
    "assignedIncidents": 1,
    "resolvedIncidents": 0,
    "openIncidents": 1
  }
]
```

### Important implementation decision

`COUNT(DISTINCT IncidentId)` was used for assigned incidents because the actual database contained multiple assignment records for the same incident.

This prevents assignment-row duplication from inflating team-performance statistics.

No duplicate `User` entity was introduced.

No changes were made to M1 Identity.

No changes were made to M3 `IncidentAssignment`.

## 8. FR-25 — Escalation Handling

FR-25 was intentionally not completed as part of the M5 integration.

Reason:

- Escalation/automation work is intended for the later M7 integration.
- The project decision was to avoid pulling M7 functionality into M5.
- The existing M5 escalation endpoint remains a placeholder.
- M7 will later integrate the appropriate escalation/automation functionality.

Therefore:

**FR-25 is a documented M7 dependency, not an M5 integration failure.**

M7 should inspect and integrate escalation functionality without redesigning M5.

## 9. FR-27 — Support Reports

FR-27 states that the IT Manager shall be able to view incident and support reports.

However, the supplied M5 implementation did not define:

- A dedicated report DTO
- A dedicated report service
- A specific report composition
- Report filters
- Date-range reporting
- PDF generation
- Excel generation
- Export requirements
- Report persistence
- A dedicated report UI

Because these details were not specified, the integration did **not** invent a new reporting subsystem.

Existing reporting functionality already provides:

- Incident statistics
- Unresolved incidents
- Team performance

FR-27 is therefore recorded as:

**CLARIFICATION REQUIRED**

The M5 owner/team should clarify what constitutes the required "incident and support reports" before additional functionality is created.

## 10. Completed FR-28 — Recurring Incident Patterns

The M5 teammate handover did not provide an implemented recurring-pattern analysis feature.

The RA nevertheless requires FR-28.

A minimal deterministic implementation was therefore added to M5.

The implementation:

1. Reads existing incidents.
2. Groups incidents by category.
3. Extracts meaningful title keywords.
4. Ignores common stop words.
5. Detects keywords appearing in multiple incidents within the same category.
6. Normalizes `wi-fi` to `wifi`.
7. Returns recurring patterns only when they occur more than once.

No AI/ML model was introduced because the requirement did not specify AI-based pattern detection.

### Final verified result

```json
[
  {
    "categoryId": 3,
    "pattern": "wifi",
    "incidentCount": 3
  }
]
```

The actual database contained:

- Wi-Fi not working
- wifi not working
- Unable to connect to office WiFi

The implementation therefore correctly identified `wifi` as a recurring incident pattern.

## 11. M5 API Surface

The integrated M5 reporting controller now provides the reporting endpoints implemented during this integration:

```text
GET /api/reporting/statistics
GET /api/reporting/unresolved
GET /api/reporting/escalated
GET /api/reporting/team-performance
GET /api/reporting/recurring-patterns
```

Current behavior:

- `/statistics` — working
- `/unresolved` — working
- `/team-performance` — working
- `/recurring-patterns` — working
- `/escalated` — placeholder / deferred to M7

## 12. Files / Areas Changed

The M5 integration retained the teammate's existing reporting structure rather than moving or redesigning it.

Relevant M5 areas include:

```text
AIITSM.Application
└── Reporting
    ├── IReportingService.cs
    ├── IncidentStatisticsDto.cs
    ├── UnresolvedIncidentDto.cs
    ├── EscalatedIncidentDto.cs
    ├── SupportTeamPerformanceDto.cs
    └── RecurringIncidentPatternDto.cs

AIITSM.Infrastructure
└── 05_M5_Reporting
    └── ReportingService.cs

AIITSM.Web
└── Controllers
    └── 05_M5_Reporting
        └── ReportingController.cs
```

The recurring-pattern DTO was added because no existing DTO represented FR-28.

The existing reporting service, interface, and controller were extended rather than replaced.

## 13. Program.cs / Dependency Injection

M5 uses the existing `AIITSMDbContext` and therefore does not require a new database context.

The M5 reporting service must remain registered through dependency injection using the existing reporting contract.

No second M5 database was introduced.

No new authentication system was introduced.

No new Identity configuration was introduced.

## 14. Database Impact

Final database classification:

**Existing tables reused.**

No database schema modification was required.

Specifically:

- `Database.sql` was not modified.
- No M5 migration was required.
- No M5 tables were created.
- No separate reporting database was created.

M5 reads existing operational data.

## 15. Two-Database Architecture Preserved

The project continues to use two database contexts:

### M1 Identity database

```text
ApplicationDbContext
    ↓
ASP.NET Core Identity
    ↓
ApplicationUser / ApplicationRole
```

### Main AI-ITSM database

```text
AIITSMDbContext
    ↓
Incidents
Categories
IncidentComments
Notifications
IncidentAttachments
IncidentFeedback
IncidentAssignments
AIAnalysis
AIAnalysisRelatedIncident
```

M5 reporting uses the existing main operational data.

M5 does not attempt to replace M1 Identity.

## 16. Important Integration Lesson — User IDs

The project contains two user representations:

### M1 Identity

```text
ApplicationUser.Id
string
```

### Main ITServiceDesk database

```text
Users.UserId
int
```

M3 `IncidentAssignment.AssignedTo` is an integer and was verified against the main database's `Users.UserId`.

Therefore M5 team performance correctly uses the main operational user relationship rather than attempting to connect `AssignedTo` directly to `ApplicationUser.Id`.

No duplicate user system was created.

## 17. Regression Safety

The M5 integration did not intentionally modify:

### M1

Identity, authentication, roles, and `ApplicationUser` ownership remain unchanged.

### M2

Incident management entities and existing incident functionality remain unchanged.

### M3

`IncidentAssignment` remains owned by M3.

### M4

Administration remains closed and was not redesigned.

### M6

AI functionality and `AIITSMDbContext` ownership remain unchanged.

## 18. Verification

The final reporting endpoints were tested against the running application.

Verified results:

### Statistics

```text
Total incidents: 5
Open: 3
Resolved: 1
Escalated: 0
```

### Unresolved

```text
4 unresolved incidents
```

### Team Performance

```text
Arjun Verma → 3 assigned / 1 resolved / 2 open
Neha Kapoor → 1 assigned / 0 resolved / 1 open
```

### Recurring Patterns

```text
Category 3 → wifi → 3 incidents
```

The project also successfully built after the M5 implementation steps.

## 19. Architecture Decisions

### Decision 1 — Reuse existing database

M5 uses the existing main IT-ITSM database instead of introducing a reporting database.

### Decision 2 — No duplicate User entity

M5 does not create another user-management model merely for reporting.

### Decision 3 — No AI for recurring-pattern detection

FR-28 does not require AI/ML, so a small deterministic keyword approach was preferred.

### Decision 4 — No invented FR-27 functionality

The vague "support reports" requirement was not expanded into PDF, Excel, export, scheduling, or other functionality without specification.

### Decision 5 — Escalation deferred to M7

M5 does not absorb M7's escalation/automation responsibility.

### Decision 6 — Preserve M4 closure

M4 remains closed. No redesign or reopening was required for M5.

## 20. Known Gaps / Follow-up

### FR-25

To be handled during M7 integration.

### FR-27

Clarification required from the M5 owner/team regarding what constitutes an "incident and support report."

### Reporting UI

The current integration primarily establishes the reporting service/API functionality. A manager-facing dashboard/UI should only be expanded where required by the actual project scope and existing UI integration plan.

## 21. M7 Handoff Notes

M7 is the next integration phase.

M7 should begin from the **current integrated main repository**, not from an earlier M5 snapshot.

M7 must preserve:

```text
M1
M2
M3
M4
M5
M6
```

M7 should particularly inspect:

1. The M5 `/api/reporting/escalated` placeholder.
2. Existing escalation-related database structures, if present in the current database.
3. Existing M3 assignment/workflow contracts.
4. Existing M2 incident lifecycle/status information.
5. Existing M4 administration/manager access.
6. Existing M6 AI functionality where M7 requirements explicitly depend on it.
7. Existing n8n/automation requirements from the project design.
8. M5 reporting integration points that should consume M7 escalation data.

M7 should not rebuild M5 reporting.

If M7 produces escalation records, M5's escalation reporting should consume those records through the agreed contract.

## 22. Recommended M7 Starting Procedure

Before changing M7 code:

1. Inspect the current main repository.
2. Inspect the complete M7 implementation.
3. Inspect M7 requirements/design documents.
4. Inspect existing M1–M6 architecture.
5. Identify M7 dependencies on M2/M3/M4/M5/M6.
6. Identify duplicate functionality.
7. Identify database impact.
8. Determine whether existing escalation structures can be reused.
9. Determine the smallest safe integration.
10. Build/test before proceeding to the next integration step.

The same inspection-first approach used for M5 should be retained for M7.

## 23. Final M5 Status

**M5 Reporting & Monitoring integration is functionally complete for the implemented/defined scope.**

Final state:

```text
FR-23  ✅
FR-24  ✅
FR-25  ⏸ M7
FR-26  ✅
FR-27  ⚠️ Clarification required
FR-28  ✅
```

M5 should now be treated as **CLOSED for integration purposes**, with FR-25 explicitly handed to M7 and FR-27 recorded as a requirements clarification.

## 24. Handoff

Next module:

**M7 — Integration / Automation phase**

M7 work should begin only after creating or confirming the M7 inspection baseline from the current main repository.

Do not reopen M4.

Do not redesign M5.

Do not modify Database.sql unless M7 proves a genuine schema requirement.

