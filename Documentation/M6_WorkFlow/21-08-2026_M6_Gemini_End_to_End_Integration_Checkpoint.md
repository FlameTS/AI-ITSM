# M6 — Gemini End-to-End Integration Checkpoint

**Date:** 21-08-2026  
**Module:** M6 — AI Assistance  
**Milestone:** Gemini Provider Integrated with AIAnalysisService

## 1. Milestone Summary

A major M6 implementation milestone has been reached.

The project now has a working AI provider abstraction and a working Gemini integration using C#, ASP.NET Core, `Google.GenAI` version `1.19.0`, Gemini API, SQL Server, and Entity Framework Core.

The Gemini provider has been successfully called independently and returned a populated structured result.

The `AIAnalysisService` has also been updated to call the provider and persist the AI result.

**Important verification note:** the integrated browser test returned `AIAnalysisId = 6`, but the SQL Server screenshot supplied immediately afterward still displayed older rows (IDs 3, 2, 1). Therefore, the database row for ID 6 has not yet been visually confirmed in the supplied evidence. This checkpoint does not claim that row 6 has already been verified.

## 2. Architecture Implemented

```text
AnalyzeIncidentRequest
        |
        v
AIAnalysisService
        |
        +----------------------+
        |                      |
        v                      v
Create Pending          IAIProvider
AIAnalysis                    |
        |                      v
        |               GeminiProvider
        |                      |
        |                      v
        |              Google.GenAI 1.19.0
        |                      |
        |                      v
        |                  Gemini API
        |                      |
        |                      v
        |               AIProviderResult
        |                      |
        +----------<-----------+
        |
        v
Update AIAnalysis
        |
        v
SQL Server
```

Application contains the provider abstraction. Infrastructure contains the Gemini implementation and EF Core persistence.

## 3. Application Provider Contract

Created:

```text
Source/AIITSM/AIITSM.Application/
└── 06_M6_AI/
    └── Providers/
        ├── IAIProvider.cs
        ├── AIProviderRequest.cs
        └── AIProviderResult.cs
```

`IAIProvider` exposes:

```csharp
Task<AIProviderResult> AnalyzeIncidentAsync(
    AIProviderRequest request);
```

The rest of the application therefore does not depend directly on Gemini.

## 4. Gemini Provider

Created:

```text
Source/AIITSM/AIITSM.Infrastructure/
└── 06_M6_AI/
    └── Providers/
        └── GeminiProvider.cs
```

The implementation uses:

```text
Google.GenAI 1.19.0
```

The provider:

1. Receives incident ID, title, and description.
2. Builds the incident analysis prompt.
3. Requests structured JSON output.
4. Defines a response schema.
5. Calls Gemini.
6. Safely extracts returned text.
7. Deserializes JSON into `AIProviderResult`.
8. Returns the structured result.

## 5. Actual Gemini Test Result

A direct Gemini provider test successfully returned:

```json
{
  "suggestedCategory": "Network",
  "suggestedPriority": "Medium",
  "suggestedResolution": "Verify internet connectivity, check VPN client configuration, and ensure credentials are correct. Restart the VPN service or reinstall the client if necessary.",
  "confidenceScore": 0.95
}
```

This proves Gemini API connectivity, structured output, and JSON-to-C# deserialization.

## 6. JSON Deserialization Fix

The first Gemini response produced empty C# properties because Gemini returned camelCase JSON while the C# properties used PascalCase.

The deserialization was corrected using:

```csharp
new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true
}
```

After this correction, populated AI values were returned successfully.

## 7. Secret Management

A repository-root `.env` file is used for local development.

Environment variable:

```text
GEMINI_API_KEY
```

A `.env.example` template contains only a placeholder and no real secret.

The real `.env` is ignored by Git.

`DotNetEnv` loads the environment variables before the ASP.NET Core application starts.

The API key is not hardcoded in source code.

## 8. AIAnalysisService Integration

The Infrastructure `AIAnalysisService` was extended from persistence-only behavior to AI-integrated behavior.

It now:

1. Creates an `AIAnalysis` with the requested `IncidentId`.
2. Sets status to `Pending`.
3. Saves the initial record.
4. Builds an `AIProviderRequest`.
5. Calls `IAIProvider`.
6. Receives `AIProviderResult`.
7. Copies category, priority, resolution, and confidence into `AIAnalysis`.
8. Sets status to `Completed`.
9. Saves the completed result.

If the provider fails, the service sets the analysis status to `Failed`, saves it, and rethrows the exception.

## 9. Dependency Injection

The Web project registers:

```csharp
builder.Services.AddScoped<IAIAnalysisService, AIAnalysisService>();
builder.Services.AddScoped<IAIProvider, GeminiProvider>();
```

Therefore ASP.NET Core can resolve the complete M6 dependency chain.

## 10. Test Controllers

Temporary development test controllers were used to prove individual and integrated behavior without interfering with other modules.

### Persistence test

```text
Controllers/06_M6_AI/AIAnalysisTestController.cs
```

### Gemini test

```text
Controllers/06_M6_AI/GeminiTestController.cs
```

These are development/test utilities and are not automatically considered the final production API design.

## 11. Integrated Test Evidence

The integrated test endpoint returned:

```text
AIAnalysis created successfully. ID = 6
```

This proves the integrated service execution returned a generated `AIAnalysisId`.

However, the supplied SQL screenshot immediately after the test still showed:

```text
AIAnalysisId
3
2
1
```

Therefore the SQL result set appears not to have been refreshed/re-run after the integrated test.

## 12. Required Final Verification

Run:

```sql
SELECT TOP 10
    AIAnalysisId,
    IncidentId,
    SuggestedCategory,
    SuggestedPriority,
    SuggestedResolution,
    ConfidenceScore,
    Status,
    CreatedAt
FROM AIAnalysis
ORDER BY AIAnalysisId DESC;
```

The newest row should be checked for:

```text
AIAnalysisId = 6
Status = Completed
SuggestedCategory = populated
SuggestedPriority = populated
SuggestedResolution = populated
ConfidenceScore = populated
```

The exact AI-generated values may vary between calls.

## 13. Current M6 Status

### Completed

- M6 domain model
- M6 database structure
- M6 EF Core configuration
- M6 SQL Server connection
- `IAIAnalysisService`
- `AIAnalysisService`
- AI provider abstraction
- `AIProviderRequest`
- `AIProviderResult`
- `GeminiProvider`
- `Google.GenAI` 1.19.0 integration
- Gemini API authentication
- `.env` secret management
- Structured Gemini JSON output
- JSON deserialization
- Dependency injection
- Gemini provider test
- AIAnalysisService/Gemini integration
- Build verification

### Pending verification

- Confirm `AIAnalysisId = 6` in SQL Server
- Confirm AI fields are persisted for row 6
- Confirm final status is `Completed`

### Not yet implemented

- Background processing
- Production API design
- AI result review UI
- Accept recommendation workflow
- Override workflow
- Related/duplicate incident AI
- Conversation summarization
- AI assistant
- Full M6 automated testing
- Production deployment

## 14. Architectural Decisions

### API-based Gemini

Use a hosted Gemini API instead of training or hosting a custom model because the project prioritizes a fast, simple, deployable AI implementation.

### Provider abstraction

Gemini is hidden behind:

```text
IAIProvider
```

so Application does not become directly dependent on Google-specific SDK types.

### Structured output

Gemini is requested to return:

- category
- priority
- resolution
- confidence

as structured JSON.

### Human-controlled AI

AI output remains a recommendation. The AI does not independently override human decisions.

## 15. Milestone Conclusion

M6 has progressed from a database-only AIAnalysis structure to a functioning Gemini-backed AI analysis pipeline.

The most significant proven capability is:

```text
IT Incident
    ↓
C# Application
    ↓
AI Provider Abstraction
    ↓
Gemini Provider
    ↓
Google.GenAI 1.19.0
    ↓
Gemini API
    ↓
Structured AI Recommendation
```

The immediate remaining task is to refresh/query SQL Server and verify that `AIAnalysisId = 6` contains the Gemini-generated fields and `Status = Completed`.
