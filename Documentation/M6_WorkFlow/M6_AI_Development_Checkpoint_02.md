# M6 AI — Development Checkpoint 02

**Project:** AI-Powered IT Service Desk & Incident Management System (AI-ITSM)  
**Module:** M6 — AI Assistance  
**Scope:** College Project / Demonstrable Implementation  
**Status:** Approved Working Direction  
**Version:** 0.1  
**Date:** August 2026

## 1. Purpose

This document records the M6 development direction established after completing the initial Domain work and beginning the Application layer.

The goal is to keep M6 practical, implementable, explainable, and suitable for a university project while maintaining the agreed modular architecture.

## 2. Scope Adjustment

M6 will be developed primarily as a **working college-project implementation**, not as a production enterprise platform.

The priority is:

```text
Working
   +
Explainable
   +
Organized
   +
Demonstrable
```

We will avoid unnecessary enterprise complexity when it does not provide meaningful value to the project.

## 3. Practical Development Target

The initial target is a working M6 implementation rather than a highly optimized production system.

```text
Domain
   ✓
   ↓
Application
   ↓
Gemini integration
   ↓
Working AI analysis
   ↓
Database persistence
   ↓
Web/demo integration
   ↓
Additional AI capabilities
```

A basic working implementation should be achieved before optional improvements are attempted.

## 4. Primary AI Strategy

### Primary

Use the **Gemini API** for the main AI functionality.

Gemini is expected to handle core generative/reasoning tasks such as:

- Incident analysis
- Category suggestion
- Priority suggestion
- Resolution suggestion
- Summarization
- Support-assistant functionality

### Optional / Bonus

Python/Jupyter and custom ML models may be used for:

- Classification experiments
- Similarity experiments
- Dataset analysis
- Model comparison

Custom ML is **not a blocker** for the initial M6 implementation.

The project should have working AI functionality before spending time on custom ML.

## 5. AI-01 to AI-04 Simplification

For the initial implementation, AI-01 through AI-04 do not need four completely independent AI pipelines.

A single structured AI analysis can produce multiple fields:

```text
Incident
   ↓
Gemini
   ↓
Structured result
   ├── Suggested Category
   ├── Suggested Priority
   ├── Suggested Resolution
   └── Confidence Score
```

C# can map the structured response into the `AIAnalysis` domain model.

## 6. AI-05 Scope

AI-05 — related/duplicate incident detection — will initially be implemented practically.

```text
Incident
   ↓
Similarity analysis
   ↓
Candidate incidents
   ↓
Related / Duplicate classification
```

Advanced embedding/vector-search/RAG approaches may be added if time permits.

They are not required before the basic M6 pipeline works.

## 7. Background Processing

The approved direction remains:

```text
Incident saved
      ↓
AI analysis requested
      ↓
Create AIAnalysis
Status = Pending
      ↓
Return to caller
      ↓
Background processing
      ↓
Status = Processing
      ↓
AI provider
      ↓
Completed / Failed
```

AI processing must not determine whether the original incident is successfully created.

If AI processing fails:

```text
Incident = remains valid

AIAnalysis = Failed
```

## 8. Application Layer Progress

The first Application contract has been created:

```text
AIITSM.Application
└── 06_M6_AI
    └── Contracts
        └── AnalyzeIncidentRequest.cs
```

Current input:

```text
AnalyzeIncidentRequest
├── IncidentId
├── Title
└── Description
```

These are the currently confirmed minimum inputs for the initial incident-analysis use case.

## 9. Attachments / Images

The requirements confirm that an incident can contain relevant supporting information.

However, the current requirements do not specifically establish:

- an image-only attachment model;
- attachment storage implementation;
- mandatory AI analysis of attachments.

Therefore, image/multimodal processing is **not part of the first M6 implementation**.

It remains a future extension:

```text
Current:
Title + Description
        ↓
       M6

Future:
Title + Description + Supporting Information
        ↓
   Multimodal AI
```

Attachment storage should be coordinated with M2 rather than creating a separate attachment system inside M6.

## 10. M2 Situation

M2 Incident Management is currently not implemented by the original assigned member.

The M2 implementation will now also be handled by the M6 developer.

This does not change module ownership.

M2 remains responsible for incident-related concepts, while M6 remains responsible for AI concepts.

The modules will be wired together after their internal functionality is established.

## 11. Current M6 Architecture

```text
AIITSM
│
├── AIITSM.Domain
│   └── 06_M6_AI
│       ├── AIAnalysis
│       ├── AIAnalysisStatus
│       ├── AIAnalysisRelatedIncident
│       └── AIIncidentRelationshipType
│
├── AIITSM.Application
│   └── 06_M6_AI
│       └── Contracts
│           └── AnalyzeIncidentRequest
│
├── AIITSM.Infrastructure
│   └── 06_M6_AI
│
└── AIITSM.Web
    └── Controllers
        └── 06_M6_AI
```

## 12. Current Development Status

### Completed

```text
✓ M6 module boundary
✓ AI capability mapping
✓ Gemini/custom-ML strategy
✓ Background AI-processing direction
✓ AI analysis history decision
✓ AI analysis lifecycle decision
✓ Related/duplicate incident design
✓ Initial M6 Domain model
✓ AIAnalysis.cs
✓ AIAnalysisStatus.cs
✓ AIAnalysisRelatedIncident.cs
✓ AIIncidentRelationshipType.cs
✓ Initial Application contract
✓ AnalyzeIncidentRequest.cs
```

### Current

```text
→ M6 Application service design
```

### Next

```text
IAIAnalysisService
        ↓
Create Pending AIAnalysis
        ↓
Background processing contract
        ↓
Gemini provider abstraction
        ↓
First working AI analysis
```

## 13. Development Principle

For every major piece of work:

```text
Discuss
   ↓
Decide
   ↓
Implement
   ↓
Test
   ↓
Document
   ↓
Commit
```

The `.md` files are the consolidated record of approved decisions and completed work, not a transcript of every idea discussed.

## 14. Current Priority

The immediate priority is:

> **Get the first complete M6 AI analysis pipeline working in C# / ASP.NET Core.**

Do not delay the first working pipeline for optional ML, multimodal processing, advanced RAG, or enterprise-grade infrastructure.

Once the basic pipeline works, improvements can be added if time permits.
