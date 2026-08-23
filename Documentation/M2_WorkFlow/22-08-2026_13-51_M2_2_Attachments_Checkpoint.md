# M2-2 — Attachments / Supporting Information Checkpoint

**Project:** AI-Powered IT Service Management and Incident Resolution Platform (AI-ITSM)  
**Module:** M2 — Incident Management  
**Workstream:** Extensions + Integration  
**Feature:** Attachments / Supporting Information  
**Checkpoint Status:** COMPLETE  
**Date:** 22-08-2026  
**Time:** 13:51

---

## 1. Objective

Implement employee incident attachments/supporting information while preserving the existing M2 incident workflow and architecture.

The implementation was added as an M2 extension and does not rebuild the existing incident-management module.

---

## 2. Requirement Basis

M2 employee responsibilities include supporting information/attachments associated with incidents.

The existing project requirements did not specify:

- Exact physical storage mechanism
- Exact allowed file extensions
- Exact maximum file size
- Exact attachment metadata schema

Therefore, these implementation details were treated explicitly as design decisions rather than requirements.

---

## 3. Design Decisions

### Storage

Attachment metadata is stored in SQL Server.

The uploaded physical file is stored by the Web application under the configured incident-upload directory.

```text
SQL Server
    └── IncidentAttachments
          └── attachment metadata

AIITSM.Web
    └── wwwroot/uploads/incidents/
          └── physical uploaded files
```

### File size

Maximum allowed upload size:

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

### Stored filename

The original filename is retained as metadata.

A unique generated filename is used for the physical stored file to avoid filename collisions.

---

## 4. Implementation

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

### DbContext

Updated the existing shared:

```text
AIITSM.Infrastructure/
└── 06_M6_AI/
    └── AIITSMDbContext.cs
```

with the `IncidentAttachments` DbSet.

### Web

Created:

```text
AIITSM.Web/
└── Controllers/
    └── 02_M2_IncidentManagement_2/
        └── IncidentAttachmentController.cs
```

Updated the existing:

```text
AIITSM.Web/
└── Views/
    └── Incident/
        └── Details.cshtml
```

to integrate the attachment upload and attachment-list UI without removing the existing Communication section.

Created the attachment partial view:

```text
AIITSM.Web/
└── Views/
    └── IncidentAttachment/
        └── _Attachments.cshtml
```

---

## 5. Upload Workflow

The completed workflow is:

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

---

## 6. Ownership Protection

The upload operation verifies that the current employee owns the incident before accepting the attachment.

The implementation uses the existing current-user/incident ownership workflow rather than trusting a user identifier submitted by the browser.

---

## 7. Database Change

The original project database script did not contain an `IncidentAttachments` table.

A new M2 database extension was therefore required.

Added to the project's database source script:

```text
dbo.IncidentAttachments
```

with:

```text
AttachmentId
IncidentId
FileName
StoredFileName
ContentType
FileSize
UploadedAt
```

Relationship:

```text
IncidentAttachments.IncidentId
            ↓
     Incidents.IncidentId
```

An index was also added on:

```text
IncidentAttachments.IncidentId
```

### Database correction performed during testing

The table was initially created in the SQL Server `master` database because the SQL query window was using `master`.

This was identified through:

```text
SELECT DB_NAME()
```

The incorrect table was removed.

The table was then correctly created under:

```text
ITServiceDesk.dbo.IncidentAttachments
```

Final verification confirmed:

```text
CurrentDatabase = ITServiceDesk
SchemaName      = dbo
TableName       = IncidentAttachments
```

---

## 8. Build Testing

The project successfully built after the attachment implementation.

Verified components:

```text
Domain                         PASS
EF configuration              PASS
DbContext                     PASS
Application DTO               PASS
Application interface         PASS
Infrastructure service        PASS
Dependency injection          PASS
Web controller                PASS
Incident Details UI           PASS
Project build                 PASS
```

---

## 9. End-to-End Upload Test

Test file:

```text
wifi.txt
```

Incident:

```text
IncidentId = 4
```

The Web UI displayed:

```text
Attachment uploaded successfully.
```

The attachment also appeared under:

```text
Uploaded Attachments
```

Displayed metadata included:

```text
wifi.txt
text/plain
0.0 KB
22 Aug 2026 13:49
```

---

## 10. SQL Verification

The final SQL verification returned:

```text
AttachmentId     1
IncidentId       4
FileName         wifi.txt
StoredFileName   b4646f8e7d6455ba92884ab3b80c158.txt
ContentType      text/plain
FileSize         35
UploadedAt       2026-08-22 13:49:46.253
```

This confirms that attachment metadata was successfully persisted in:

```text
ITServiceDesk.dbo.IncidentAttachments
```

---

## 11. Final Verification

The complete feature path was successfully verified:

```text
Employee
   ↓
Incident Details
   ↓
Choose File
   ↓
Upload Attachment
   ↓
Controller
   ├── Ownership verification       PASS
   ├── File validation              PASS
   ├── Physical storage             PASS
   └── SQL metadata persistence     PASS
           ↓
   IncidentAttachments
           ↓
   Attachment list                  PASS
```

---

## 12. Current Status

### M2 Extensions + Integration

```text
Communication / Comments       COMPLETE
Notifications / Updates       COMPLETE
Attachments                    COMPLETE
Feedback                       PENDING
M2 ↔ M6 Integration            PENDING
Integration Testing            PENDING
Final M2 Checkpoint            PENDING
```

### Attachment Feature

**M2-2.3 Attachments / Supporting Information — COMPLETE**

The feature has been implemented, integrated into Incident Details, built successfully, and verified end-to-end against SQL Server.

---

## 13. Remaining Work

The following are intentionally not included in this checkpoint:

```text
Attachment download feature
Advanced file preview
Cloud/object storage
Virus scanning
AI analysis of uploaded files
Additional attachment management features
```

These were not required for this M2 attachment implementation and were not silently introduced.

Future additions require requirement/design verification first.

---

## 14. Checkpoint Conclusion

M2-2.3 — Attachments / Supporting Information is complete.

The implementation remains within the M2 extension area and uses the existing project architecture.

No unrelated module was rebuilt.

The next M2 workstream can proceed from this stable checkpoint.
