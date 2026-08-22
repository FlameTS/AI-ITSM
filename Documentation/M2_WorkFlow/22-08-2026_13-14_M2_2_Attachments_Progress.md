# M2-2.3 — Attachments / Supporting Information Progress Checkpoint

**Project:** AI-Powered IT Service Management and Incident Resolution Platform (AI-ITSM)  
**Module:** M2 — Incident Management  
**Workstream:** M2 Extensions + Integration  
**Feature:** Attachments / Supporting Information  
**Status:** IN PROGRESS  
**Date:** 22-08-2026  
**Time:** 13:14

---

## 1. Purpose

This checkpoint documents the completed foundation and backend preparation for M2-2.3 — Attachments / Supporting Information.

The feature is **not yet considered complete** because the Incident Details UI and end-to-end upload test are still pending.

---

## 2. Requirement Basis

The M2 employee-side responsibilities include:

- Supporting information / attachments
- Employee incident creation
- Employee incident details

The requirements establish that an incident can contain relevant supporting information.

The exact attachment storage mechanism, exact file types, and exact file-size limit were not explicitly specified.

Therefore, the storage approach and validation limits below are explicitly recorded as design decisions.

---

## 3. Design Decision

For the college-project implementation:

```text
SQL Server
    ↓
Attachment metadata

Web Server
    ↓
wwwroot/uploads/incidents/
    ↓
Physical uploaded files
```

The database stores metadata while the uploaded file itself is stored on the Web application's filesystem.

The planned metadata is:

```text
AttachmentId
IncidentId
FileName
StoredFileName
ContentType
FileSize
UploadedAt
```

A generated unique server filename is used instead of storing the browser-provided filename as the physical filename.

---

## 4. Validation Design Decisions

The current implementation uses:

### Maximum file size

```text
10 MB
```

### Allowed extensions

```text
.pdf
.png
.jpg
.jpeg
.doc
.docx
.txt
```

These are implementation/design decisions rather than requirements claimed by the source documentation.

No image-only requirement has been introduced.

No Gemini/multimodal attachment analysis has been introduced.

No cloud storage has been introduced.

---

## 5. Completed Files

### Domain

Created:

```text
AIITSM.Domain/
└── 02_M2_IncidentManagement_2/
    └── IncidentAttachment.cs
```

### Application

Created:

```text
AIITSM.Application/
└── 02_M2_IncidentManagement_2/
    └── Attachments/
        ├── IncidentAttachmentDto.cs
        └── IIncidentAttachmentService.cs
```

### Infrastructure

Created:

```text
AIITSM.Infrastructure/
└── 02_M2_IncidentManagement_2/
    ├── Configurations/
    │   └── IncidentAttachmentConfiguration.cs
    └── Attachments/
        └── IncidentAttachmentService.cs
```

### Shared DbContext

Modified:

```text
AIITSM.Infrastructure/
└── 06_M6_AI/
    └── AIITSMDbContext.cs
```

Added:

```csharp
public DbSet<IncidentAttachment> IncidentAttachments { get; set; }
```

### Web

Created:

```text
AIITSM.Web/
└── Controllers/
    └── 02_M2_IncidentManagement_2/
        └── IncidentAttachmentController.cs
```

---

## 6. Backend Workflow Implemented

The current upload controller supports:

```text
POST Upload
    ↓
Verify employee owns incident
    ↓
Verify file exists
    ↓
Validate file size
    ↓
Validate extension
    ↓
Generate unique stored filename
    ↓
Create upload directory if required
    ↓
Write physical file
    ↓
Save attachment metadata
    ↓
Redirect to Incident Details
```

If metadata persistence fails after the physical file is written, the implementation attempts to remove the physical file so that an orphaned upload is not left behind.

---

## 7. Ownership Protection

The upload endpoint checks that the current user owns the incident before accepting an attachment.

It uses the existing:

```text
ICurrentUserService
```

and existing:

```text
IIncidentService.GetMyIncidentsAsync(...)
```

This follows the existing M2 ownership model instead of trusting an incident/user relationship supplied by the browser.

---

## 8. Build Testing

Successfully verified:

```text
Domain implementation          ✅
EF configuration              ✅
Shared DbContext              ✅
Application DTO               ✅
Application interface         ✅
Infrastructure service        ✅
DI registration               ✅
Upload controller             ✅
dotnet build                  ✅
```

The project currently builds successfully.

---

## 9. Not Yet Completed

The following are still pending:

```text
Incident Details attachment UI       ⏳
Choose file / upload form             ⏳
Existing attachments display          ⏳
Physical file upload test             ⏳
Database metadata verification        ⏳
Refresh/persistence verification      ⏳
Invalid file validation test          ⏳
Oversized file validation test       ⏳
Final M2-2.3 checkpoint               ⏳
```

Therefore:

> **M2-2.3 Attachments is NOT yet complete.**

---

## 10. Next Step

Integrate the attachment section into the existing:

```text
AIITSM.Web/
└── Views/
    └── Incident/
        └── Details.cshtml
```

The existing Communication section must be preserved.

Planned UI:

```text
Incident Details
    ↓
Communication
    ↓
Attachments
    ├── Choose File
    ├── Upload
    └── Existing Attachments
```

After UI integration, perform an actual upload and verify the complete database + filesystem flow.

---

## 11. M2-2 Overall Progress

```text
Communication / Comments       ✅ COMPLETE
Notifications / Updates        ✅ COMPLETE
Attachments                    ⏳ IN PROGRESS
Feedback                       ⏳
M2 ↔ M6 Integration            ⏳
Integration Testing            ⏳
Final M2 Checkpoint            ⏳
```

---

## 12. Checkpoint Result

**M2-2.3 Attachments / Supporting Information — BACKEND FOUNDATION COMPLETE**

The domain, Application, Infrastructure, shared DbContext, DI registration, and Web upload endpoint have been implemented and successfully built.

The feature will only be marked **COMPLETE** after Incident Details UI integration and end-to-end upload testing.
