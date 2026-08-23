# M6 AI --- AI Capability & Technology Blueprint

**Project:** AI-Powered IT Service Desk & Incident Management System\
**Module:** M6 --- AI Assistance\
**Document:** AI Capability & Technology Blueprint\
**Version:** 0.1\
**Status:** Working Blueprint / Subject to Revision\
**Date:** August 2026

------------------------------------------------------------------------

## 1. Purpose

This document establishes the current working blueprint for Member 6 (M6
--- AI Assistance).

It maps each confirmed AI requirement to:

-   Intended functionality
-   Potential AI/ML approach
-   Potential model/API
-   Primary implementation language
-   Likely project layer
-   Dependencies on other team members/modules
-   Expected outputs
-   Current design status

This is a **design blueprint, not a final implementation
specification**.

AI model choices, ML approaches, persistence, background-processing
mechanisms, and integration details may change after further analysis,
experimentation, Database v2, and implementation testing.

------------------------------------------------------------------------

## 2. Design Principles

### 2.1 Primary technology

The production AI module should be predominantly implemented using:

-   C#
-   ASP.NET Core
-   Existing Layered Modular Monolith architecture
-   Existing project module structure

Python and Jupyter may be introduced where they provide a genuine
benefit, such as:

-   ML experimentation
-   Dataset analysis
-   Model training
-   Model evaluation
-   Embedding/similarity experiments
-   Prototyping

Python/Jupyter should remain supporting technology unless a later design
decision demonstrates that a separate Python component is genuinely
required.

### 2.2 Human-in-the-loop

AI provides recommendations and assistance.

AI must not become the final decision-maker.

Human support personnel remain responsible for accepting, rejecting, or
overriding AI recommendations.

### 2.3 Module isolation

M6 should primarily work within:

``` text
AIITSM.Domain/06_M6_AI
AIITSM.Application/06_M6_AI
AIITSM.Infrastructure/06_M6_AI
AIITSM.Web/Controllers/06_M6_AI
```

M6 must not duplicate entities owned by other modules.

Cross-module communication should use clearly defined
contracts/interfaces/integration points.

Changes to another member's module must be discussed before
implementation.

### 2.4 Core incident management must remain independent of AI

An incident must be successfully persisted before AI processing begins.

AI analysis should run in the background.

If AI processing fails, the incident must remain available and usable.

------------------------------------------------------------------------

# 3. AI Requirement Mapping

  --------------------------------------------------------------------------------------------------------------------------------------------------------------------
  ID         Confirmed           Intended Functionality          Potential Approach          Potential          Primary Language  Main Dependencies
             Requirement                                                                     Model/API                            
  ---------- ------------------- ------------------------------- --------------------------- ------------------ ----------------- ------------------------------------
  AI-01      Analyze newly       Analyze title/description and   LLM-based structured        Gemini Flash-class C#                M2 Incident Management
             submitted incident  produce structured              analysis                    model                                
             descriptions        understanding of the incident                                                                    

  AI-02      Suggest category    Recommend an incident category  LLM classification; custom  Gemini             C#;               M2 Categories/Incident data
                                                                 ML may be evaluated later   Flash-Lite-class   Python/Jupyter    
                                                                                             model or custom    for ML            
                                                                                             classifier         experiments       

  AI-03      Suggest             Recommend priority/severity     LLM                         Gemini             C#;               M2 Incident data
             priority/severity                                   classification/reasoning;   Flash-Lite-class   Python/Jupyter    
                                                                 custom ML may be evaluated  model or custom    for ML            
                                                                 later                       classifier         experiments       

  AI-04      Suggest possible    Generate possible               LLM reasoning/generation    Gemini Flash-class C#                M2 Incident data; potentially M3
             resolutions         troubleshooting/resolution                                  model                                investigation/resolution context
                                 steps                                                                                            

  AI-05      Identify            Find potentially similar        Embeddings/vector           Gemini Embedding   C#;               M2 incident history; database/vector
             related/duplicate   historical incidents and assess similarity + LLM            model + Gemini     Python/Jupyter    storage decision
             incidents           relevance                       verification                Flash-class model  for               
                                                                                                                experimentation   

  AI-06      Generate summaries  Summarize incident              LLM summarization           Gemini Flash-class C#                M3
             of lengthy incident comments/conversation/history                               model                                IncidentComments/status/resolution
             conversations                                                                                                        information

  AI-07      AI assistant for    Conversational support for      LLM assistant;              Gemini Flash-class C#                M2/M3 knowledge/context; possible
             common IT support   common IT questions             knowledge/context strategy  model                                future knowledge source
             queries                                             to be designed                                                   

  AI-08      Authorized support  Allow an authorized human to    Normal C#                   No LLM required    C#                M1 authorization; M2 incident data
             personnel can       accept an AI recommendation     application/business logic                                       
             accept                                                                                                               
             recommendations                                                                                                      

  AI-09      Authorized support  Allow an authorized human to    Normal C#                   No LLM required    C#                M1 authorization; M2 incident data
             personnel can       override AI recommendations     application/business logic                                       
             override                                                                                                             
             recommendations                                                                                                      

  AI-10      AI cannot           Enforce human authority over AI Authorization/business      No LLM required    C#                M1 authorization; M2 incident
             automatically                                       rules and workflow                                               workflow
             override human                                      constraints                                                      
             decisions                                                                                                            
  --------------------------------------------------------------------------------------------------------------------------------------------------------------------

------------------------------------------------------------------------

# 4. Requirement-by-Requirement Notes

## AI-01 --- Incident Analysis

### Goal

Analyze a newly submitted incident and produce structured information
that can support the remaining AI capabilities.

### Candidate implementation

``` text
Incident
   |
   v
AI Analysis Service
   |
   v
LLM
   |
   v
Structured AI Result
```

The result should be structured rather than treated as unrestricted
text.

### Candidate technology

-   C# / ASP.NET Core for orchestration
-   Gemini Flash-class model as the current candidate
-   Structured output/schema validation to be investigated

### Dependencies

**M2 --- Incident Management**

M6 needs the final incident information exposed by M2.

Potential input:

-   IncidentId
-   Title
-   Description
-   Category information if available
-   Priority information if available
-   Status if relevant

The exact contract must follow the final M2 design and Database v2.

------------------------------------------------------------------------

## AI-02 --- Category Suggestion

### Goal

Suggest an appropriate category for a newly submitted incident.

### Candidate approaches

**Option A --- LLM classification**

Use an LLM with the allowed category list and incident information.

**Option B --- Custom ML classifier**

Use Python/Jupyter to experiment with a dataset and evaluate a
traditional or transformer-based classifier.

### Current direction

Start by evaluating the LLM approach.

Only introduce a custom ML model if a dataset exists and experimentation
demonstrates a meaningful benefit.

### Important constraint

The AI suggestion must not automatically become the final human-approved
category unless the authorized workflow explicitly accepts it.

------------------------------------------------------------------------

## AI-03 --- Priority/Severity Suggestion

### Goal

Suggest an appropriate priority/severity.

### Candidate approach

LLM-based classification/reasoning.

A lightweight model may be sufficient, but the final model choice should
be validated experimentally.

### Important consideration

Priority should be based on defined project criteria once those criteria
are finalized.

AI should not invent a priority scale that conflicts with the system's
final allowed values.

Database v1 currently leaves exact priority values under review.

------------------------------------------------------------------------

## AI-04 --- Resolution Suggestion

### Goal

Generate possible resolutions for the incident.

### Candidate approach

LLM reasoning/generation.

Potential future enhancement:

-   Provide incident history/context
-   Provide relevant support knowledge
-   Provide previous resolutions
-   Generate actionable troubleshooting steps

### Human control

Generated resolution suggestions are recommendations.

They must not automatically close or resolve an incident.

------------------------------------------------------------------------

## AI-05 --- Related / Duplicate Incident Detection

### Goal

Identify potentially related or duplicate incidents.

### Preferred candidate architecture

``` text
New Incident
     |
     v
Embedding
     |
     v
Vector Similarity Search
     |
     v
Top Candidate Incidents
     |
     v
LLM Verification
     |
     v
Related / Duplicate Assessment
```

This avoids asking an LLM to compare a new incident against every
incident in the database.

### Candidate technology

-   Embedding model/API
-   Vector similarity search
-   LLM for final semantic assessment
-   SQL Server/vector capability or another approved storage approach,
    subject to Database v2

### Python/Jupyter role

Python/Jupyter may be used to experiment with:

-   Embedding similarity thresholds
-   Similarity metrics
-   Candidate ranking
-   Evaluation datasets

Python does not automatically become part of the production
architecture.

------------------------------------------------------------------------

## AI-06 --- Conversation Summarization

### Goal

Generate a concise summary of lengthy incident conversations.

### Candidate input

Potentially:

-   Incident comments
-   Status history
-   Investigation information
-   Resolution information

The exact available data depends on M3 and Database v2.

### Candidate approach

LLM summarization.

### Dependency

**M3 --- Agent Workflow**

M6 should consume the agreed M3 contract rather than duplicate
comments/history entities.

------------------------------------------------------------------------

## AI-07 --- IT Support Assistant

### Goal

Provide an AI assistant for common IT support queries.

### Candidate approach

LLM-based conversational assistant.

### Questions still requiring design

-   What knowledge should the assistant use?
-   Should it answer only from approved project knowledge?
-   Should it have access to incident information?
-   Should it be available to all users or only certain roles?
-   Does it require retrieval/RAG?
-   Should conversations be persisted?
-   What security/privacy restrictions apply?

These questions must be resolved before implementing a production
assistant.

------------------------------------------------------------------------

# 5. AI-08 / AI-09 / AI-10 --- Human Decision Layer

These requirements do not require an LLM.

They belong primarily to application/business logic and authorization.

Conceptually:

``` text
AI Recommendation
       |
       v
Authorized Human
       |
   +---+---+
   |       |
 Accept  Override
   |       |
   +---+---+
       |
       v
Human Decision
```

AI must never bypass this workflow.

M6 must work with the final authorization model provided by M1.

------------------------------------------------------------------------

# 6. Potential Technology Stack

## Production

  Area                           Candidate
  ------------------------------ ---------------------------------------------
  Application                    C# / ASP.NET Core
  AI orchestration               C#
  AI abstraction                 Application layer
  External AI implementation     Infrastructure layer
  LLM                            Gemini API
  Embeddings                     Gemini embedding API, subject to validation
  Persistence                    Existing project database / Database v2
  Background processing          To be finalized
  Authentication/authorization   Existing project Identity design
  Automation                     n8n, but AI integration is not assumed
  Version control                Git + GitHub

## Experimental

  Area                     Candidate
  ------------------------ ------------------
  Data analysis            Python
  ML experimentation       Python + Jupyter
  Model training           Python
  Evaluation               Python/Jupyter
  Similarity experiments   Python/Jupyter
  Visualization            Python/Jupyter

------------------------------------------------------------------------

# 7. Cross-Module Dependencies

## M1 --- Identity & Access

M6 may depend on M1 for:

-   Authenticated user context
-   Role/authorization information
-   Authorization for accepting recommendations
-   Authorization for overriding recommendations
-   Authorization for using certain AI functions

M6 must not create a second authentication system.

------------------------------------------------------------------------

## M2 --- Incident Management

M6 depends heavily on M2.

Potential information:

``` text
IncidentId
Title
Description
Category
Priority
Status
```

M6 should consume M2-owned data/contracts.

M6 must not create a duplicate Incident entity.

------------------------------------------------------------------------

## M3 --- Agent Workflow

Potential information:

``` text
IncidentComments
IncidentStatusHistory
Investigation information
Resolution information
```

This is particularly relevant to AI-06.

The exact contract depends on M3's final design.

------------------------------------------------------------------------

## M5 --- Reporting

Potential future dependency:

``` text
AI-generated data
      |
      v
Reporting / Analytics
```

M6 should expose AI results through an agreed contract rather than
modifying M5's implementation.

------------------------------------------------------------------------

## M7 --- Automation

Potential future integration:

``` text
AI Event
   |
   v
Automation
   |
   v
n8n workflow
```

Examples might eventually include AI-related notifications.

However, the exact M6 ↔ M7/n8n integration is not currently finalized.

------------------------------------------------------------------------

# 8. Proposed M6 Internal Boundary

The initial conceptual structure is:

``` text
AIITSM.Domain
└── 06_M6_AI
    └── AI domain concepts / contracts

AIITSM.Application
└── 06_M6_AI
    └── AI orchestration / use cases / interfaces

AIITSM.Infrastructure
└── 06_M6_AI
    └── Gemini / embeddings / external AI implementations
    └── AI persistence implementation where appropriate

AIITSM.Web
└── Controllers
    └── 06_M6_AI
        └── AI endpoints / presentation integration
```

The exact classes and folders are intentionally not finalized yet.

------------------------------------------------------------------------

# 9. Background Processing Direction

Current agreed direction:

``` text
Employee
   |
   v
Submit Incident
   |
   v
Save Incident
   |
   +------------------> Return success
   |
   v
AI Processing Job
   |
   v
Background AI Analysis
   |
   v
Validate AI Result
   |
   v
Persist AI Analysis
```

If AI fails:

``` text
Incident
   |
   +---- remains valid and usable
   |
AI Analysis
   |
   +---- Failed
```

The exact background-processing technology is still a design decision.

Candidates include:

-   ASP.NET Core background processing
-   Persistent job mechanism
-   Background-job framework
-   n8n integration
-   Other appropriate mechanism

No technology is finalized by this document.

------------------------------------------------------------------------

# 10. Database Impact

Database v1 already contains an `AIAnalysis` concept associated with an
Incident.

Current concepts include:

``` text
AIAnalysisId
IncidentId
SuggestedCategory
SuggestedPriority
SuggestedResolution
RelatedIncidentId
ConfidenceScore
CreatedAt
```

However, Database v1 explicitly identifies AI persistence as incomplete.

Potential future decisions include:

-   AI processing status
-   AI failure information
-   Conversation summary
-   Accept/override decisions
-   AI recommendation history
-   Additional AI metadata

**Do not modify the database solely from this blueprint.**

Database changes should be made only after the relevant AI workflow and
Database v2 decisions have been agreed.

------------------------------------------------------------------------

# 11. Current Model Selection --- Provisional

The following is a working hypothesis only.

  -----------------------------------------------------------------------
  Capability                          Current Candidate
  ----------------------------------- -----------------------------------
  AI-01 Incident Analysis             Gemini Flash-class model

  AI-02 Category                      Gemini Flash-Lite-class model /
                                      custom ML evaluation

  AI-03 Priority                      Gemini Flash-Lite-class model /
                                      custom ML evaluation

  AI-04 Resolution                    Gemini Flash-class model

  AI-05 Related/Duplicate             Embeddings + similarity search +
                                      Gemini verification

  AI-06 Summarization                 Gemini Flash-class model

  AI-07 Support Assistant             Gemini Flash-class model

  AI-08 Accept                        C# logic

  AI-09 Override                      C# logic

  AI-10 Human authority               C# authorization/business rules
  -----------------------------------------------------------------------

Exact model IDs must be verified at implementation time because model
availability, lifecycle, pricing, and capabilities can change.

------------------------------------------------------------------------

# 12. What Is Not Yet Decided

The following remain open:

-   Exact Gemini model IDs
-   Exact AI prompts
-   Structured output schema
-   AI confidence calculation/interpretation
-   Background processing technology
-   AI job persistence
-   AI failure handling implementation
-   Database v2 AI entities/fields
-   Conversation-summary persistence
-   Accept/override persistence
-   Duplicate-detection embedding strategy
-   Vector storage technology
-   AI assistant architecture
-   Whether custom ML models are worthwhile
-   Whether Python is required in production
-   AI security/privacy constraints
-   AI API quota/cost strategy
-   Exact cross-module contracts

------------------------------------------------------------------------

# 13. Decision Rule for Future Changes

A technology should be added only when it solves a demonstrated
requirement or technical problem.

For every proposed addition, ask:

1.  What requirement does it satisfy?
2.  Why can't the current C#/ASP.NET Core architecture handle it
    adequately?
3.  Does it introduce a new dependency?
4.  Which module owns it?
5.  Does it affect another member's work?
6.  Can it be isolated behind an interface/contract?
7.  Does it require a database change?
8.  Can we test and maintain it as a student team?
9.  Does it add meaningful technical value?

------------------------------------------------------------------------

# 14. Current Status

**Status:** Blueprint established.

We have established:

-   AI-01 to AI-10 mapping
-   C# / ASP.NET Core as the primary production stack
-   Python/Jupyter as optional supporting technology
-   Candidate LLM/embedding/ML approaches
-   Human-in-the-loop boundary
-   M6 module boundary
-   Cross-module dependencies
-   Background-processing direction
-   Database impact areas
-   Provisional model strategy

This document is expected to evolve as the project progresses.

------------------------------------------------------------------------

# 15. Next Step

The next design step is to convert this blueprint into the **M6 AI
Architecture**.

That step should define:

1.  AI Application responsibilities
2.  AI Domain responsibilities
3.  AI Infrastructure responsibilities
4.  AI Web responsibilities
5.  Cross-module contracts
6.  AI processing workflow
7.  Background-job boundary
8.  Gemini abstraction
9.  AI result structure
10. Error/failure boundary

Only after those decisions should the first production AI classes be
created.
