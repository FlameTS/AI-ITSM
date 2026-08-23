# M6 AI --- Architecture Decision 02: AI Analysis Lifecycle

**Project:** AI-Powered IT Service Desk & Incident Management System\
**Module:** M6 --- AI Assistance\
**Decision:** AI Analysis Lifecycle\
**Version:** 0.1\
**Status:** Approved Working Design Decision\
**Date:** August 2026

------------------------------------------------------------------------

## 1. Decision

**Decision: An individual AI analysis represents one AI analysis attempt
and has a processing lifecycle.**

The conceptual lifecycle is:

``` text
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

Each analysis attempt is therefore more than just its final
recommendation. It also represents the state of that processing attempt.

------------------------------------------------------------------------

## 2. Why This Decision Was Made

The agreed M6 workflow performs AI analysis in the background after an
incident has already been successfully persisted.

Therefore, there is a period where:

-   the incident exists;
-   AI analysis has been requested;
-   the AI result is not yet available.

The system needs a way to distinguish that state from a completed
analysis and from a failed analysis.

This also supports the previously approved decision that an incident can
have multiple AI analyses over its lifecycle.

------------------------------------------------------------------------

## 3. Conceptual Example

For Incident `#1042`:

``` text
Incident #1042
    |
    +── AI Analysis #1
    |      └── Failed
    |
    +── AI Analysis #2
    |      └── Completed
    |
    +── AI Analysis #3
           └── Completed
```

Each record represents a distinct analysis attempt.

------------------------------------------------------------------------

## 4. Analysis Lifecycle

### Pending

The analysis has been requested but processing has not started.

### Processing

The background AI process is currently performing the analysis.

### Completed

The AI analysis completed successfully and produced a validated result.

### Failed

The AI analysis could not complete successfully.

A failed AI analysis must not invalidate or remove the underlying
incident.

------------------------------------------------------------------------

## 5. Relationship to Incident Management

The core workflow is:

``` text
Employee
   |
   v
Submit Incident
   |
   v
Save Incident
   |
   +--------------------> Incident remains available
   |
   v
Create AI Analysis
   |
   v
Pending
   |
   v
Processing
   |
   +--------------------+
   |                    |
   v                    v
Completed             Failed
```

The incident-management operation is therefore independent from
successful AI processing.

------------------------------------------------------------------------

## 6. Relationship to AI Analysis History

Because each analysis attempt is represented separately, an incident may
have a sequence of analysis records:

``` text
Incident
   |
   +── Analysis #1 — Failed
   +── Analysis #2 — Completed
   +── Analysis #3 — Completed
```

The application can later determine which completed analysis is the
current/latest recommendation according to the final workflow rules.

The exact rule for identifying the current analysis is not yet
finalized.

------------------------------------------------------------------------

## 7. Relationship to Database v1

Database v1 currently contains an `AIAnalysis` table with:

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

The database document does not currently define a processing-status
field.

Therefore:

**Do not modify Database v1 solely because of this decision.**

The need for persistence of processing state must be validated during
Database v2 design.

------------------------------------------------------------------------

## 8. Failed Analysis

A failed AI analysis must not cause:

-   incident deletion;
-   incident creation failure after the incident has already been
    persisted;
-   automatic human decision;
-   automatic change to the incident's final
    category/priority/resolution.

The exact failure information to persist, display, or log remains a
future design decision.

------------------------------------------------------------------------

## 9. Human Decision Boundary

The analysis lifecycle does not give AI authority over the incident.

Even after:

``` text
AI Analysis = Completed
```

the result remains a recommendation.

The human review workflow remains:

``` text
AI Recommendation
       |
       v
Authorized Support Personnel
       |
   +---+---+
   |       |
Accept   Override
   |       |
   +---+---+
       |
       v
Human Decision
```

------------------------------------------------------------------------

## 10. Architectural Impact

This decision affects the future design of:

-   M6 Domain concepts
-   M6 Application services
-   Background AI processing
-   AI persistence
-   Error handling
-   Human review workflow
-   Database v2

It does not by itself determine the exact C# class structure or database
schema.

------------------------------------------------------------------------

## 11. Current Status

**Approved working decision.**

The following are now approved M6 design directions:

1.  An incident may have multiple AI analysis records.
2.  Each AI analysis represents one analysis attempt.
3.  Each analysis has a conceptual processing lifecycle:
    -   Pending
    -   Processing
    -   Completed
    -   Failed
4.  AI failure must not invalidate the underlying incident.
5.  Exact database persistence will be decided during Database v2
    validation.

------------------------------------------------------------------------

## 12. Next Step

The next design step is to define the **minimum domain representation of
one AI analysis**.

We will identify only the concepts that genuinely belong in the M6
Domain layer before creating the first production C# class.
