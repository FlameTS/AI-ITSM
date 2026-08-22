# M2-2.5 — M2 ↔ M6 Integration Analysis and Design Decision

**Project:** AI-Powered IT Service Desk & Incident Management System (AI-ITSM)  
**Module:** M2 — Incident Management  
**Workstream:** M2 Extensions + Integration  
**Checkpoint:** M2-2.5 — M2 ↔ M6 Integration Analysis / Design Decision  
**Date:** 22-08-2026  
**Status:** DESIGN DECISION COMPLETE — READY FOR IMPLEMENTATION

## 1. Purpose

This checkpoint verifies the existing M2 incident creation flow and the completed M6 AI core before implementing M2 ↔ M6 integration.

No M6 core redesign is proposed.

## 2. Existing M2 Incident Creation Flow

The existing `IncidentController.Create` creates a `CreateIncidentRequest` and calls:

`IIncidentService.CreateIncidentAsync(request, _currentUser.UserId)`

The existing `IncidentService`:

- trims title and description;
- sets the initial status to `IncidentStatus.Open`;
- sets `CreatedBy` from the logged-in employee;
- saves the incident;
- returns the generated `IncidentId`.

This is the correct source for the real incident identifier and incident content.

## 3. Existing M6 Integration Contract

The completed M6 Application layer exposes:

`IAIAnalysisService.RequestAnalysis(AnalyzeIncidentRequest request)`

`AnalyzeIncidentRequest` contains:

- `IncidentId`
- `Title`
- `Description`

The M6 Infrastructure implementation creates an `AIAnalysis` using the supplied incident ID, sends the title/description to the configured AI provider, persists the AI result, and returns the generated `AIAnalysisId`.

The current M6 implementation uses the existing `IAIProvider` / `GeminiProvider` chain.

## 4. Verified Integration Boundary

The justified integration boundary is:

```text
Employee
    ↓
M2 IncidentController.Create
    ↓
M2 IncidentService
    ↓
Incident saved
    ↓
Real IncidentId returned
    ↓
M6 IAIAnalysisService.RequestAnalysis(...)
    ↓
M6 AIAnalysisService
    ↓
IAIProvider
    ↓
GeminiProvider
    ↓
AIAnalysis persisted
```

M6 consumes M2-owned incident information. M6 does not create or duplicate the `Incident` entity.

## 5. Design Decision

For the current college-project implementation, the smallest justified integration is to invoke the existing M6 `IAIAnalysisService` immediately after the M2 incident has been successfully saved.

The M2 incident must be saved first.

The M6 call therefore receives:

- the newly generated `IncidentId`;
- the submitted incident title;
- the submitted incident description.

## 6. Failure Isolation

M6 AI processing failure must not invalidate the successfully created M2 incident.

If the M6 request fails:

- the incident remains saved and usable;
- the M2 create operation should not delete or roll back the incident;
- the failure should be handled at the Web integration boundary;
- M6's own `AIAnalysisService` already records a failed analysis state when provider processing fails.

This follows the M6 design principle that AI failure does not invalidate the underlying incident.

## 7. Background Processing Boundary

The project documentation identifies background AI processing as the intended direction, but does not finalize the exact technology.

Therefore this checkpoint does not introduce a new background-processing framework, queue, hosted service, or job system.

The immediate integration will use the existing M6 service contract.

A future background-processing implementation can replace the invocation mechanism without changing the M2 incident ownership model or M6 provider abstraction.

## 8. Scope Boundary

This work remains under M2-2 Integration.

The integration may update the existing M2 incident creation controller because that is the current point where a newly created real IncidentId becomes available.

This is an integration change, not a transfer of ownership of M2 core incident functionality.

No changes are planned to:

- M2 Incident domain ownership
- M2 Incident database structure
- M6 Domain model
- M6 provider implementation
- Gemini configuration
- M6 AIAnalysis database structure

## 9. Requirements / Decisions

### Requirement-supported

- M6 should analyze newly submitted incident descriptions.
- M2 owns the real incident data.
- M6 consumes M2 incident information.
- AI recommendations remain assistance and do not automatically override human decisions.

### Design decisions

- Invoke the existing M6 service after successful incident persistence.
- Pass `IncidentId`, `Title`, and `Description`.
- Keep incident creation successful even if AI processing fails.
- Do not introduce a new integration abstraction unless the existing code proves it necessary.
- Do not rebuild M6.

## 10. Implementation Plan

1. Update the existing `IncidentController` to receive `IAIAnalysisService`.
2. Create the M2 incident first using the existing `IIncidentService`.
3. Build `AnalyzeIncidentRequest` from the newly created incident data.
4. Call `IAIAnalysisService.RequestAnalysis`.
5. Handle M6 failure without invalidating the saved incident.
6. Build the application.
7. Create a real new incident through the M2 UI.
8. Verify a corresponding `AIAnalysis` record is created for the same `IncidentId`.
9. Verify AI result fields and status.
10. Verify that an AI failure does not remove the incident.
11. Document the completed integration checkpoint.

## 11. Current Status

**M2-2.5 Integration Analysis: COMPLETE**

**Ready for implementation.**

## 12. Next M2-2 Work

Implementation of the approved M2 → M6 integration, followed by M2-2.5 integration testing and documentation.
