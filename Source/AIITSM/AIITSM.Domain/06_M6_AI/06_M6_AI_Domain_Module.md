# M6 AI — Domain Module

**Project:** AI-Powered IT Service Desk & Incident Management System  
**Module:** M6 — AI Assistance  
**Layer:** Domain  
**Folder:** `AIITSM.Domain/06_M6_AI`  
**Status:** Initial Domain Design Implemented  
**Version:** 0.1  
**Date:** August 2026

---

## 1. Purpose

This folder contains the Domain-layer concepts owned by the M6 AI Assistance module.

The Domain layer represents concepts that are meaningful to the AI module itself. It does not contain API calls, database access, controllers, UI logic, or Gemini-specific implementation.

---

## 2. Why M6 AI Concepts Are in the Domain Layer

The project follows a Layered Modular Monolith.

The Domain layer is used to represent the core concepts and rules belonging to a module.

For M6, an AI analysis is a business/domain concept:

> An incident can be analyzed by the AI, producing recommendations and related-incident information while moving through an analysis lifecycle.

Therefore concepts such as:

- `AIAnalysis`
- `AIAnalysisStatus`
- `AIAnalysisRelatedIncident`
- `AIIncidentRelationshipType`

belong to the M6 Domain layer.

They describe **what an AI analysis means**, not **how Gemini, a database, or HTTP performs the work**.

---

## 3. What Does NOT Belong Here

The M6 Domain layer should not contain:

- Gemini API calls
- API keys
- HTTP clients
- JSON/API-specific code
- Entity Framework Core configuration
- Database connection code
- ASP.NET Core controllers
- UI code
- Background worker implementation
- Python code
- Jupyter notebooks

Those concerns will be handled by the appropriate layers later.

Conceptually:

```text
Domain
  ↓
What an AI analysis IS

Application
  ↓
What the application DOES with an AI analysis

Infrastructure
  ↓
HOW external services/database/AI providers are accessed

Web
  ↓
HOW users/API endpoints interact with the functionality
```

---

## 4. Current Domain Concepts

### `AIAnalysis`

Represents one AI analysis attempt for an incident.

Current responsibilities:

```text
AIAnalysis
├── AIAnalysisId
├── IncidentId
├── Status
├── SuggestedCategory
├── SuggestedPriority
├── SuggestedResolution
├── ConfidenceScore
└── CreatedAt
```

An incident may have multiple AI analyses over its lifecycle.

Example:

```text
Incident #1042
    ├── AI Analysis #1
    ├── AI Analysis #2
    └── AI Analysis #3
```

This preserves analysis history instead of automatically overwriting previous results.

---

### `AIAnalysisStatus`

Represents the lifecycle of one AI analysis attempt.

Current states:

```text
Pending
Processing
Completed
Failed
```

Conceptually:

```text
Pending
   ↓
Processing
   ↓
Completed
```

or:

```text
Pending
   ↓
Processing
   ↓
Failed
```

AI failure must not invalidate the underlying incident.

---

### `AIAnalysisRelatedIncident`

Represents a relationship between an AI analysis and an existing incident identified as potentially related or duplicate.

Current concepts:

```text
AIAnalysisRelatedIncident
├── AIAnalysisRelatedIncidentId
├── AIAnalysisId
├── RelatedIncidentId
├── RelationshipType
└── SimilarityScore
```

This was introduced because a single `RelatedIncidentId` inside `AIAnalysis` would only allow one related incident.

The new relationship structure allows:

```text
AIAnalysis #25
    ├── Incident #101 → Related
    ├── Incident #108 → Duplicate
    └── Incident #117 → Related
```

---

### `AIIncidentRelationshipType`

Current relationship types:

```text
Related
Duplicate
```

These are controlled application concepts rather than arbitrary AI-generated strings.

The final representation and possible future values remain open to later design.

---

## 5. Module Ownership

M6 owns the AI concepts above.

M6 does **not** own:

```text
Incident
User
Role
IncidentComment
IncidentStatusHistory
```

Those belong to other functional modules.

For example:

```text
M2 owns:
Incident

M6 owns:
AIAnalysis
AIAnalysisRelatedIncident
```

M6 references an incident using its identifier rather than creating a duplicate `Incident` entity.

---

## 6. Current Relationship Model

The intended relationship is:

```text
AIAnalysis
    1
    │
    │
    └──────────< AIAnalysisRelatedIncident
                       │
                       └──────> Existing Incident
```

This means:

- One AI analysis can have many related/duplicate incident records.
- Each relationship belongs to one AI analysis.
- Each relationship points to an existing incident.

The exact EF Core navigation properties and database constraints will be designed later in the persistence/infrastructure work.

---

## 7. Important Design Decisions

### Decision 01 — Analysis History

An incident may have multiple AI analysis records.

### Decision 02 — Analysis Lifecycle

Each analysis represents one analysis attempt with:

- Pending
- Processing
- Completed
- Failed

### Decision 03 — Multiple Related Incidents

AI-05 requires the possibility of multiple related/duplicate incidents.

Therefore, the original single `RelatedIncidentId` concept is replaced by the `AIAnalysisRelatedIncident` relationship concept.

---

## 8. Current Database Direction

The working database design is:

```text
AIAnalysis
────────────────────────
AIAnalysisId
IncidentId
Status
SuggestedCategory
SuggestedPriority
SuggestedResolution
ConfidenceScore
CreatedAt
```

and:

```text
AIAnalysisRelatedIncident
──────────────────────────────
AIAnalysisRelatedIncidentId
AIAnalysisId
RelatedIncidentId
RelationshipType
SimilarityScore
```

This is a working design and can be refined when EF Core persistence and the shared database are implemented.

---

## 9. Current M2 Integration Situation

M2 Incident Management is currently not implemented.

M6 therefore does not create an `Incident` class.

For now, `RelatedIncidentId` remains an integer reference.

When M2 becomes available, the integration will be wired through an agreed module boundary.

---

## 10. What Comes Next

The Domain layer should remain small until another requirement justifies a new concept.

The next major layer is:

```text
AIITSM.Application
└── 06_M6_AI
```

The Application layer will eventually contain the AI use-case/orchestration responsibilities, such as:

- Starting an AI analysis
- Coordinating analysis steps
- Calling AI abstractions
- Validating AI results
- Handling the application-level analysis workflow
- Coordinating background processing

No Gemini-specific implementation belongs in the Domain layer.

---

## 11. Current Status

**M6 Domain — Initial Design Complete**

Implemented/defined:

- `AIAnalysis`
- `AIAnalysisStatus`
- `AIAnalysisRelatedIncident`
- `AIIncidentRelationshipType`

The Domain layer is intentionally kept simple and isolated.

Further concepts should only be added when a requirement or later design decision justifies them.
