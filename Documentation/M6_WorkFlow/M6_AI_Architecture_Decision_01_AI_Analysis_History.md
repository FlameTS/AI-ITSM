# M6 AI --- Architecture Decision 01: AI Analysis History

**Project:** AI-Powered IT Service Desk & Incident Management System\
**Module:** M6 --- AI Assistance\
**Decision:** AI Analysis History\
**Version:** 0.1\
**Status:** Approved Working Design Decision\
**Date:** August 2026

------------------------------------------------------------------------

## 1. Decision

**Decision: B --- AI analyses will be treated as historical records
associated with an incident.**

An incident may have multiple AI analysis records over its lifecycle.

Conceptually:

``` text
Incident
   |
   +── AI Analysis #1
   |
   +── AI Analysis #2
   |
   +── AI Analysis #3
```

The latest analysis may be used as the current recommendation, while
previous analyses remain available as history where the final
database/design supports it.

------------------------------------------------------------------------

## 2. Why This Decision Was Made

AI recommendations are not permanent facts.

An incident may be analyzed again because:

-   the incident information changed;
-   additional conversation/history became available;
-   AI processing is retried;
-   a new analysis is explicitly requested;
-   the AI model or analysis strategy changes;
-   an earlier analysis failed or was incomplete.

Keeping analysis history allows the system to preserve what the AI
previously produced rather than silently replacing it.

This is particularly appropriate because AI is an assistance capability
and human users remain responsible for final decisions.

------------------------------------------------------------------------

## 3. Relationship to Database v1

Database v1 already defines `AIAnalysis` as a separate table associated
with `Incidents` through `IncidentId`.

Current concepts include:

``` text
AIAnalysisId
IncidentId
SuggestedCategory
SuggestedPriority
SuggestedResolution
RelatedIncidentId
ConfidenceScore
CreatedAt
```

Database v1 does not explicitly finalize the exact analysis-history
behavior. Therefore, this decision is an M6 design decision that must be
validated during Database v2.

------------------------------------------------------------------------

## 4. Important Boundary

This decision does **not** mean that M6 should immediately modify the
database.

Before implementation, the team must determine:

-   whether the existing `AIAnalysis` structure is sufficient;
-   whether additional fields are required;
-   whether analysis status should be stored;
-   whether failed analyses should create persistent records;
-   how accepted/overridden recommendations are recorded;
-   how the latest/current analysis is identified;
-   whether analysis history should be visible to users;
-   whether model/version information should be stored.

These are separate decisions.

------------------------------------------------------------------------

## 5. Conceptual Lifecycle

A possible future lifecycle is:

``` text
Incident Created
      |
      v
AI Analysis #1
      |
      +── Completed
      |
      v
New information / retry / re-analysis
      |
      v
AI Analysis #2
      |
      +── Completed
      |
      v
Current AI Recommendation
```

The exact triggering rules will be defined during the AI workflow
design.

------------------------------------------------------------------------

## 6. Human Decision Relationship

AI analysis history must not be confused with human decisions.

For example:

``` text
AI Analysis #1
    |
    └── Suggested Priority: High

Help Desk Agent
    |
    └── Override
          |
          └── Final Priority: Medium
```

The AI recommendation remains part of the AI analysis history.

The human decision remains a separate application/workflow concern and
must be designed explicitly.

------------------------------------------------------------------------

## 7. Architectural Impact

This decision suggests that the M6 Domain/Application design should
allow an incident to have multiple AI analysis results.

It also supports future capabilities such as:

-   AI analysis retry;
-   re-analysis after incident updates;
-   comparing previous recommendations;
-   auditing AI behavior;
-   evaluating model changes;
-   understanding why an earlier recommendation differed from a later
    one.

No specific class structure is finalized by this document.

------------------------------------------------------------------------

## 8. Status

**Approved working decision.**

This decision should be considered when designing:

-   M6 Domain
-   M6 Application
-   AI persistence
-   Database v2
-   Human accept/override workflow
-   AI background processing

------------------------------------------------------------------------

## 9. Next Decision

The next architectural question is:

> **What should the M6 Domain model represent for an individual AI
> analysis?**

We will determine the minimum domain concepts required before creating
the first production C# files.
