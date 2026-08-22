# M2 Incident Management — Work Log

**Module:** M2 — Incident Management
**Scope covered:** Incident Domain, Incident creation, My Incidents, Incident Details, core testing.
**Not covered (owned by teammate):** Comments/communication, Attachments, Feedback, Notifications, M6 integration.

## Domain (`AIITSM.Domain/02_M2_IncidentManagement`)

- `Incident.cs` — Title, Description, CategoryId, Priority, Status, CreatedBy, CreatedAt, ResolvedAt.
- `IncidentStatus.cs` — Open, InProgress, Resolved, Closed.
- `IncidentPriority.cs` — Low, Medium, High, Critical.
- `Category.cs` — CategoryId, CategoryName.

### Assumptions / things to flag with the team

- **Category didn't exist as an entity anywhere yet** (only the
  `Categories` table in `Database.sql`). Incident creation needs a
  CategoryId, so `Category.cs` was added here. Happy to move it if the
  team wants a dedicated module for it.
- **No User entity was created.** `Incident.CreatedBy` is a plain `int`
  (matches `Incidents.CreatedBy` FK in the DB), same pattern M6 used for
  `IncidentId` — reference by id, don't take ownership of another
  module's entity.
- **`ICurrentUserService`** (`AIITSM.Application/Common`) is a small
  abstraction for "who is the logged-in employee". Since M1
  (Identity/Access) hasn't been built, `AIITSM.Web/Common/DemoCurrentUserService.cs`
  is a temporary stub that always returns UserId = 1. Swap that one file
  out once real login exists — nothing else needs to change.
- **Incident number** (e.g. `INC-000042`) is not a stored column — it's
  formatted from the existing `IncidentId` identity column on read, to
  avoid a second source of truth.

## Application (`AIITSM.Application/02_M2_IncidentManagement` + `Common`)

- `IIncidentService` — CreateIncidentAsync, GetMyIncidentsAsync,
  GetIncidentDetailsAsync, GetCategoriesAsync.
- `CreateIncidentRequest`, `IncidentSummaryDto`, `IncidentDetailsDto`,
  `CategoryDto`.
- `ICurrentUserService` (in `Common`, since it isn't owned by any one
  module).

## Infrastructure (`AIITSM.Infrastructure/02_M2_IncidentManagement`)

- `IncidentConfiguration`, `CategoryConfiguration` — EF Fluent API,
  same enum-to-string pattern M6 used.
- `IncidentService` — implements `IIncidentService` against the shared
  `AIITSMDbContext`.
- `AIITSMDbContext` updated with `DbSet<Incident>` and `DbSet<Category>`
  (only DbContext file in the solution — shared with M6, no new context
  created).

## Web (`AIITSM.Web/Controllers/02_M2_IncidentManagement`)

- `IncidentController` (MVC, matches `AgentWorkflowController` style):
  - `GET /Incident` — My Incidents (filtered to the logged-in employee).
  - `GET /Incident/Create`, `POST /Incident/Create` — Status is forced to
    Open and CreatedBy to the logged-in employee server-side; never
    trusted from the form.
  - `GET /Incident/Details/{id}` — incident info + current status.
- Views: `Views/Incident/Create.cshtml`, `Index.cshtml`, `Details.cshtml`.

## Testing (`AIITSM.Tests`, new project — added to `AIITSM.slnx`)

xUnit + EF Core InMemory provider. Covers:

- New incident gets Status = Open.
- New incident gets CreatedBy = the logged-in employee's id.
- "My Incidents" only returns the calling employee's own incidents.
- Incident number is formatted correctly from IncidentId.
- Looking up a non-existent incident returns null (controller turns
  this into a 404).

## Still open / for discussion with the team

- `Incident Details` currently doesn't restrict viewing to the incident's
  creator — only "My Incidents" is scoped that way, per the given
  requirements. Worth confirming whether agents/other roles will need
  Details access once M1 roles exist.
- Connection string in `appsettings.json` is still the placeholder
  `YOUR_CONNECTION_STRING_HERE` — needs to be set before this can run
  against a real SQL Server instance.
