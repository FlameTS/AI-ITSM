# M7 n8n Notification Integration — Checkpoint

## Status

**Completed successfully**

## Completed Work

The first two M7 automation workflows have been implemented and tested using n8n.

### 1. Assignment Notification

```text
Webhook → HTTP Request → ASP.NET Core → AutomationController
→ IAutomationService → AutomationService → AIITSMDbContext → Notifications
```

Webhook:
`POST /webhook/aitsm/assignment`

Payload:
```json
{"incidentId":1,"assignedTo":1}
```

ASP.NET endpoint:
`POST /Automation/SendAssignmentNotification`

Result: **PASS**

### 2. Status Change Notification

```text
Webhook → HTTP Request → ASP.NET Core → AutomationController
→ IAutomationService → AutomationService → AIITSMDbContext → Notifications
```

Webhook:
`POST /webhook/aitsm/status-change`

Payload:
```json
{"incidentId":1,"userId":1,"newStatus":"Resolved"}
```

ASP.NET endpoint:
`POST /Automation/SendStatusChangeNotification`

Result: **PASS**

## Authentication

n8n Webhook authentication uses the Header Auth credential with:

`X-AIITSM-Webhook-Secret`

## Local Development HTTPS

The local ASP.NET application uses a development HTTPS certificate. n8n required **Ignore SSL Issues (Insecure)** for the local HTTP Request connection.

This is a local-development workaround and is not intended as the final deployment security configuration.

## Reuse / Database Impact

- Existing M7 `AutomationService` reused.
- Existing `IAutomationService` reused.
- Existing `AutomationController` reused.
- Existing `AIITSMDbContext` reused.
- Existing `Notifications` table reused.
- No new database.
- No Database.sql change.
- No migration.
- No M1–M6 redesign.

## Current Status

```text
Assignment Notification        ✅
Status Change Notification     ✅
Critical Notification          ⏳
Automated Escalation            ⏳
AR-06 Automation Logging        ⏸ Deferred
```

AR-06 logging remains deferred because no finalized logging structure was specified.

## Next Step

Implement and test the **Critical Incident Notification** n8n workflow using the same minimal pattern:

```text
Webhook → HTTP Request → existing M7 Automation endpoint
```

Existing ASP.NET endpoint:

`POST /Automation/SendCriticalIncidentNotification`
