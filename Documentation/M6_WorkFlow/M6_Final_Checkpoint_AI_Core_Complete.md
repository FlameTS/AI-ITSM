# M6 — Final Checkpoint: AI Core Complete

**Project:** AI-Powered IT Service Management and Incident Resolution Platform (AI-ITSM)  
**Module:** M6 — AI Assistance  
**Checkpoint:** M6 Independent Core Implementation Complete  
**Date:** 21-08-2026

---

## 1. Checkpoint Purpose

This document records the final independent checkpoint for M6 before the module becomes dependent on integration with the other team members' modules.

The M6 AI core is working independently and has been tested end-to-end using a controlled test incident.

At this checkpoint, M6 should be treated as a stable baseline.

Further M6 requirements that depend on the real Incident Management, Authentication/Authorization, Workflow, and Conversation implementations should be resumed only after the relevant team modules are ready for integration.

---

# 2. M6 Requirement Status

| Requirement | Description | Current Status |
|---|---|---|
| AI-01 | The system shall analyze newly submitted incident descriptions. | CORE IMPLEMENTED |
| AI-02 | The system shall suggest an appropriate incident category. | CORE IMPLEMENTED |
| AI-03 | The system shall suggest an appropriate priority or severity level. | CORE IMPLEMENTED |
| AI-04 | The system shall suggest possible resolutions based on available support information. | CORE IMPLEMENTED |
| AI-05 | The system shall identify potentially related or duplicate incidents. | WAITING FOR INTEGRATION |
| AI-06 | The system shall generate summaries of lengthy incident conversations. | WAITING FOR INTEGRATION |
| AI-07 | The system shall provide an AI-based assistant for common IT support queries. | WAITING FOR INTEGRATION / SCOPE CONFIRMATION |
| AI-08 | Authorized support personnel shall be able to accept an AI recommendation. | WAITING FOR AUTHORIZATION/WORKFLOW INTEGRATION |
| AI-09 | Authorized support personnel shall be able to override an AI recommendation. | WAITING FOR AUTHORIZATION/WORKFLOW INTEGRATION |
| AI-10 | AI recommendations shall be presented as assistance and shall never automatically override a human decision. | CORE PRINCIPLE IMPLEMENTED; FINAL SYSTEM VERIFICATION PENDING |

---

# 3. Completed M6 Core

The following independent M6 implementation is complete:

```text
Incident Analysis Request
        ↓
AIAnalysisService
        ↓
IAIProvider
        ↓
GeminiProvider
        ↓
Google.GenAI 1.19.0
        ↓
Gemini API
        ↓
Structured AIProviderResult
        ↓
AIAnalysis
        ↓
EF Core
        ↓
SQL Server
```

The implementation provides:

- Incident analysis
- Suggested category
- Suggested priority
- Suggested resolution
- Confidence score
- Pending/Completed/Failed analysis status
- AI provider abstraction
- Gemini provider implementation
- Structured JSON output
- SQL Server persistence
- Secure local API-key configuration
- Dependency Injection

---

# 4. Application Layer

Created:

```text
Source/AIITSM/AIITSM.Application/
└── 06_M6_AI/
    ├── Contracts/
    │   └── AnalyzeIncidentRequest.cs
    │
    ├── Providers/
    │   ├── IAIProvider.cs
    │   ├── AIProviderRequest.cs
    │   └── AIProviderResult.cs
    │
    └── Services/
        └── IAIAnalysisService.cs
```

## Provider abstraction

The Application layer exposes:

```csharp
Task<AIProviderResult> AnalyzeIncidentAsync(
    AIProviderRequest request);
```

The Application layer does not directly depend on Google's Gemini SDK.

This keeps the external AI provider implementation in Infrastructure.

---

# 5. Infrastructure Layer

Created/implemented:

```text
Source/AIITSM/AIITSM.Infrastructure/
└── 06_M6_AI/
    ├── Configurations/
    ├── Services/
    │   └── AIAnalysisService.cs
    └── Providers/
        └── GeminiProvider.cs
```

## AIAnalysisService

The service now:

1. Creates an `AIAnalysis` record.
2. Sets the initial status to `Pending`.
3. Saves the initial record.
4. Converts the incident request into an `AIProviderRequest`.
5. Calls `IAIProvider`.
6. Receives the structured AI result.
7. Copies the AI result into `AIAnalysis`.
8. Sets the analysis status to `Completed`.
9. Saves the completed analysis.

If the provider fails, the analysis is marked as:

```text
Failed
```

before the exception is rethrown.

---

# 6. Gemini Integration

NuGet package installed in:

```text
AIITSM.Infrastructure
```

Package:

```text
Google.GenAI
Version: 1.19.0
```

The package is intentionally not installed in Application or Domain.

Runtime architecture:

```text
Application
    ↓
IAIProvider
    ↓
Infrastructure
    ↓
GeminiProvider
    ↓
Google.GenAI 1.19.0
    ↓
Gemini API
```

No custom ML model was trained or fine-tuned.

The project uses a hosted Gemini model through its API to keep implementation and deployment simple.

---

# 7. Structured Gemini Output

Gemini is requested to return structured JSON containing:

```json
{
  "suggestedCategory": "...",
  "suggestedPriority": "...",
  "suggestedResolution": "...",
  "confidenceScore": 0.0
}
```

A real Gemini provider test returned:

```json
{
  "suggestedCategory": "Network",
  "suggestedPriority": "Medium",
  "suggestedResolution": "Verify internet connectivity, check VPN client configuration, and ensure credentials are correct. Restart the VPN service or reinstall the client if necessary.",
  "confidenceScore": 0.95
}
```

This proves that the Gemini API connection and structured response handling work.

---

# 8. JSON Deserialization

An initial response produced empty C# properties because the Gemini JSON used camelCase while the C# properties used PascalCase.

The deserialization was corrected using:

```csharp
new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true
}
```

After the correction, populated AI values were successfully returned.

---

# 9. Secret Management

Local Gemini authentication uses:

```text
GEMINI_API_KEY
```

A repository-root `.env` file is used for the real local development secret.

A `.env.example` file contains only a placeholder.

The real `.env` is excluded through `.gitignore`.

`DotNetEnv` is used by the Web application to load the environment variable.

The Gemini API key is not hardcoded in source code.

---

# 10. Dependency Injection

The Web application registers the M6 services:

```csharp
builder.Services.AddScoped<IAIAnalysisService, AIAnalysisService>();
builder.Services.AddScoped<IAIProvider, GeminiProvider>();
```

This allows ASP.NET Core to resolve the complete M6 dependency chain.

---

# 11. End-to-End Verification

The integrated test endpoint returned:

```text
AIAnalysis created successfully. ID = 6
```

The correct SQL Server database was then verified.

## Database connection verification

```text
ServerName   = LAPTOP-P9UE95IH
DatabaseName = ITServiceDesk
```

The database contained:

```text
TotalRows       = 6
MaxAIAnalysisId = 6
```

## Verified M6 record

```text
AIAnalysisId        = 6
IncidentId          = 1
SuggestedCategory   = Software
SuggestedPriority   = Low
SuggestedResolution = Confirm test completion and close the incident.
ConfidenceScore     = 0.95
Status              = Completed
CreatedAt           = 2026-08-21 13:57:07.920
```

This confirms that the Gemini-generated result was successfully persisted to SQL Server.

---

# 12. Important Debugging Finding

During verification, SSMS was initially querying:

```text
Database = master
```

while the ASP.NET Core application was configured to use:

```text
Database = ITServiceDesk
```

This caused the apparent discrepancy where the application returned `AIAnalysisId = 6` but the SQL query initially showed only IDs 1–3.

After switching the SQL query to the correct `ITServiceDesk` database, ID 6 was confirmed with:

```text
Status = Completed
```

and populated AI fields.

---

# 13. Test Controllers

Temporary development controllers were used to prove M6 independently:

```text
Controllers/
└── 06_M6_AI/
    ├── AIAnalysisTestController.cs
    └── GeminiTestController.cs
```

They were useful for controlled testing of:

```text
Persistence
```

and:

```text
Gemini Provider
```

and finally the integrated M6 flow.

These controllers should be reviewed later and either removed or replaced with the final production-facing M6 API/UI once the overall system integration is performed.

---

# 14. AI-10 Design Principle

The current M6 core treats AI output as a recommendation.

Gemini produces:

```text
SuggestedCategory
SuggestedPriority
SuggestedResolution
ConfidenceScore
```

The AI does not directly modify or override a human decision.

Therefore the current architecture supports:

```text
AI
 ↓
Recommendation
 ↓
Human decision
```

rather than:

```text
AI
 ↓
Automatic override
```

The final AI-10 verification will occur when M6 is integrated with the actual authorization, incident workflow, and user interface.

---

# 15. What Is Deliberately NOT Implemented Yet

The following are intentionally left for the integration phase:

## AI-05 — Related/Duplicate Incidents

Requires the actual Incident Management data and workflow from the relevant team module.

Do not create a duplicate incident repository inside M6.

## AI-06 — Conversation Summarization

Requires the actual conversation/message structure and workflow.

## AI-07 — AI Support Assistant

Requires the confirmed support-information source and final application interaction design.

## AI-08 — Accept Recommendation

Requires the actual authorization and incident workflow.

## AI-09 — Override Recommendation

Requires the actual authorization and incident update workflow.

## AI-10 — Final Human-Control Verification

The core architecture follows the human-assistance principle, but final system verification must occur after integration with the real workflow.

---

# 16. M6 Stopping Point

M6 has reached the appropriate independent development stopping point.

```text
M6 AI Core
    ↓
AI-01 ─── Complete
AI-02 ─── Complete
AI-03 ─── Complete
AI-04 ─── Complete
    ↓
Gemini Integration
    ↓
SQL Server Persistence
    ↓
End-to-End Verification
    ↓
⭐ M6 CORE COMPLETE
    ↓
WAIT FOR TEAM INTEGRATION
```

The next M6 implementation phase should begin only when the required team modules are available.

---

# 17. Integration Dependencies

The next phase will require coordination with:

### M1 — Identity & Access

Needed for:

- Authorized support personnel
- Accept recommendation
- Override recommendation
- Role-based access

### M2 — Incident Management

Needed for:

- Real submitted incidents
- Real incident descriptions
- Related/duplicate incident analysis
- Applying accepted AI recommendations

### M3 — Agent Workflow

Needed for:

- Agent-facing review
- Recommendation handling
- Human acceptance/override workflow
- Agent interaction with AI results

### Conversation/Communication Implementation

Needed for:

- AI-06 conversation summarization

---

# 18. Final M6 Status

## Independent M6 Core

**STATUS: COMPLETE**

The following are proven:

```text
C# / ASP.NET Core
        ↓
AI Provider Abstraction
        ↓
Gemini Provider
        ↓
Google.GenAI 1.19.0
        ↓
Gemini API
        ↓
Structured AI Result
        ↓
AIAnalysis
        ↓
SQL Server
```

## Overall M6

**STATUS: PARTIALLY COMPLETE — WAITING FOR TEAM INTEGRATION**

This is intentional.

The independent AI core is complete, while AI-05 through AI-09 require real system data/workflows that are being developed by other modules.

---

# 19. Recommended Next Action

Do not add temporary implementations for AI-05, AI-06, AI-08, or AI-09 just to mark requirements as complete.

Instead:

1. Preserve the current M6 implementation as the stable baseline.
2. Commit the M6 changes to the team repository.
3. Keep the checkpoint documentation with the project.
4. Coordinate with the M1/M2/M3 members.
5. Resume M6 when the required modules are ready.
6. Perform full integration testing.
7. Complete AI-05 through AI-10 using the real system workflows.
8. Perform final system testing.
9. Proceed toward deployment.

---

# 20. Milestone Conclusion

M6 has successfully progressed from an empty AI module to a working Gemini-powered AI analysis core.

The project has demonstrated a real end-to-end AI operation:

```text
Test Incident
    ↓
AIAnalysisService
    ↓
GeminiProvider
    ↓
Gemini API
    ↓
Category / Priority / Resolution / Confidence
    ↓
AIAnalysis
    ↓
SQL Server
    ↓
Status = Completed
```

The verified database result for `AIAnalysisId = 6` demonstrates that the AI result is not merely displayed or simulated; it is persisted by the application.

**M6 independent core implementation is therefore complete and ready for team-level integration.**
