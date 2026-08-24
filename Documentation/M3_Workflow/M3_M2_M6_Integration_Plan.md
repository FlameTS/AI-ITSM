# M3 → M2 ↔ M6 Integration — Design and Implementation Plan

**Project:** AI-Powered IT Service Management and Incident Resolution Platform (AI-ITSM)
**Module Boundary:** M3 Agent Workflow ↔ M2 Incident Management ↔ M6 AI Assistance
**Workstream:** M3 Extensions + Integration
**Date:** 23-08-2026
**Status:** PLANNING — NOT YET IMPLEMENTED

---

# 1. Purpose

This document defines how to connect the currently-standalone M3 Agent Workflow module to the
real M2 `Incident` entity and to the already-completed M2 ↔ M6 pipeline, without rebuilding M2 or
M6 and without duplicating the `Incident` entity — following the same integration pattern already
used for M2 ↔ M6 (see `M2_M6_Integration.md`).

This is a **plan**, not a completion record. Nothing described here has been implemented yet.

---

# 2. Current State (verified against the repository)

## 2.1 M3 today

```text
AIITSM.Web/Controllers/AgentWorkflowController.cs
AIITSM.Web/Models/AgentWorkflowModel.cs
AIITSM.Web/Views/AgentWorkflow/Index.cshtml
```

- `AgentWorkflowController` holds a single `private static AgentWorkflowModel incident`
  hardcoded to `IncidentId = 1001`.
- `UpdateStatus`, `AssignAgent`, `AddComment` all mutate that one static in-memory object.
- No `DbContext`, no `IIncidentService`, no real `Incident`, no `IncidentComments` reuse.
- The files are **not** placed under the module folders (`AIITSM.Domain/03_M3_AgentWorkflow`,
  `AIITSM.Application/03_M3_AgentWorkflow`, etc.) that every other module uses — those currently
  contain only placeholder `info.md` files.
- This was intentional at the time (per `AGENT_WORKFLOW` notes): M3 was built standalone so it
  didn't block on the database/AI modules, with integration deferred to this stage.

## 2.2 M2 today (relevant surface for M3)

```text
IIncidentService
    CreateIncidentAsync(...)
    GetMyIncidentsAsync(...)
    GetIncidentDetailsAsync(...)
    GetCategoriesAsync(...)
```

`Incident` (Domain):

```text
IncidentId, Title, Description, CategoryId, Category,
Priority, Status, CreatedBy, CreatedAt, ResolvedAt
```

There is **no `AssignedAgentId`** on `Incident` and **no agent-side operations**
(status update, assignment, priority override) anywhere in `IIncidentService`.

`Incidents` table (`Database.sql`) likewise has no assignment column.

`IncidentComments` (M2-2) already exists and is the correct table for comments — it must not be
duplicated by M3's own comment field.

## 2.3 M6 today

`IAIAnalysisService.RequestAnalysis(...)` writes analyses; there is currently **no read-side
contract** for "give me the latest AI analysis for this incident," which M3 will need in order to
show agents the AI's suggested category/priority/resolution (AI-08/AI-09).

---

# 3. Requirement Basis (from Requirement Analysis — Final)

M3's scope is defined by the Help Desk Agent requirements:

| ID | Requirement |
|---|---|
| FR-09 | Agent can view incidents assigned to them |
| FR-10 | Agent can update the status of an incident |
| FR-11 | Agent can assign or reassign incidents where authorized |
| FR-12 | Agent can modify incident priority where authorized |
| FR-13 | Agent can communicate directly with employees |
| FR-14 | Agent can record investigation and resolution details |
| FR-15 | Agent can mark an incident as resolved |
| FR-16 | Agent can close a completed incident |
| FR-17 | Agent can view the full history of an incident |

And the AI-assistance side, from `Final_M6_Workflow.md`:

| ID | Requirement | Depends on |
|---|---|---|
| AI-08 | Authorized personnel can accept AI recommendations | M3 read access to `AIAnalysis` |
| AI-09 | Authorized personnel can override AI recommendations | M3 write to `Incident.Priority`/`CategoryId` |
| AI-10 | AI cannot automatically override human decisions | Enforced by keeping AI output and human decision as separate fields (already true in the domain model) |

FR-13 (agent ↔ employee communication) is **already implemented** by M2-2's
`IncidentComment` / `IIncidentCommentService` — M3 should reuse it, not reinvent it.

---

# 4. Ownership Boundary

```text
M2
 └── owns Incident (identity, title, description, category, employee-set fields)

M3
 └── owns the agent-side actions performed on an existing Incident:
     status transitions, assignment, priority/category override,
     resolution notes, close

M6
 └── owns AIAnalysis and AI provider interaction (unchanged)
```

M3 does **not** create a second Incident record and does **not** fork `IncidentComments` into a
separate comment field. M3 **extends what an authorized agent may do to an M2-owned Incident**,
the same way M6 **consumes** what M2 produces rather than owning it.

```text
Employee (M2)                Agent (M3)                  AI (M6)
Creates Incident      ←→     Views assigned Incidents    Generates AIAnalysis
                              Reads latest AIAnalysis  ←──
                              Updates Status
                              Assigns/Reassigns
                              Overrides Priority/Category
                              Adds Comment (shared IncidentComments)
                              Resolves / Closes
```

---

# 5. Data Contract Changes

## 5.1 `Incident` domain — add one field

```csharp
public int? AssignedAgentId { get; set; }
```

Nullable, matching the existing "Not Assigned" state already modeled in the M3 mockup UI.
No navigation property to `User` is added — same pattern already used for `CreatedBy`
(reference by id only; M1 owns `User`).

## 5.2 `Database.sql` — one additive column, no breaking change

```sql
ALTER TABLE dbo.Incidents
ADD AssignedAgentId INT NULL;
```

No FK constraint to `Users` is added yet, for the same reason `CreatedBy` currently has none —
M1 isn't finalized. This mirrors the existing project convention of deferring FK enforcement
until the owning module is stable.

## 5.3 `IncidentDetailsDto` — extend for the agent view

Add:

```csharp
public int? AssignedAgentId { get; set; }
```

## 5.4 What is explicitly *not* duplicated

- No second `Comment` field on any M3 model — `IncidentComments` (M2-2) is reused.
- No second incident table or DTO fork — `IncidentDetailsDto` is extended, not replaced.
- No new `AIAnalysis`-like table — M3 only *reads* M6's existing table.

---

# 6. Application Layer Changes

## 6.1 Extend `IIncidentService` (owned by M2, used by M3)

```csharp
Task UpdateStatusAsync(int incidentId, IncidentStatus newStatus, int currentUserId, CancellationToken ct = default);

Task AssignAgentAsync(int incidentId, int? agentId, int currentUserId, CancellationToken ct = default);

Task OverridePriorityAsync(int incidentId, IncidentPriority newPriority, int currentUserId, CancellationToken ct = default);
```

These live on `IIncidentService` — not a new `IAgentWorkflowService` — because they mutate the
`Incident` entity that M2 owns. This keeps a single write-path into `Incident`, avoiding two
services racing to update the same row (the same reasoning already applied when M2-2 kept
attachments/comments/feedback as separate services that *reference* `IncidentId` rather than
writing to `Incident` itself — the difference here is these three operations change `Incident`'s
own columns, so they belong on the interface that owns those columns).

## 6.2 New read-only M6 contract for M3 to consume

```csharp
// AIITSM.Application/06_M6_AI/Services/IAIAnalysisService.cs
Task<AIAnalysisSummaryDto?> GetLatestAnalysisAsync(int incidentId, CancellationToken ct = default);
```

`AIAnalysisSummaryDto` exposes `SuggestedCategory`, `SuggestedPriority`, `SuggestedResolution`,
`ConfidenceScore`, `Status` — read-only, no new persistence. This is additive to the existing
`IAIAnalysisService`, not a new abstraction, matching the existing rule that M6's own contracts
should not be forked per consumer.

## 6.3 New M3 Application layer (currently empty placeholders)

```text
AIITSM.Application/03_M3_AgentWorkflow/
├── AgentIncidentDto.cs           (agent-facing incident shape: incident + latest AIAnalysis)
└── IAgentDashboardService.cs     (read-side: "incidents assigned to me" / "unassigned queue")
```

`IAgentDashboardService` is a genuinely new read concern (FR-09, FR-17) — it queries incidents by
`AssignedAgentId` and is not something `IIncidentService` (built for the employee's "My
Incidents") already does.

---

# 7. Infrastructure Layer Changes

```text
AIITSM.Infrastructure/02_M2_IncidentManagement/IncidentService.cs
    → implement UpdateStatusAsync / AssignAgentAsync / OverridePriorityAsync

AIITSM.Infrastructure/06_M6_AI/Services/AIAnalysisService.cs
    → implement GetLatestAnalysisAsync (order by CreatedAt/completion, Status = Completed)

AIITSM.Infrastructure/03_M3_AgentWorkflow/
    └── AgentDashboardService.cs   (implements IAgentDashboardService against AIITSMDbContext)
```

No new `DbContext` and no new EF configuration classes are required beyond adding the
`AssignedAgentId` column mapping to the existing `IncidentConfiguration.cs` — the shared
`AIITSMDbContext` is reused, same as every other M2-2 feature.

---

# 8. Web Layer Changes

```text
AIITSM.Web/Controllers/03_M3_AgentWorkflow/AgentWorkflowController.cs   (moved + rebuilt)
AIITSM.Web/Views/AgentWorkflow/Index.cshtml                            (bound to real data)
```

`AgentWorkflowController` is moved into the `03_M3_AgentWorkflow` folder (matching every other
module) and rebuilt to:

1. Resolve the current agent via the existing `ICurrentUserService` (same mechanism M2-2 uses)
   instead of a hardcoded static object.
2. Call `IAgentDashboardService` for the incident list / details.
3. Call `IAIAnalysisService.GetLatestAnalysisAsync` to show the AI suggestion alongside the
   employee-set values (mirrors §11 of `M2_M6_Integration.md` — AI suggestion and human-approved
   value are shown as **separate** fields, never merged silently).
4. Call `IIncidentService.UpdateStatusAsync` / `AssignAgentAsync` / `OverridePriorityAsync` for
   the POST actions, replacing the static-object mutation.
5. Call the existing `IIncidentCommentService.AddCommentAsync` for FR-13, instead of overwriting
   a single `Comment` string.
6. On status change to `Resolved`, call the existing `INotificationService.CreateNotificationAsync`
   so the employee gets the "Your incident has been resolved" notification already proven in
   `M2_M6_Integration.md` §14 Test 4 — no new notification mechanism is introduced.

All POST actions keep `[ValidateAntiForgeryToken]`, consistent with the existing pattern.

---

# 9. Failure Isolation

Same principle as M2 ↔ M6:

```text
Agent action (status/assign/priority/comment)
            ↓
     Incident write succeeds?
        /            \
      YES             NO
       ↓               ↓
  Persist +        Return error,
  optional          Incident and
  notification      AIAnalysis remain
                     untouched
```

An agent action must never retroactively invalidate an `AIAnalysis` record — accepting or
overriding an AI suggestion changes `Incident.Priority`/`CategoryId`, it does not delete or
rewrite the `AIAnalysis` row, per the already-approved "AI analysis history is separate from the
human decision" rule (`Final_M6_Workflow.md` §5).

---

# 10. Authorization Note

M1 (Identity/Access) is not finalized, so "where authorized" (FR-11, FR-12) cannot yet be
enforced with real roles. Until M1 lands, the plan is to use `ICurrentUserService` the same way
M2-2 does — resolve *who* is acting, but not yet gate *what role* they hold. This should be
called out explicitly in the M3 checkpoint documentation as a known limitation, the same way
`M2_M6_Integration.md` §19 explicitly flagged the untested AI-provider-failure path rather than
silently claiming full coverage.

---

# 11. Implementation Sequence

```text
1. Add AssignedAgentId column (Database.sql, non-breaking)
        ↓
2. Extend Incident domain + IncidentConfiguration
        ↓
3. Extend IIncidentService / IncidentService with the 3 agent-write operations
        ↓
4. Add GetLatestAnalysisAsync to IAIAnalysisService / AIAnalysisService
        ↓
5. Create 03_M3_AgentWorkflow Application + Infrastructure services
        ↓
6. Move + rebuild AgentWorkflowController against real services
        ↓
7. Wire IncidentComment + Notification reuse into the agent view
        ↓
8. Register new services in Program.cs
        ↓
9. End-to-end test (see §12)
        ↓
10. Update M3 checkpoint documentation
```

---

# 12. Planned End-to-End Test

Using the same real incident already proven in the M2 ↔ M6 checkpoint:

```text
IncidentId: 5
Incident Number: INC-000005
Title: Unable to connect to office WiFi
Current AIAnalysis: SuggestedCategory = Network / Wi-Fi, SuggestedPriority = Low
```

Planned verification:

1. Agent Workflow index shows Incident #5 with the employee-set values (Category: Network,
   Priority: Medium) **and** the AI suggestion (Network / Wi-Fi, Low) shown separately.
2. Agent assigns themselves to Incident #5 → `AssignedAgentId` persists → visible after refresh.
3. Agent updates status to `In Progress` → persists → employee's Notifications page is
   unaffected (no notification expected here per current design — only Resolved triggers one).
4. Agent adds a comment → appears in the same `IncidentComments` thread the employee sees on
   their Incident Details page (proves no duplicate comment store was introduced).
5. Agent overrides priority to `Low` (accepting the AI suggestion) → `Incident.Priority` changes
   → the original `AIAnalysis` row for Incident #5 remains unchanged in SQL Server.
6. Agent marks Incident #5 `Resolved` → employee's Notifications page shows a new resolution
   notification, reusing the existing `INotificationService` path.
7. Regression: re-run the M2 ↔ M6 end-to-end test (`M2_M6_Integration.md` §10) to confirm
   nothing in the M2 → M6 path broke.

---

# 13. Scope Boundary

```text
In scope for this integration:
- AssignedAgentId on Incident
- Agent status/assignment/priority operations on IIncidentService
- Read-only AI analysis access for M3
- Reuse of existing IncidentComments and Notifications
- Moving M3 files into the 03_M3_AgentWorkflow module structure

Out of scope:
- M1 role-based authorization (until M1 is finalized)
- AI-05 (related/duplicate detection) — still waiting on M2 incident history design
- AI-06 (conversation summarization) — depends on this M3 integration being complete first
- AI-07 (support assistant)
- Background/queued AI processing
- Any change to the Gemini provider or AIAnalysis schema
```

No unrelated M1, M4, M5, or M7 functionality is touched.

---

# 14. Final Status

```text
M3 Standalone Module
COMPLETE (per AGENT_WORKFLOW notes)

M3 → M2 ↔ M6 Integration
PLANNED — NOT STARTED

Next action: implement §11 Step 1 (Database.sql column addition)
```
