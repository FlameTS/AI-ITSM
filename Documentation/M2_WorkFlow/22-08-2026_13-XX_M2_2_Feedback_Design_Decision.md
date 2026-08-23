# M2-2.4 — Employee Feedback Design Decision

**Project:** AI-Powered IT Service Desk & Incident Management System (AI-ITSM)  
**Module:** M2 — Incident Management  
**Workstream:** M2 Extensions + Integration  
**Checkpoint:** M2-2.4 — Employee Feedback after Resolution  
**Date:** 22-08-2026

## 1. Purpose

This checkpoint records the design decision for employee feedback after an incident is resolved. No implementation has been started in this checkpoint.

## 2. Requirement Addressed

FR-08 requires the employee to be able to provide feedback after an incident is resolved.

The requirement does not specify:
- a rating scale;
- stars;
- exact feedback fields;
- whether written feedback is mandatory;
- editing or deletion;
- whether one or multiple submissions are allowed.

These items are therefore treated as design decisions.

## 3. Design Decisions

### Feedback representation

Feedback will use a text/string field rather than a star or numeric rating system.

### Database structure

A new dedicated `IncidentFeedback` table will be created and added to `Database.sql`.

The existing incident, comments, attachments, or notification tables will not be repurposed for feedback.

### Proposed table

- `FeedbackId` — primary key
- `IncidentId` — foreign key to `Incidents`
- `UserId` — foreign key to `Users`
- `FeedbackText` — nullable text
- `CreatedAt` — submission timestamp

### Submission rule

One employee may submit one feedback record for a particular incident.

A unique constraint on `(IncidentId, UserId)` will enforce this rule.

### Editing and deletion

Feedback will not support editing or deletion in the initial implementation. The requirement only requires the employee to provide feedback, so additional CRUD operations are unnecessary.

### Resolution condition

Feedback will be available when the incident status is `Resolved`.

The existing `IncidentStatus` already contains `Resolved`.

### Ownership/security

The logged-in employee will be determined through the existing current-user mechanism. The client will not be trusted to provide an arbitrary employee/user ID.

### UI placement

Feedback will be added to the existing Incident Details page rather than creating a separate incident-feedback workflow/page.

## 4. Architecture Decision

The feature will remain entirely within the M2-2 workstream and follow the existing four-layer architecture:

AIITSM.Domain
→ AIITSM.Application
→ AIITSM.Infrastructure
→ AIITSM.Web

The existing M2-2 implementation pattern for Communication, Notifications, and Attachments will be followed.

## 5. M6 Boundary

No changes to the completed M6 AI core are required for employee feedback.

M6 remains responsible for AIAnalysis and AI-specific functionality. Feedback remains an M2-2 employee-side responsibility.

## 6. Expected Implementation Structure

### Domain
`IncidentFeedback.cs`

### Application
`Feedback/IIncidentFeedbackService.cs`  
`Feedback/IncidentFeedbackDto.cs`

### Infrastructure
`Feedback/IncidentFeedbackService.cs`  
`Configurations/IncidentFeedbackConfiguration.cs`

### Web
`IncidentFeedbackController.cs`

The existing `Incident/Details.cshtml` will be extended with the feedback section.

The shared `AIITSMDbContext` will receive the required `DbSet<IncidentFeedback>` registration, following the existing project pattern.

## 7. Database Changes

A new `dbo.IncidentFeedback` table will be added to `Database.sql`.

No existing feedback-related table exists in the current database design, so no existing table will be repurposed.

## 8. Testing Plan

Implementation testing will verify:

1. Employee can see feedback functionality only for a resolved incident.
2. Employee can submit feedback.
3. Nullable feedback text is persisted correctly.
4. Feedback is associated with the correct incident and logged-in employee.
5. Duplicate feedback for the same employee/incident is prevented.
6. Feedback remains persisted after refresh.
7. Another employee cannot submit feedback as the original employee.
8. Existing M2-2 features continue to work.
9. The project builds successfully.

## 9. Current Status

**M2-2.4 Feedback Design: APPROVED / READY FOR IMPLEMENTATION**

Implementation has not yet started.

## 10. Remaining M2-2 Work

After feedback:

- M2-2.5 — M2 ↔ M6 Integration
- M2-2.6 — Integration Testing
- Final M2 checkpoint

