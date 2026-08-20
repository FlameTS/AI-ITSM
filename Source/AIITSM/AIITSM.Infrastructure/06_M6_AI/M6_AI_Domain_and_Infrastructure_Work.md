# M6 AI — Domain & Infrastructure Work Log

**Project:** AI-Powered IT Service Desk & Incident Management System  
**Module:** M6 — AI Assistance  
**Period:** August 2026

## Part A — Domain

### Domain responsibility

M6 Domain contains concepts that describe what an AI analysis is. It intentionally does not contain:

- Gemini/API calls
- API keys
- HTTP clients
- JSON/API-specific code
- Entity Framework Core configuration
- Database connection code
- Controllers
- UI code
- Background worker implementation
- Python/Jupyter code

### Implemented files

```text
AIITSM.Domain
└── 06_M6_AI
    ├── AIAnalysis.cs
    ├── AIAnalysisRelatedIncident.cs
    ├── AIAnalysisStatus.cs
    └── AIIncidentRelationshipType.cs
```

### AIAnalysis.cs

Represents one AI analysis attempt.

Properties:

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

### AIAnalysisStatus.cs

```text
Pending
Processing
Completed
Failed
```

This represents the lifecycle of one AI analysis attempt.

### AIAnalysisRelatedIncident.cs

Represents a possible relationship between an AI analysis and an existing incident.

Properties:

```text
AIAnalysisRelatedIncidentId
AIAnalysisId
RelatedIncidentId
RelationshipType
SimilarityScore
```

The separate entity supports multiple related incidents per analysis.

### AIIncidentRelationshipType.cs

Current values:

```text
Related
Duplicate
```

### Domain ownership decision

M6 owns the AI concepts.

M6 does not own:

- Incident
- User
- Role
- IncidentComment
- IncidentStatusHistory

M6 references existing incidents using `IncidentId` rather than creating a duplicate Incident entity.

---

# Part B — Infrastructure

## Initial state

The Infrastructure project had the M6 module folder but no EF Core implementation.

```text
AIITSM.Infrastructure
└── 06_M6_AI
    └── info.md
```

## EF Core packages

Installed into `AIITSM.Infrastructure`:

```text
Microsoft.EntityFrameworkCore
Microsoft.EntityFrameworkCore.SqlServer
```

These provide EF Core and SQL Server provider support.

## AIITSMDbContext.cs

Created:

```text
AIITSM.Infrastructure
└── 06_M6_AI
    └── AIITSMDbContext.cs
```

The context contains:

```csharp
public DbSet<AIAnalysis> AIAnalyses { get; set; }
public DbSet<AIAnalysisRelatedIncident> AIAnalysisRelatedIncidents { get; set; }
```

The context is responsible for exposing the M6 persistence entities to EF Core.

## Configuration files

Created:

```text
AIITSM.Infrastructure
└── 06_M6_AI
    └── Configurations
        ├── AIAnalysisConfiguration.cs
        └── AIAnalysisRelatedIncidentConfiguration.cs
```

### AIAnalysisConfiguration

Responsibilities:

- Maps `AIAnalysis` to `AIAnalysis`.
- Configures `AIAnalysisId` as the key.
- Converts `AIAnalysisStatus` enum to a string.
- Sets `Status` maximum length to 20.
- Configures category length as 100.
- Configures priority length as 50.
- Maps confidence score to `decimal(5,2)`.
- Configures `CreatedAt` default to `GETDATE()`.

### AIAnalysisRelatedIncidentConfiguration

Responsibilities:

- Maps the entity to `AIAnalysisRelatedIncident`.
- Configures its primary key.
- Converts `AIIncidentRelationshipType` enum to a string.
- Sets relationship type maximum length to 20.
- Maps similarity score to `decimal(5,2)`.

## Configuration registration

`AIITSMDbContext` uses:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    modelBuilder.ApplyConfigurationsFromAssembly(
        typeof(AIITSMDbContext).Assembly);
}
```

This allows EF Core to automatically discover the M6 configuration classes.

## Why this separation was used

The Domain entities remain free of EF Core-specific configuration.

Infrastructure owns the persistence mapping.

Conceptually:

```text
Domain
  ↓
Defines AI concepts

Infrastructure
  ↓
Defines how those concepts are stored

SQL Server
  ↓
Stores the actual data
```

## Validation

After the Infrastructure work, the solution was built successfully.

Current Infrastructure status:

```text
EF Core packages          ✅
AIITSMDbContext           ✅
AIAnalysis mapping        ✅
Related incident mapping  ✅
Configuration discovery   ✅
Build                     ✅

Connection configuration  ⏳
Database runtime test     ⏳
```

## Next Infrastructure step

Configure the connection to the existing SQL Server database and register `AIITSMDbContext` with the application.

Only after that should the M6 application service persist `AIAnalysis` records.
