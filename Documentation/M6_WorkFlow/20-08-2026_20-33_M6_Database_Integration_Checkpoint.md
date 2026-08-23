# M6 AI — Database Integration Checkpoint

**Date:** 20 August 2026  
**Time:** 20:33 IST  
**Module:** M6 — AI Assistance  
**Project:** AI-Powered IT Service Desk & Incident Management System

---

## 1. Checkpoint Purpose

This checkpoint records the work completed after the initial M6 Domain, Application, and Infrastructure implementation.

The main goal of this stage was to connect the M6 Infrastructure layer to the team's existing SQL Server database and verify that the ASP.NET Core application starts successfully with the database configuration in place.

---

## 2. Database Source

The team maintains the SQL database separately from the C# solution.

A teammate provided the updated:

```text
Database/Database.sql
```

This version contains the M6 database changes required by the current M6 Domain design.

The database created from the script is:

```text
ITServiceDesk
```

---

## 3. M6 Database Structure Verified

The SQL Server database was created locally using SQL Server 2025 Developer Edition and SQL Server Management Studio (SSMS).

The following tables were verified:

```text
AIAnalysis
AIAnalysisRelatedIncident
Categories
Escalations
IncidentAssignments
IncidentComments
Incidents
Notifications
Roles
Users
```

### AIAnalysis

The following columns were verified:

```text
AIAnalysisId
IncidentId
SuggestedCategory
SuggestedPriority
SuggestedResolution
ConfidenceScore
CreatedAt
Status
```

### AIAnalysisRelatedIncident

The table exists and supports multiple related incidents for one AI analysis.

This replaces the earlier single-related-incident approach and aligns the database with the M6 Domain model.

---

## 4. SQL Server Setup

Installed locally:

```text
SQL Server 2025 Developer Edition
SQL Server Management Studio 22
```

SQL Server instance:

```text
localhost
```

Authentication:

```text
Windows Authentication / Trusted Connection
```

Database:

```text
ITServiceDesk
```

The database script was executed successfully enough to create the database and required tables. The script was accidentally executed twice, which produced duplicate-object/default/constraint errors on the second execution. The existing database was then verified directly rather than recreating it.

---

## 5. EF Core Infrastructure

The following packages were installed in:

```text
AIITSM.Infrastructure
```

```text
Microsoft.EntityFrameworkCore
Microsoft.EntityFrameworkCore.SqlServer
```

### AIITSMDbContext

Created:

```text
AIITSM.Infrastructure
└── 06_M6_AI
    └── AIITSMDbContext.cs
```

The context exposes:

```csharp
DbSet<AIAnalysis> AIAnalyses
DbSet<AIAnalysisRelatedIncident> AIAnalysisRelatedIncidents
```

The DbContext provides the EF Core persistence boundary for the M6 entities.

---

## 6. EF Core Entity Configuration

Created:

```text
AIITSM.Infrastructure
└── 06_M6_AI
    └── Configurations
        ├── AIAnalysisConfiguration.cs
        └── AIAnalysisRelatedIncidentConfiguration.cs
```

### AIAnalysisConfiguration

Configured:

- `AIAnalysis` table mapping
- `AIAnalysisId` primary key
- `Status` enum stored as a string
- Status maximum length
- Suggested category length
- Suggested priority length
- `ConfidenceScore` as `decimal(5,2)`
- `CreatedAt` default value using `GETDATE()`

### AIAnalysisRelatedIncidentConfiguration

Configured:

- `AIAnalysisRelatedIncident` table mapping
- Primary key
- `RelationshipType` enum stored as a string
- Relationship type maximum length
- `SimilarityScore` as `decimal(5,2)`

The configurations are registered automatically through:

```csharp
modelBuilder.ApplyConfigurationsFromAssembly(
    typeof(AIITSMDbContext).Assembly);
```

---

## 7. Database Connection Configuration

The Web project's `appsettings.json` was updated with:

```json
"ConnectionStrings": {
  "AIITSMDatabase": "Server=localhost;Database=ITServiceDesk;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

This provides the connection information for the existing local SQL Server database.

---

## 8. Program.cs Registration

The Web project's `Program.cs` was updated to register the M6 DbContext with ASP.NET Core dependency injection.

Added:

```csharp
using AIITSM.Infrastructure._06_M6_AI;
using Microsoft.EntityFrameworkCore;
```

And:

```csharp
builder.Services.AddDbContext<AIITSMDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("AIITSMDatabase")));
```

This establishes the runtime path:

```text
ASP.NET Core Web
        ↓
Dependency Injection
        ↓
AIITSMDbContext
        ↓
EF Core SQL Server Provider
        ↓
localhost
        ↓
ITServiceDesk
```

---

## 9. Runtime Validation

The application was started successfully.

Observed output:

```text
Now listening on: https://localhost:7002
Now listening on: http://localhost:5096

Application started.
Hosting environment: Development
```

This confirms that the Web application starts successfully with the new database configuration and DbContext registration.

The current validation confirms successful application startup. A dedicated database CRUD/query test has not yet been implemented.

---

## 10. Current M6 Status

```text
M6 Domain                         ✅
Application Contract              ✅
IAIAnalysisService Interface      ✅
SQL Database Design               ✅
SQL Server Database               ✅
EF Core Packages                  ✅
AIITSMDbContext                   ✅
EF Core Configurations            ✅
Connection String                 ✅
Program.cs DbContext Registration ✅
Application Startup               ✅

AIAnalysisService Implementation  ⏳
Database CRUD Test                ⏳
AI Provider Abstraction           ⏳
Gemini Integration                ⏳
Background Processing              ⏳
AI Result Persistence Workflow     ⏳
Web/API Integration                ⏳
```

---

## 11. Architecture at This Checkpoint

```text
Domain
  ↓
AIAnalysis
AIAnalysisStatus
AIAnalysisRelatedIncident
AIIncidentRelationshipType
  ↓
Application
  ↓
AnalyzeIncidentRequest
IAIAnalysisService
  ↓
Infrastructure
  ↓
AIITSMDbContext
EF Core Configurations
  ↓
SQL Server
  ↓
ITServiceDesk
```

---

## 12. Why These Changes Were Made

### Domain

Defines the business concepts and lifecycle of an AI analysis without depending on infrastructure technologies.

### Application

Defines the operation the application wants to perform, currently requesting an AI analysis for an incident.

### Interface

Defines the application service contract without committing the application layer to a specific implementation.

### Database

Provides persistent storage for AI analysis results and supports multiple related/duplicate incident candidates.

### DbContext

Provides EF Core's database access boundary for the M6 persistence entities.

### Configuration

Keeps SQL-specific mapping details outside the Domain entities.

### Program.cs

Registers the DbContext with ASP.NET Core dependency injection so application services can obtain it at runtime.

---

## 13. Next Step

The next implementation step is:

```text
Implement AIAnalysisService
        ↓
Receive AnalyzeIncidentRequest
        ↓
Create AIAnalysis
        ↓
Set Status = Pending
        ↓
Save through AIITSMDbContext
        ↓
Return AIAnalysisId
```

After this basic persistence workflow works, we can move toward the AI processing/provider abstraction.

**Do not implement Gemini integration before the basic persistence/request workflow is tested.**
