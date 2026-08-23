# M6 AI — Work Log & Blueprint Progress

**Project:** AI-Powered IT Service Desk & Incident Management System (AI-ITSM)  
**Module:** M6 — AI Assistance  
**Status:** Working Document  
**Version:** 0.1  
**Date:** August 2026

## 1. Purpose

This document records the M6 AI work completed so far: approved decisions, implemented concepts, current design direction, unresolved questions, and the next step.

It is a working project reference and may evolve as implementation reveals new requirements.

## 2. Project Architecture

The project uses a **Layered Modular Monolith** with:

```text
AIITSM
├── AIITSM.Application
├── AIITSM.Domain
├── AIITSM.Infrastructure
└── AIITSM.Web
```

Functional modules:

```text
01_M1_IdentityAccess
02_M2_IncidentManagement
03_M3_AgentWorkflow
04_M4_Administration
05_M5_Reporting
06_M6_AI
07_M7_Automation
```

M6 exists in:

```text
AIITSM.Application/06_M6_AI
AIITSM.Domain/06_M6_AI
AIITSM.Infrastructure/06_M6_AI
AIITSM.Web/Controllers/06_M6_AI
```

The M6 folders initially contained `info.md` placeholders.

## 3. Seven-Member Module Isolation

M6 is being developed as a self-contained module so that parallel development does not unnecessarily disturb other members.

M6 should:

- Own AI functionality.
- Avoid duplicating entities owned by other modules.
- Avoid modifying another member's internal implementation without discussion.
- Use agreed contracts/integration points when modules are connected.
- Remain predominantly C# / ASP.NET Core.

The eventual wiring is expected to look conceptually like:

```text
M2 ──────► M6
M3 ──────► M6
M6 ──────► M5
M6 ──────► M7
```

M2 is currently not implemented, so M6 can continue internally and later integrate with M2.

## 4. AI Requirements

| ID | Requirement | Current Direction |
|---|---|---|
| AI-01 | Analyze newly submitted incident descriptions | LLM-based analysis |
| AI-02 | Suggest category | LLM classification; custom ML may be evaluated |
| AI-03 | Suggest priority/severity | LLM classification/reasoning; custom ML may be evaluated |
| AI-04 | Suggest possible resolutions | LLM reasoning/generation |
| AI-05 | Identify related/duplicate incidents | Embeddings/similarity + possible LLM verification |
| AI-06 | Summarize lengthy incident conversations | LLM summarization |
| AI-07 | AI assistant for common IT support queries | LLM-based assistant |
| AI-08 | Authorized personnel can accept recommendations | C# business logic |
| AI-09 | Authorized personnel can override recommendations | C# business logic |
| AI-10 | AI cannot override human decisions | C# authorization/business rules |

## 5. Technology Principle

Production implementation should be predominantly:

- C#
- ASP.NET Core
- Existing modular-monolith architecture
- Existing database

Python/Jupyter may be used for genuine supporting work such as:

- Dataset analysis
- ML experimentation
- Model training/evaluation
- Similarity/embedding experiments

Python is not automatically part of the production architecture.

## 6. Background AI Processing

Agreed workflow:

```text
Employee
   |
   v
Submit Incident
   |
   v
Save Incident
   |
   +--------------------> Return success
   |
   v
AI Analysis Job
   |
   v
Background AI Processing
   |
   v
Validate AI Result
   |
   v
Persist AI Analysis
   |
   v
Human Review
```

If AI fails, the incident remains valid and usable.

The exact background-processing technology is not finalized.

## 7. Decision 01 — AI Analysis History

**Approved decision:** An incident can have multiple AI analysis records over its lifecycle.

```text
Incident #1042
    ├── AI Analysis #1
    ├── AI Analysis #2
    └── AI Analysis #3
```

Reasons may include re-analysis, new information, retries, or changes in analysis strategy.

An AI analysis is therefore treated as a historical analysis attempt rather than simply overwriting the previous result.

## 8. Decision 02 — AI Analysis Lifecycle

**Approved decision:** Each AI analysis represents one analysis attempt with a processing lifecycle:

```text
Pending
   |
   v
Processing
   |
   +----------------+
   |                |
   v                v
Completed         Failed
```

A failed analysis must not invalidate the underlying incident.

## 9. Initial M6 Domain Model

The first domain concept is `AIAnalysis`.

Its current responsibilities are:

```text
AIAnalysis
├── Identity
├── Incident reference
├── Processing status
├── AI recommendations
├── Related-incident information
├── Confidence information
└── Creation timestamp
```

The design intentionally starts small rather than creating unnecessary classes.

## 10. Current `AIAnalysis`

File:

```text
AIITSM.Domain
└── 06_M6_AI
    └── AIAnalysis.cs
```

Current intended structure:

```csharp
namespace AIITSM.Domain._06_M6_AI
{
    public class AIAnalysis
    {
        public int AIAnalysisId { get; set; }
        public int IncidentId { get; set; }
        public AIAnalysisStatus Status { get; set; }
        public string? SuggestedCategory { get; set; }
        public string? SuggestedPriority { get; set; }
        public string? SuggestedResolution { get; set; }
        public int? RelatedIncidentId { get; set; }
        public decimal? ConfidenceScore { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
```

`SuggestedCategory` and `SuggestedPriority` remain strings for now because M6 has not finalized their project-wide allowed values.

`ConfidenceScore` is nullable because pending/failed analysis may not have a result. Its exact meaning and scale remain open.

## 11. Current `AIAnalysisStatus`

File:

```text
AIITSM.Domain
└── 06_M6_AI
    └── AIAnalysisStatus.cs
```

Current intended implementation:

```csharp
namespace AIITSM.Domain._06_M6_AI
{
    public enum AIAnalysisStatus
    {
        Pending,
        Processing,
        Completed,
        Failed
    }
}
```

## 12. AI-05 — Related / Duplicate Incidents

The original Database v1 design used a single:

```text
AIAnalysis.RelatedIncidentId
```

This only supports one related incident.

AI-05 can require multiple candidates, so the M6 design is being expanded.

Current concept:

```text
AIAnalysis
      |
      v
AIAnalysisRelatedIncident
      ├── AIAnalysisRelatedIncidentId
      ├── AIAnalysisId
      ├── RelatedIncidentId
      ├── RelationshipType
      └── SimilarityScore
```

Example:

```text
AIAnalysis #25
    ├── Incident #101 → Related → 0.81
    ├── Incident #108 → Duplicate → 0.94
    └── Incident #117 → Related → 0.73
```

This supports multiple related incidents while preserving relationship information.

## 13. Current `AIAnalysisRelatedIncident`

File:

```text
AIITSM.Domain
└── 06_M6_AI
    └── AIAnalysisRelatedIncident.cs
```

The current implementation is:

```csharp
namespace AIITSM.Domain._06_M6_AI
{
    public class AIAnalysisRelatedIncident
    {
        public int AIAnalysisRelatedIncidentId { get; set; }

        public AIAnalysis AIAnalysisId { get; set; }

        public int RelatedIncidentId { get; set; }

        public string? RelationshipType { get; set; }

        public double SimilarityScore { get; set; }
    }
}
```

### Code review correction required

This property:

```csharp
public AIAnalysis AIAnalysisId { get; set; }
```

uses the `AIAnalysis` object where the property name indicates an ID.

The initial correction should be:

```csharp
public int AIAnalysisId { get; set; }
```

A navigation property can be considered later when EF Core relationships are implemented:

```csharp
public AIAnalysis AIAnalysis { get; set; }
```

Do not add the navigation property yet unless the next persistence design requires it.

## 14. Relationship Type

Current working values:

```text
Related
Duplicate
```

The final representation (string/enum/etc.) is not yet finalized.

## 15. Similarity Score

`SimilarityScore` represents how strongly a candidate incident resembles the incident being analyzed.

Example:

```text
0.94 → very strong similarity
0.71 → moderate similarity
```

It must **not** currently be described as a probability of duplication.

The exact scale, threshold, and calculation method remain open.

Potential future pipeline:

```text
New Incident
     |
     v
Embedding
     |
     v
Similarity Search
     |
     v
Candidate Incidents
     |
     v
LLM Verification
     |
     v
Final Relationship Assessment
```

Python/Jupyter may be used to experiment with similarity methods and thresholds.

## 16. Database Direction

Database v1 is under review and may be changed as M6 discovers justified requirements.

The single:

```text
AIAnalysis.RelatedIncidentId
```

is considered insufficient for the intended AI-05 functionality.

Current working database direction:

```text
AIAnalysis
     |
     +── AIAnalysisRelatedIncident
              ├── RelatedIncidentId
              ├── RelationshipType
              └── SimilarityScore
```

This is a working design change. It is not yet an EF Core migration or production database change.

The actual database implementation will be finalized with the M6 persistence design.

## 17. M2 Integration

M2 Incident Management is currently not implemented.

M6 therefore uses:

```text
RelatedIncidentId
```

as an integer reference and does not create a duplicate `Incident` entity.

When M2 becomes available, M6 will be wired to it through the agreed integration boundary.

## 18. Current M6 Ownership

M6 currently owns:

```text
AIAnalysis
AIAnalysisStatus
AIAnalysisRelatedIncident
```

M6 does not own:

```text
Incident
User
Role
IncidentComment
IncidentStatusHistory
```

Those belong to other functional modules.

## 19. Current Unresolved Items

- Exact Gemini model IDs
- Exact prompts
- AI structured-output schema
- Confidence-score calculation and meaning
- Background-processing mechanism
- AI job persistence
- Failure logging/persistence
- AI result validation
- AI-05 similarity algorithm
- Similarity thresholds
- Final `RelationshipType` representation
- `SimilarityScore` final numeric type
- AI-07 assistant/RAG design
- Conversation-summary persistence
- Accept/override persistence
- Cross-module contracts
- EF Core navigation properties
- Final database relationships and constraints

## 20. Working Rules

1. Discuss before major architectural decisions.
2. Distinguish confirmed requirements from proposals.
3. Do not invent requirements and present them as confirmed.
4. Keep production M6 predominantly C# / ASP.NET Core.
5. Use Python/Jupyter when it genuinely adds value.
6. Keep M6 isolated from other members' internal code.
7. Do not duplicate other modules' entities.
8. Modify the shared database design when M6 genuinely requires it.
9. Document every major decision/work stage in `.md`.
10. Do not implement unresolved ideas prematurely.
11. Wire M6 to other modules deliberately when they become available.
12. Prefer the simplest architecture that satisfies the requirements.

## 21. Completed Work

```text
✓ Confirmed M6 module boundary
✓ Confirmed C# / ASP.NET Core as primary stack
✓ Established optional Python/Jupyter/ML strategy
✓ Mapped AI-01 to AI-10
✓ Established background AI-processing direction
✓ Decision 01 — AI analysis history
✓ Decision 02 — AI analysis lifecycle
✓ Defined initial AIAnalysis domain concept
✓ Created AIAnalysis.cs
✓ Created AIAnalysisStatus.cs
✓ Identified limitation of single RelatedIncidentId
✓ Designed AIAnalysisRelatedIncident concept
✓ Created AIAnalysisRelatedIncident.cs
✓ Identified correction needed for AIAnalysisId property
```

## 22. Immediate Next Step

Before adding more AI functionality:

1. Correct `AIAnalysisRelatedIncident.AIAnalysisId` to represent an ID.
2. Review the complete M6 Domain model.
3. Finalize the AI-05 relationship concept.
4. Create the AI-05 decision documentation.
5. Update the working database design.
6. Move into the M6 Application layer.

The goal is to reach real AI functionality without prematurely building infrastructure or blocking on unfinished modules.
