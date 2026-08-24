# AI-ITSM M7 Integration Checkpoint — n8n Automation Webhooks

## Status

**Checkpoint:** M7 n8n automation webhook integration completed and tested  
**Status:** Completed  
**Technology:** n8n + ASP.NET Core AutomationController  
**Date:** 2026-08-24

---

## 1. Objective

Integrate the M7 automation workflows with the AI-ITSM ASP.NET Core application using n8n Webhook triggers and HTTP Request nodes.

The workflows receive automation events through authenticated n8n webhooks and call the corresponding ASP.NET Core `AutomationController` endpoints.

---

## 2. Authentication

All n8n webhook triggers use Header Authentication.

### Header

```text
X-AIITSM-Webhook-Secret
```

### Configured secret

```text
AITSM-M7-n8n-secret-2026
```

The secret is supplied by the caller in the request header and validated by the n8n Webhook node.

---

## 3. Completed Webhook Workflows

### 3.1 Assignment Notification

**Webhook path**

```text
aitsm/assignment
```

**Method**

```text
POST
```

**Input payload**

```json
{
  "incidentId": 1,
  "assignedTo": 1
}
```

**ASP.NET Core endpoint**

```text
POST /Automation/SendAssignmentNotification
```

**Parameters**

- `incidentId`
- `assignedTo`

**Verified result**

```text
Assignment notification sent successfully.
```

---

### 3.2 Status Change Notification

**Webhook path**

```text
aitsm/status-change
```

**Method**

```text
POST
```

**Input payload**

```json
{
  "incidentId": 1,
  "userId": 1,
  "newStatus": "Resolved"
}
```

**ASP.NET Core endpoint**

```text
POST /Automation/SendStatusChangeNotification
```

**Parameters**

- `incidentId`
- `userId`
- `newStatus`

**Verified result**

```text
Status change notification sent successfully.
```

---

### 3.3 Critical Incident Notification

**Webhook path**

```text
aitsm/critical
```

**Method**

```text
POST
```

**Input payload**

```json
{
  "incidentId": 1,
  "userId": 1
}
```

**ASP.NET Core endpoint**

```text
POST /Automation/SendCriticalIncidentNotification
```

**Parameters**

- `incidentId`
- `userId`

**Verified result**

```text
Critical incident notification sent successfully.
```

---

### 3.4 Incident Escalation

**Webhook path**

```text
aitsm/escalation
```

**Method**

```text
POST
```

**Input payload**

```json
{
  "incidentId": 1,
  "escalatedBy": 1,
  "escalatedTo": 1,
  "reason": "Critical incident requires immediate escalation"
}
```

**ASP.NET Core endpoint**

```text
POST /Automation/EscalateIncident
```

**Parameters**

- `incidentId`
- `escalatedBy`
- `escalatedTo`
- `reason`

**Verified result**

```text
Incident escalated successfully.
```

---

## 4. Important Endpoint Correction

During escalation integration, the initially assumed endpoint was:

```text
/Automation/SendEscalationNotification
```

This endpoint did not exist in the provided `AutomationController`.

The actual M7 controller contains:

```csharp
[HttpPost]
public async Task<IActionResult> EscalateIncident(
    int incidentId,
    int escalatedBy,
    int escalatedTo,
    string reason)
{
    await _automationService.EscalateIncidentAsync(
        incidentId,
        escalatedBy,
        escalatedTo,
        reason);

    return Ok("Incident escalated successfully.");
}
```

Therefore the final n8n HTTP Request configuration uses:

```text
POST https://localhost:7002/Automation/EscalateIncident
```

This was successfully tested.

---

## 5. n8n HTTP Request Configuration Pattern

The HTTP Request nodes use:

- HTTP method matching the ASP.NET controller action (`POST`)
- HTTPS localhost ASP.NET endpoint
- Query parameters mapped from the Webhook body
- Header authentication secret passed from the Webhook input
- `Ignore SSL Issues (Insecure)` enabled for the local HTTPS development environment

Example expression:

```text
{{ $json.body.incidentId }}
```

The same mapping pattern is used for the other webhook parameters.

---

## 6. Testing

PowerShell was used to send test requests to the n8n test webhook URLs.

Example escalation test:

```powershell
$secret = "AITSM-M7-n8n-secret-2026"

Invoke-RestMethod `
  -Uri "http://localhost:5678/webhook-test/aitsm/escalation" `
  -Method POST `
  -Headers @{ "X-AIITSM-Webhook-Secret" = $secret } `
  -ContentType "application/json" `
  -Body '{"incidentId":1,"escalatedBy":1,"escalatedTo":1,"reason":"Critical incident requires immediate escalation"}'
```

The webhook successfully received the request and the downstream HTTP Request node returned:

```text
Incident escalated successfully.
```

---

## 7. Test-Mode Note

The n8n `/webhook-test/` URLs are intended for test execution while the workflow is listening for a test event.

For the final deployed/active workflow, the Production Webhook URL should be used after the workflow is published/activated according to the project's deployment setup.

---

## 8. Backup

The completed n8n workflows have been downloaded as JSON files.

These JSON exports should be retained as the M7 workflow backup and can be re-imported into n8n if required.

---

## 9. Current M7 Status

| M7 Component | Status |
|---|---|
| Assignment webhook | Completed and tested |
| Status-change webhook | Completed and tested |
| Critical-incident webhook | Completed and tested |
| Escalation webhook | Completed and tested |
| Header authentication | Configured |
| ASP.NET endpoint integration | Verified |
| n8n JSON backups | Downloaded |
| M7 checkpoint documentation | Completed |

---

## 10. Next Step

This checkpoint completes the current M7 n8n webhook integration/testing work.

**Do not redesign or rebuild these workflows.**

The next project step should proceed from this completed checkpoint and focus on the remaining M4/M5 integration and overall AI-ITSM integration/testing work according to the established module ownership and integration plan.
