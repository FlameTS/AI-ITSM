# M6 AI — Work Completed So Far

**Project:** AI-Powered IT Service Desk & Incident Management System  
**Module:** M6 — AI Assistance  
**Period:** August 2026

## 1. Purpose

This document records the work completed so far for M6 AI, including the Domain, Application, SQL database alignment, and initial Infrastructure/EF Core work.

## 2. M6 Domain Work

The M6 Domain was designed to contain the concepts owned by the AI Assistance module without database, HTTP, Gemini, controller, UI, or infrastructure-specific code.

Implemented Domain concepts:

- `AIAnalysis`
- `AIAnalysisStatus`
- `AIAnalysisRelatedIncident`
- `AIIncidentRelationshipType`

### AIAnalysis

Represents one AI analysis attempt for an incident.

Current properties:

- `AIAnalysisId`
- `IncidentId`
- `Status`
- `SuggestedCategory`
- `SuggestedPriority`
- `SuggestedResolution`
- `ConfidenceScore`
- `CreatedAt`

### AIAnalysisStatus

The analysis lifecycle is:

```text
Pending
   ↓
Processing
   ↓
Completed

or

Pending
   ↓
Processing
   ↓
Failed
```

AI failure does not invalidate the underlying incident.

### AIAnalysisRelatedIncident

A separate relationship entity was introduced so one AI analysis can identify multiple potentially related or duplicate incidents.

Properties:

- `AIAnalysisRelatedIncidentId`
- `AIAnalysisId`
- `RelatedIncidentId`
- `RelationshipType`
- `SimilarityScore`

### AIIncidentRelationshipType

Current values:

- `Related`
- `Duplicate`

## 3. Application Work

Created:

```text
AIITSM.Application
└── 06_M6_AI
    ├── Contracts
    │   └── AnalyzeIncidentRequest.cs
    └── Services
        └── IAIAnalysisService.cs
```

### AnalyzeIncidentRequest

Input contract contains:

- `IncidentId`
- `Title`
- `Description`

### IAIAnalysisService

The first application operation is:

```text
RequestAnalysis(AnalyzeIncidentRequest request)
        ↓
returns AIAnalysisId
```

The intended first workflow is:

```text
AnalyzeIncidentRequest
        ↓
RequestAnalysis
        ↓
Create AIAnalysis
        ↓
Status = Pending
        ↓
Persist
        ↓
Return AIAnalysisId
```

Actual AI processing/Gemini integration has not been implemented yet.

## 4. SQL Database Alignment

The project uses an existing SQL database created separately according to the team's database design.

The original `AIAnalysis` table contained a single `RelatedIncidentId` and did not contain an analysis status.

The database design was updated to align with the M6 Domain model.

### AIAnalysis

Now contains:

```text
AIAnalysisId
IncidentId
Status
SuggestedCategory
SuggestedPriority
SuggestedResolution
ConfidenceScore
CreatedAt
```

`Status` is stored as `VARCHAR(20)` with a default value of:

```text
Pending
```

The old `RelatedIncidentId` column was removed from `AIAnalysis`.

### AIAnalysisRelatedIncident

A new table was added:

```text
AIAnalysisRelatedIncidentId
AIAnalysisId
RelatedIncidentId
RelationshipType
SimilarityScore
```

Foreign keys connect:

- `AIAnalysisId` → `AIAnalysis`
- `RelatedIncidentId` → `Incidents`

This supports multiple related/duplicate incidents for one AI analysis.

## 5. Infrastructure Work

The Infrastructure project initially contained only module folders and no EF Core database implementation.

The following NuGet packages were installed into `AIITSM.Infrastructure`:

- `Microsoft.EntityFrameworkCore`
- `Microsoft.EntityFrameworkCore.SqlServer`

### AIITSMDbContext

Created:

```text
AIITSM.Infrastructure
└── 06_M6_AI
    └── AIITSMDbContext.cs
```

The context currently exposes:

```text
DbSet<AIAnalysis> AIAnalyses
DbSet<AIAnalysisRelatedIncident> AIAnalysisRelatedIncidents
```

### Entity Configurations

Created:

```text
AIITSM.Infrastructure
└── 06_M6_AI
    └── Configurations
        ├── AIAnalysisConfiguration.cs
        └── AIAnalysisRelatedIncidentConfiguration.cs
```

`AIAnalysisConfiguration` maps:

- table name
- primary key
- enum `Status` as string
- maximum lengths for category/priority
- `ConfidenceScore` as `decimal(5,2)`
- `CreatedAt` default using `GETDATE()`

`AIAnalysisRelatedIncidentConfiguration` maps:

- table name
- primary key
- enum `RelationshipType` as string
- `SimilarityScore` as `decimal(5,2)`

The DbContext uses:

```csharp
modelBuilder.ApplyConfigurationsFromAssembly(
    typeof(AIITSMDbContext).Assembly);
```

This keeps EF Core mapping separate from the Domain classes.

## 6. Validation

The solution was successfully built after:

1. Creating the M6 Domain models.
2. Creating the Application contract and service interface.
3. Updating the SQL design.
4. Installing EF Core packages.
5. Creating the DbContext.
6. Creating EF Core entity configurations.
7. Registering the configurations through `OnModelCreating`.

Current result:

**Build successful.**

## 7. Current Status

```text
M6 Domain                         ✅
Application Contract              ✅
Application Service Interface     ✅
SQL M6 Database Design            ✅
EF Core Packages                  ✅
M6 DbContext                      ✅
EF Core Configurations             ✅
Build                              ✅

Database connection                ⏳
Application Service implementation ⏳
AI provider abstraction            ⏳
Gemini implementation              ⏳
Background processing              ⏳
AI result persistence workflow     ⏳
Web/API integration                ⏳
```

## 8. Next Planned Step

The next step is to configure the existing SQL Server database connection and register `AIITSMDbContext` with the application.

After that, the `IAIAnalysisService` implementation can be created to persist a new `AIAnalysis` with `Pending` status and return its ID.

No Gemini integration should be added until the persistence/request workflow is working.
