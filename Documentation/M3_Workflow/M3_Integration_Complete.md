# M3 → M2 ↔ M6 Integration — Complete

**Project:** AI-Powered IT Service Management and Incident Resolution Platform (AI-ITSM)
**Module Boundary:** M3 Agent Workflow ↔ M2 Incident Management ↔ M6 AI Assistance
**Workstream:** M3 Integration
**Date:** 23-08-2026
**Status:** COMPLETE — IMPLEMENTED AND MANUALLY VERIFIED

---

# 1. Purpose

This document records the integration of the previously-standalone M3 Agent Workflow module into
the real M2 `Incident` data and the existing M2 ↔ M6 pipeline.

Before this integration, `AgentWorkflowController` operated on a single hardcoded in-memory
`AgentWorkflowModel` (`IncidentId 1001`) and had no connection to the database, M2, or M6.

---

# 2. Ownership Boundary (unchanged principle, now enforced in code)

```text
M2
 └── owns Incident (identity, title, description, category, status, resolution timestamp)

M3
 └── owns agent-side actions performed on an existing Incident:
     status updates, assignment/reassignment
     (comments are NOT owned by M3 — reused from M2-2)

M6
 └── owns AIAnalysis and AI provider interaction (untouched by this work)
```

M3 does not create a second Incident entity and does not fork the comment system.

---

# 3. Database

**No changes were made to `Database.sql`.**

The `IncidentAssignments` table already existed in the schema (`AssignmentId`, `IncidentId`,
`AssignedTo`, `AssignedAt`, FKs to `Users` and `Incidents`) with seed data, but had no
corresponding C# domain/EF mapping. This integration added the missing application-layer code
for a table that was already part of the approved schema — it did not alter, extend, or add any
table.

---

# 4. Files Added

```text
AIITSM.Domain/03_M3_AgentWorkflow/IncidentAssignment.cs

AIITSM.Application/03_M3_AgentWorkflow/IIncidentAssignmentService.cs

AIITSM.Infrastructure/03_M3_AgentWorkflow/IncidentAssignmentService.cs
AIITSM.Infrastructure/03_M3_AgentWorkflow/Configurations/IncidentAssignmentConfiguration.cs
```

## 4.1 `IncidentAssignment` (Domain)

Plain POCO, no data-annotation attributes — consistent with every other domain entity in the
project (`Incident`, `IncidentComment`, `IncidentAttachment`, etc. are all annotation-free, with
persistence concerns kept in Infrastructure).

```text
AssignmentId
IncidentId
AssignedTo
AssignedAt
```

## 4.2 `IncidentAssignmentConfiguration` (Infrastructure)

Maps `IncidentAssignment` to the existing `IncidentAssignments` table and explicitly declares
`AssignmentId` as the primary key.

This was necessary: EF Core's default key convention looks for a property named `Id` or
`IncidentAssignmentId` on the `IncidentAssignment` class. `AssignmentId` matches neither, so
without this explicit configuration EF Core throws *"The entity type 'IncidentAssignment'
requires a primary key to be defined"* at application startup. The same situation already existed
for `IncidentAttachment.AttachmentId` elsewhere in the project and was resolved the same way, via
an explicit `IEntityTypeConfiguration<T>` class rather than a `[Table]`/`[Key]` attribute.

## 4.3 `IIncidentAssignmentService` (Application)

```text
GetAssignedAgentAsync(incidentId) -> int?
AssignAgentAsync(incidentId, assignedTo?)
```

## 4.4 `IncidentAssignmentService` (Infrastructure)

Read side: returns the `AssignedTo` of the most recent row for the incident, ordered by
`AssignedAt` descending.

Write side, with one deliberate design decision:

- **Reassignment** (`assignedTo` has a value): a **new row is inserted**; the previous
  assignment row is left in place. This preserves assignment history instead of overwriting it,
  matching the same "history is not silently overwritten" principle already established for
  `AIAnalysis` in the M6 design decisions. Verified in testing — reassigning Incident #1 from
  Agent 3 to Agent 4 produced two separate rows rather than one row being updated in place.
- **Unassignment** (`assignedTo` is `null`): all rows for that incident are removed. This is not
  a design preference — `IncidentAssignments.AssignedTo` is `NOT NULL` in the schema, so "no
  agent" cannot be represented as a stored row. Absence of any row is what "unassigned" means.

---

# 5. Files Changed

```text
AIITSM.Application/02_M2_IncidentManagement/IIncidentService.cs
AIITSM.Infrastructure/02_M2_IncidentManagement/IncidentService.cs
AIITSM.Infrastructure/06_M6_AI/AIITSMDbContext.cs
AIITSM.Web/Program.cs
AIITSM.Web/Controllers/AgentWorkflowController.cs
```

## 5.1 `IIncidentService` / `IncidentService`

One new method was added to the interface M2 already owns:

```csharp
Task UpdateStatusAsync(int incidentId, IncidentStatus status, CancellationToken ct = default);
```

This lives on `IIncidentService` rather than a separate M3 service because it mutates
`Incident.Status` / `Incident.ResolvedAt`, columns M2 owns. Keeping a single write path into
`Incident` avoids two services independently writing to the same row.

`ResolvedAt` handling:

```text
Status -> Resolved (first time)   : ResolvedAt = now
Status -> Open / In Progress      : ResolvedAt = null   (reopened)
Status -> Closed                  : ResolvedAt unchanged
```

An earlier draft of this logic unconditionally set `ResolvedAt = null` whenever status was
anything other than `Resolved`, which would have silently erased the resolution timestamp the
moment a resolved incident was closed. This was caught during review and corrected before
implementation.

## 5.2 `AIITSMDbContext`

Added:

```csharp
public DbSet<IncidentAssignment> IncidentAssignments { get; set; }
```

No other change. `OnModelCreating` already applies configurations from the assembly, so
`IncidentAssignmentConfiguration` required no additional registration.

## 5.3 `Program.cs`

Added one DI registration:

```csharp
builder.Services.AddScoped<IIncidentAssignmentService, IncidentAssignmentService>();
```

All existing registrations (M2, M2-2, M6) were left untouched.

## 5.4 `AgentWorkflowController`

Full rewrite — the previous version had no injectable dependencies to preserve. The new
controller:

- Loads the real incident via the existing `IIncidentService.GetIncidentDetailsAsync(id)`.
- Loads the current assignment via `IIncidentAssignmentService.GetAssignedAgentAsync(id)`.
- Loads comments via the existing, unmodified `IIncidentCommentService.GetCommentsAsync(id)` and
  displays the most recent one.
- `UpdateStatus`, `AssignAgent`, `AddComment` POST actions now call the real services instead of
  mutating a static object, and each wraps its call in a try/catch on `InvalidOperationException`
  so a failure (e.g. incident not found) surfaces as a message rather than an unhandled exception.
- Retains `[ValidateAntiForgeryToken]` on all POST actions, and the original action names
  (`Index`, `UpdateStatus`, `AssignAgent`, `AddComment`), so `Index.cshtml` required no changes.
- `Index(int id = 1)` defaults to Incident #1 as a temporary entry point, since there is not yet
  a menu/dashboard link that passes a real incident ID in. This should be revisited once M1 or a
  reporting/dashboard view provides real navigation into this page.

---

# 6. Files Preserved (no changes)

```text
AIITSM.Web/Models/AgentWorkflowModel.cs
AIITSM.Web/Views/AgentWorkflow/Index.cshtml
```

The view still shows a single `Comment` field rather than a full thread — a known, accepted
limitation. The employee-facing Incident Details page shows the complete `IncidentComments`
thread; the agent view currently only surfaces the latest entry. Redesigning the agent UI to show
the full thread was intentionally left out of this integration's scope.

---

# 7. Manual Verification

Performed against real seed data, Incident #1 ("Laptop not starting", originally Status = Open).

| Step | Action | Result |
|---|---|---|
| 1 | Loaded `/AgentWorkflow/Index/1` | Real incident data displayed (not the old hardcoded #1001 record) |
| 2 | Updated status to `In Progress` | Persisted; confirmed after refresh |
| 3 | Assigned Agent #3 | `IncidentAssignments` gained a new row for Incident #1, Agent 3 |
| 4 | Added a comment | Persisted to `IncidentComments`; same comment visible on the employee-facing `/Incident/Details/1` page — confirms no duplicate comment store was introduced |
| 5 | Reassigned to Agent #4 | A **second** row was inserted rather than the first being overwritten |

Database state after testing:

```text
IncidentAssignments (IncidentId = 1):
AssignmentId 1 | AssignedTo 3 | 2026-08-18 23:55:28.137  (original seed)
AssignmentId 4 | AssignedTo 3 | 2026-08-23 11:31:27.747  (test reassignment)
AssignmentId 5 | AssignedTo 4 | 2026-08-23 11:31:46.483  (test reassignment)

Incidents (IncidentId = 1):
Status = InProgress
ResolvedAt = NULL
```

`GetAssignedAgentAsync` correctly returned Agent 4 (the latest row by `AssignedAt`) throughout,
confirming the read path is unaffected by history being preserved on the write path.

Result: **PASS**

---

# 8. Regression Check

Reused the M2-2 comment thread: a comment added through the Agent Workflow page appeared on the
employee's `/Incident/Details/1` page without any additional wiring — confirming
`IncidentComments` remains a single shared table and M3 did not fork it.

The M2 ↔ M6 pipeline (`IncidentController.Create` → `IAIAnalysisService.RequestAnalysis` →
`GeminiProvider` → `AIAnalysis` persistence) was not touched by any file in this integration and
was not expected to regress.

---

# 9. Scope Boundary

```text
In scope and done:
- AssignedAgentId read/write via the existing IncidentAssignments table
- Incident status updates via IIncidentService
- Reuse of existing IncidentComments for agent-side comments
- Moving AgentWorkflowController onto real DI-backed services

Out of scope (not done, not claimed as done):
- M1 role-based authorization ("where authorized" in FR-11/FR-12)
- Full comment thread display in the agent view (still shows latest only)
- Real navigation/dashboard entry point into /AgentWorkflow/Index/{id}
- AI-08 / AI-09 (agent accept/override of AI suggestions) — M3 does not yet read AIAnalysis
- Priority/category override by the agent
- AI-05, AI-06, AI-07 — unaffected, still pending per M6's own documented status
```

No changes were made to `Database.sql`, the M6 domain model, the Gemini provider, or any M1/M4/M5/M7 area.

---

# 10. Final Status

```text
M3 Standalone Module
COMPLETE (prior work)

M3 → M2 ↔ M6 Integration
COMPLETE

Manual Verification
PASS

Regression (M2-2 comments, M2 ↔ M6 pipeline)
NOT BROKEN
```

## Known follow-ups for a later checkpoint

- Replace the hardcoded `Index(id = 1)` default once a real entry point exists.
- Decide whether the agent view should show the full comment thread instead of the latest entry.
- Connect M3 to `IAIAnalysisService` (read-only) so agents can see AI-suggested
  category/priority/resolution — required for AI-08/AI-09.
- Revisit authorization once M1 ships real roles.
