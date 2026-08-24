# M2 — Incident Management: Extensions + Integration

**Project:** AI-Powered IT Service Management and Incident Resolution Platform (AI-ITSM)  
**Module:** M2 — Incident Management  
**Workstream:** M2 Extensions + Integration  
**Date consolidated:** 22-08-2026  
**Status:** COMPLETE for the documented M2-2 scope

---

## 1. Purpose

This document consolidates the M2 Extensions + Integration work documented across the M2-2 checkpoints.

The documented extension responsibilities were:

- Incident communication/comments
- Employee incident notifications/updates
- Incident attachments/supporting information
- Employee feedback after resolution
- M2 ↔ M6 integration
- Integration testing
- Final M2 integration/checkpoint coordination

The work remains an M2 employee-side responsibility. Other module ownership is preserved:

- M1 — Authentication, users and roles
- M3 — Agent workflow and incident history
- M4 — Administration
- M5 — Manager dashboard and reporting
- M6 — Gemini AI core
- M7 — n8n automation and escalation

M6 was not rebuilt.

---

# 2. M2-2 Work Division

The M2 Extensions + Integration contributor was responsible for:

- Incident communication/comments
- Employee ↔ Agent communication support
- Employee incident notifications/updates
- Incident attachments/supporting information
- Employee feedback after resolution
- M2 ↔ M6 integration
- Integration testing
- Final M2 integration/checkpoint coordination

The documented extension workflow was:

```text
Core Incident
     ↓
Comments / Communication
     ↓
Notifications / Updates
     ↓
Attachments
     ↓
Resolution
     ↓
Employee Feedback
     ↓
M6 AI Integration
```

The project follows the principle of implementing the documented requirements with the simplest maintainable solution that fits the existing architecture.

---

# 3. Existing Architecture

M2-2 follows the existing four-layer project architecture:

```text
AIITSM.Domain
      ↓
AIITSM.Application
      ↓
AIITSM.Infrastructure
      ↓
AIITSM.Web
```

The shared `AIITSMDbContext` is reused. No separate M2-specific DbContext was introduced.

M2-2 also reuses the existing Incident Details page instead of creating duplicate incident UI.

---

# 4. M2-2.1 — Communication / Comments

## 4.1 Purpose

Incident Communication / Comments was implemented end-to-end to support communication between the employee and support/agent side and allow an employee to provide additional information about an incident.

## 4.2 Workflow

```text
Employee
    ↓
My Incidents
    ↓
Incident Details
    ↓
Communication
    ↓
View existing comments
    ↓
Add comment
    ↓
IncidentCommunicationController
    ↓
IIncidentCommentService
    ↓
IncidentCommentService
    ↓
AIITSMDbContext
    ↓
IncidentComments
    ↓
SQL Server
```

The employee can:

- open an incident;
- view existing comments;
- add a comment;
- receive success feedback;
- see the persisted comment after refresh.

## 4.3 Implementation Structure

### Domain

```text
AIITSM.Domain/
└── 02_M2_IncidentManagement_2/
    └── IncidentComment.cs
```

Represents:

```text
CommentId
IncidentId
UserId
CommentText
CreatedAt
```

### Application

```text
AIITSM.Application/
└── 02_M2_IncidentManagement_2/
    └── Communication/
        ├── IIncidentCommentService.cs
        └── IncidentCommentDto.cs
```

### Infrastructure

```text
AIITSM.Infrastructure/
└── 02_M2_IncidentManagement_2/
    ├── Configurations/
    │   └── IncidentCommentConfiguration.cs
    └── Communication/
        └── IncidentCommentService.cs
```

### Web

```text
AIITSM.Web/
└── Controllers/
    └── 02_M2_IncidentManagement_2/
        └── IncidentCommunicationController.cs
```

The existing:

```text
AIITSM.Web/Views/Incident/Details.cshtml
```

was extended with the Communication section.

### Shared DbContext

The existing:

```text
AIITSM.Infrastructure/06_M6_AI/AIITSMDbContext.cs
```

was extended with:

```csharp
public DbSet<IncidentComment> IncidentComments { get; set; }
```

### Dependency Injection

```csharp
builder.Services.AddScoped<
    IIncidentCommentService,
    IncidentCommentService>();
```

## 4.4 Database

The existing `IncidentComments` table was reused.

No new SQL table or migration was required for this feature.

---

# 5. M2-2.2 — Notifications / Incident Updates

## 5.1 Purpose

The notification/update feature provides employee-facing persisted notifications without rebuilding M7.

M7 remains responsible for n8n automation and escalation.

## 5.2 Workflow

```text
Notification record
      ↓
NotificationService
      ↓
NotificationController
      ↓
Employee Notification UI
      ↓
View notification
      ↓
Mark as Read
      ↓
Persist IsRead = true
```

The employee can:

- view notifications;
- identify unread notifications;
- open a related incident when available;
- mark notifications as read.

## 5.3 Implementation Structure

### Domain

```text
AIITSM.Domain/
└── 02_M2_IncidentManagement_2/
    └── Notification.cs
```

Fields:

```text
NotificationId
UserId
IncidentId
Message
IsRead
CreatedAt
```

### Application

```text
AIITSM.Application/
└── 02_M2_IncidentManagement_2/
    └── Notifications/
        ├── NotificationDto.cs
        └── INotificationService.cs
```

Operations:

```text
GetMyNotificationsAsync
GetUnreadCountAsync
CreateNotificationAsync
MarkAsReadAsync
```

### Infrastructure

```text
AIITSM.Infrastructure/
└── 02_M2_IncidentManagement_2/
    ├── Configurations/
    │   └── NotificationConfiguration.cs
    └── Notifications/
        └── NotificationService.cs
```

### Web

```text
AIITSM.Web/
└── Controllers/
    └── 02_M2_IncidentManagement_2/
        └── NotificationController.cs
```

View:

```text
AIITSM.Web/
└── Views/
    └── Notification/
        └── Index.cshtml
```

### Shared DbContext

The existing shared DbContext was extended with:

```csharp
public DbSet<Notification> Notifications { get; set; }
```

### Dependency Injection

```csharp
builder.Services.AddScoped<
    INotificationService,
    NotificationService>();
```

## 5.4 Security / Ownership

Notification retrieval uses the current application user through:

```csharp
_currentUser.UserId
```

Mark-as-read verifies both the notification ID and current user ID before changing `IsRead`.

## 5.5 UI Behavior

The notification page displays:

- unread state;
- message;
- creation time;
- Mark as Read action;
- View Incident link when `IncidentId` is available.

After marking as read:

- the unread badge disappears;
- Mark as Read disappears;
- the notification remains visible;
- View Incident remains available;
- refresh preserves the read state.

## 5.6 Database

The existing `Notifications` table was reused.

No new SQL table or migration was required.

---

# 6. M2-2.3 — Attachments / Supporting Information

## 6.1 Requirement Basis

M2 employee-side responsibilities include supporting information/attachments associated with incidents.

The requirements did not specify:

- exact physical storage mechanism;
- exact allowed file extensions;
- exact maximum file size;
- exact attachment metadata schema.

Therefore these implementation details were explicitly treated as design decisions.

## 6.2 Design

The chosen project implementation is:

```text
SQL Server
    ↓
Attachment metadata

AIITSM.Web
    ↓
wwwroot/uploads/incidents/
    ↓
Physical uploaded files
```

Database metadata:

```text
AttachmentId
IncidentId
FileName
StoredFileName
ContentType
FileSize
UploadedAt
```

The original filename is retained as metadata.

A generated unique filename is used for physical storage to avoid collisions.

## 6.3 Validation Decisions

Maximum upload size:

```text
10 MB
```

Allowed extensions:

```text
.pdf
.png
.jpg
.jpeg
.doc
.docx
.txt
```

These limits are implementation/design decisions, not requirements claimed by the source requirements.

No image-only requirement, cloud storage, or Gemini/multimodal attachment analysis was introduced.

## 6.4 Implementation Structure

### Domain

```text
AIITSM.Domain/
└── 02_M2_IncidentManagement_2/
    └── IncidentAttachment.cs
```

### Application

```text
AIITSM.Application/
└── 02_M2_IncidentManagement_2/
    └── Attachments/
        ├── IncidentAttachmentDto.cs
        └── IIncidentAttachmentService.cs
```

### Infrastructure

```text
AIITSM.Infrastructure/
└── 02_M2_IncidentManagement_2/
    ├── Configurations/
    │   └── IncidentAttachmentConfiguration.cs
    └── Attachments/
        └── IncidentAttachmentService.cs
```

### Web

```text
AIITSM.Web/
└── Controllers/
    └── 02_M2_IncidentManagement_2/
        └── IncidentAttachmentController.cs
```

The existing Incident Details page was extended and the attachment partial was added:

```text
AIITSM.Web/
└── Views/
    └── IncidentAttachment/
        └── _Attachments.cshtml
```

### Shared DbContext

The existing shared DbContext was extended with:

```csharp
public DbSet<IncidentAttachment> IncidentAttachments { get; set; }
```

## 6.5 Upload Workflow

```text
Employee
   ↓
Incident Details
   ↓
Select supporting file
   ↓
POST IncidentAttachment/Upload
   ↓
Verify employee owns incident
   ↓
Validate file
   ↓
Generate unique stored filename
   ↓
Store physical file
   ↓
Save attachment metadata
   ↓
Redirect to Incident Details
   ↓
Load attachment list
```

If metadata persistence fails after the physical file is written, the implementation attempts to remove the physical file to avoid an orphaned upload.

## 6.6 Ownership Protection

The upload operation verifies that the current employee owns the incident.

It uses the existing:

```text
ICurrentUserService
```

and:

```text
IIncidentService.GetMyIncidentsAsync(...)
```

rather than trusting a browser-supplied user/incident relationship.

## 6.7 Database

The original database script did not contain `IncidentAttachments`.

A new:

```text
dbo.IncidentAttachments
```

table was therefore added with:

```text
AttachmentId
IncidentId
FileName
StoredFileName
ContentType
FileSize
UploadedAt
```

---

# 7. M2-2.4 — Employee Feedback After Resolution

## 7.1 Requirement

FR-08 requires the employee to be able to provide feedback after an incident is resolved.

The requirement did not specify:

- rating scale;
- stars;
- exact feedback fields;
- whether written feedback is mandatory;
- editing/deletion;
- whether one or multiple submissions are allowed.

These were therefore handled as M2-2 design decisions.

## 7.2 Final Design

Feedback is text/string based.

No star or numeric rating system is used.

A dedicated table is used:

```text
dbo.IncidentFeedback
```

Fields:

```text
FeedbackId
IncidentId
UserId
FeedbackText
CreatedAt
```

`FeedbackText` is nullable.

One feedback record is allowed per employee per incident.

A unique `(IncidentId, UserId)` constraint provides database-level duplicate protection.

Feedback is read-only after submission.

Feedback is available when:

```text
IncidentStatus.Resolved
```

The logged-in employee is obtained through the existing current-user mechanism.

The service verifies incident ownership.

Feedback is integrated into the existing Incident Details page.

## 7.3 Implementation Structure

### Domain

```text
IncidentFeedback.cs
```

### Application

```text
Feedback/
├── IncidentFeedbackDto.cs
└── IIncidentFeedbackService.cs
```

The interface exposes only the required operations:

```text
GetFeedbackAsync(...)
AddFeedbackAsync(...)
```

No unnecessary update/delete/all-feedback operations were introduced.

### Infrastructure

```text
Feedback/
└── IncidentFeedbackService.cs

Configurations/
└── IncidentFeedbackConfiguration.cs
```

The service validates:

1. incident exists;
2. logged-in employee owns the incident;
3. incident status is `Resolved`;
4. employee has not already submitted feedback.

Whitespace-only or empty feedback is normalized to `NULL`; non-empty feedback is trimmed before persistence.

### Web

```text
IncidentFeedbackController.cs
```

The controller:

- retrieves current employee feedback;
- accepts feedback submission;
- uses anti-forgery protection;
- obtains the logged-in employee through `ICurrentUserService`;
- redirects back to Incident Details.

The feedback feature remains separate from the core `IncidentController`.

## 7.4 UI

The existing Incident Details page contains the feedback section.

Behavior:

### Resolved incident without feedback

The feedback form is displayed.

### Resolved incident with existing feedback

Submitted feedback is displayed read-only.

### Non-resolved incident

The employee is informed that feedback can be provided after resolution.

Written feedback is not mandatory.

## 7.5 Testing

The documented tests include:

- unresolved incident restriction — PASS;
- resolved incident feedback form/submission — PASS;
- nullable feedback — PASS;
- duplicate submission prevention — PASS;
- ownership/security was verified by the relevant teammate and was not independently repeated in the final checkpoint;
- solution build — SUCCESS.

---

# 8. M2-2 Final Functional Status

The completed M2-2 employee-side extensions are:

| Feature | Status |
|---|---|
| Communication / Comments | COMPLETE |
| Notifications / Updates | COMPLETE |
| Attachments / Supporting Information | COMPLETE |
| Employee Feedback after Resolution | COMPLETE |
| M2 ↔ M6 Integration | COMPLETE |
| Integration Testing | COMPLETE |

The final integration test confirmed that the M2-2 functionality and M2 ↔ M6 integration continued to operate together without breaking the existing functionality.

---

# 9. Final M2-2 Integration Test

A real M2 incident was used:

```text
IncidentId: 5
Incident Number: INC-000005
Title: Unable to connect to office WiFi
```

The test verified:

- incident creation;
- M6 AI analysis;
- communication/comments;
- attachment upload;
- notification behavior;
- resolution → feedback;
- persistence after refresh;
- regression of existing M2-2 features.

The corresponding M6 analysis was:

```text
AIAnalysisId: 8
IncidentId: 5
SuggestedCategory: Network / Wi-Fi
SuggestedPriority: Low
ConfidenceScore: 0.95
Status: Completed
```

M2-2 final integration testing passed.

---

# 10. M2-2 Database Summary

M2-2 reused existing database structures where appropriate and added dedicated structures where required.

### Reused

```text
IncidentComments
Notifications
```

### Added

```text
IncidentAttachments
IncidentFeedback
```

### Shared persistence boundary

```text
AIITSM.Infrastructure/06_M6_AI/AIITSMDbContext.cs
```

The shared DbContext was extended rather than creating a second M2 DbContext.

---

# 11. Security and Ownership Summary

The M2-2 implementation consistently uses the logged-in application user rather than trusting arbitrary user IDs from the browser.

Applied protections include:

- current-user resolution through `ICurrentUserService`;
- incident ownership validation for attachments;
- incident ownership validation for feedback;
- notification ownership validation;
- anti-forgery protection for feedback submission;
- existing M2 ownership workflow reuse.

---

# 12. Scope Boundary

The work was limited to M2-2.

No unrelated M1, M3, M4, M5, M6, or M7 functionality was rebuilt.

In particular:

- M6 AI core remains owned by M6;
- M3 remains responsible for agent-side resolution workflow;
- M7 remains responsible for n8n automation/escalation;
- M2 remains responsible for employee-side incident functionality.

---

# 13. Final Status

```text
M2-2 Extensions + Integration
        ↓
Communication              COMPLETE
Notifications              COMPLETE
Attachments                COMPLETE
Employee Feedback          COMPLETE
M2 ↔ M6 Integration        COMPLETE
Integration Testing        COMPLETE
        ↓
M2-2 COMPLETE
```

The documented next step after these checkpoints was a final source-control review before committing and pushing the completed M2 work, including identification of changed files, new files, `Database.sql` changes, documentation files, and unrelated teammate changes that must not be overwritten.
