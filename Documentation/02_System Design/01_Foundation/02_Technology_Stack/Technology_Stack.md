# Phase A — Step 2: Confirm Technology Stack

## 1. Purpose

This step establishes the technology baseline for the AI-Powered IT Service Desk & Incident Management System before architectural and detailed technical design decisions are made.

The technology baseline is derived from the current project technology direction, the Requirement Analysis, Database Design v1, and the faculty-provided automation technology direction.

Technology choices that have not yet been sufficiently determined are explicitly marked as proposed or unresolved and will be addressed in later System Design phases.

---

## 2. Technology Stack Summary

| Area | Technology | Classification |
|---|---|---|
| Programming Language | C# | Confirmed project technology direction |
| Application Framework | ASP.NET Core | Confirmed project technology direction |
| Web UI | ASP.NET Core MVC + Razor/CSHTML | Proposed design decision |
| Web API | ASP.NET Core Web API | Proposed design decision |
| ORM / Data Access | Entity Framework Core | Proposed design decision |
| Database | Microsoft SQL Server | Proposed/selected technical direction |
| Authentication | ASP.NET Core Identity | Proposed design decision; requires Database v1 reconciliation |
| Authorization | ASP.NET Core role-based authorization | Proposed design direction; detailed design in Step 22 |
| AI | Gemini API | Proposed technical decision |
| Automation | n8n | Faculty-provided technology direction |
| Background Processing | TBD | Unresolved |
| Version Control | Git/GitHub | Confirmed development direction |

---

# 3. Application Technology

## 3.1 Programming Language — C#

**Status:**  
Confirmed project technology direction

**Description:**

C# will be used as the primary programming language for the AI-Powered IT Service Desk & Incident Management System.

The project is being developed using the C# / ASP.NET Core technology direction.

---

## 3.2 Application Framework — ASP.NET Core

**Status:**  
Confirmed project technology direction

**Description:**

ASP.NET Core will be used as the primary application framework for the AI-Powered IT Service Desk & Incident Management System.

The Requirement Analysis identifies technology selection as part of the System Design stage. The use of ASP.NET Core is therefore treated as the established project technology direction rather than as a requirement directly mandated by the Requirement Analysis.

---

## 3.3 Web UI — ASP.NET Core MVC + Razor/CSHTML

**Decision:**  
ASP.NET Core MVC with Razor/CSHTML views

**Status:**  
Proposed design decision

**Description:**

The user-facing web application will use ASP.NET Core MVC with Razor/CSHTML views for server-rendered web pages.

This approach provides the web interface through `.cshtml` views while remaining within the C# and ASP.NET Core application ecosystem.

The exact organization of controllers, views, services, and modules will be determined during the later architecture and module-design steps.

---

## 3.4 Web API

**Decision:**  
ASP.NET Core Web API endpoints/controllers where appropriate

**Status:**  
Proposed design decision

**Description:**

The ASP.NET Core application may expose Web API endpoints where API-based communication is required by the system design.

The use of Web API does not replace the MVC/CSHTML user interface. MVC/Razor views will provide the primary web interface, while API endpoints may be used where structured application communication is appropriate.

The exact API boundaries, controllers, endpoints, request/response structures, and usage will be defined later during:

**Phase E — Step 23: Application/API Design**

---

# 4. Data Layer

## 4.1 ORM / Data Access — Entity Framework Core

**Decision:**  
Entity Framework Core

**Status:**  
Proposed design decision

**Description:**

Entity Framework Core will be used as the proposed ORM/data-access technology between the ASP.NET Core application and SQL Server.

EF Core will provide application-level interaction with the relational database while allowing the database structure to be represented through application entities and relationships.

The final mapping will be validated against Database Design v2 before development is finalized.

---

## 4.2 Database — Microsoft SQL Server

**Decision:**  
Microsoft SQL Server

**Status:**  
Proposed/selected technical direction

**Description:**

Microsoft SQL Server will be used as the relational database platform for the system.

Database Design v1 is already structured using SQL Server-compatible relational constructs, including identity columns, `DATETIME`, `VARCHAR`, default values, and foreign-key relationships.

Therefore, continuing with SQL Server maintains compatibility with the existing Database Design v1 rather than introducing an unnecessary database-platform change.

> **Note:** Database Design v1 is not modified during this step. Database changes and pending database decisions will be addressed during System Design validation and Database v2.

---

# 5. Authentication and Authorization

## 5.1 Authentication — ASP.NET Core Identity

**Decision:**  
ASP.NET Core Identity

**Status:**  
Proposed design decision — requires later validation

**Description:**

ASP.NET Core Identity is proposed for authentication and identity management.

Database Design v1 currently contains its own `Users` and `Roles` structures, including a `PasswordHash` field in the `Users` table.

Therefore, the use of ASP.NET Core Identity must be reconciled with the existing Database Design v1 rather than introducing two independent identity mechanisms.

The mapping between ASP.NET Core Identity and the existing `Users`/`Roles` design will be addressed during:

- **Step 21 — Authentication**
- **Step 22 — Authorization**
- **Step 31 — System Design-to-Database comparison**
- **Step 32 — Resolve Database v1 pending items**
- **Step 33 — Database v2/finalization**

No Database v1 changes are made at this stage.

---

## 5.2 Authorization

**Status:**  
Proposed design direction

The system requires role-based access and restricted administrative functionality.

Role-based authorization is therefore the proposed authorization direction.

The detailed authorization model, role permissions, access rules, and implementation will be defined during:

**Phase E — Step 22: Authorization**

The four human actors identified in the Requirement Analysis are:

- Employee
- Help Desk Agent
- IT Administrator
- IT Manager

The exact mapping between roles, permissions, modules, and actions will be determined later and will not be finalized during this step.

---

# 6. AI Integration

**Technology:**  
Gemini API

**Status:**  
Proposed technical decision

**Description:**

Gemini API is proposed as the external AI service for implementing the AI-assisted capabilities defined in AI-01 through AI-10.

The Requirement Analysis defines the required AI capabilities but does not prescribe a specific AI provider.

The proposed use of Gemini API is therefore a technical implementation decision.

The detailed AI architecture and integration approach will be defined during:

- **Phase C — Step 12: AI Architecture**
- **Step 13: AI Incident Analysis**
- **Step 14: Human Review / Accept / Override**
- **Step 15: Related/Duplicate Incident Analysis**
- **Step 16: Conversation Summarization**

The current step does not finalize prompts, model configuration, AI workflows, or AI data-handling mechanisms.

---

# 7. Automation

**Technology:**  
n8n

**Status:**  
Faculty-provided technology direction

**Description:**

n8n has been specified by the faculty as the automation technology for the project.

The Requirement Analysis defines the automation requirements AR-01 through AR-06, including notifications, assignment notifications, status-change notifications, automated escalation, critical-incident notifications, and recording of important automated actions.

The exact mapping of these requirements to n8n workflows has not yet been finalized.

The following will therefore be designed during **Phase D — Automation**:

- n8n workflow responsibilities
- ASP.NET Core ↔ n8n communication
- workflow triggers
- notification workflows
- escalation workflows
- background processing
- automated action logging
- automation failure handling

n8n is therefore recorded as the technology direction, while the detailed automation architecture remains unresolved.

---

# 8. Background Processing

**Status:**  
Unresolved

The Requirement Analysis establishes requirements that may require processing outside the immediate user request, particularly AI processing and automated workflows.

NFR-04 requires AI processing to provide feedback without unnecessarily blocking the user's workflow, while NFR-11 requires failure of an individual automation workflow not to prevent core incident-management functionality.

However, the current documents do not specify the exact background-processing mechanism.

> **This is not specified in the current documents and requires a design decision.**

The mechanism will be evaluated during:

**Phase D — Step 19: Background Processing**

---

# 9. Version Control

**Technology:**  
Git + GitHub

**Status:**  
Confirmed project development direction

**Description:**

Git and GitHub will be used for source-code version control and collaborative development.

The existing project repository will be used to maintain application source code, documentation, database-design artifacts, diagrams, and other project materials according to the team's repository structure.

---

# 10. Technology-to-Requirement Alignment

| Requirement Area | Technology / Design Support |
|---|---|
| FR-01–FR-28 | ASP.NET Core MVC/web application |
| AI-01–AI-10 | Gemini API — proposed |
| AR-01–AR-06 | n8n — faculty-provided technology direction |
| NFR-05 | ASP.NET Core Identity — proposed |
| NFR-06 | Role-based authorization — proposed |
| NFR-07 | Authorization design — detailed in Step 22 |
| NFR-09 | Auditability — detailed design required later |
| NFR-11 | n8n/background-processing architecture — detailed in Phase D |
| NFR-12–NFR-14 | ASP.NET Core + SQL Server; architecture to be defined in Steps 3–5 |
| NFR-18–NFR-20 | Maintainable architecture and modular separation — Steps 3–5 |

Technology selection alone does not satisfy a requirement. The corresponding architecture, workflows, implementation, and validation will be developed in later System Design phases.

---

# 11. Database v1 Compatibility

| Technology | Database v1 Relationship |
|---|---|
| SQL Server | Directly compatible |
| Entity Framework Core | Can map to the relational schema |
| ASP.NET Core Identity | ⚠️ Requires reconciliation with `Users`/`Roles` |
| Gemini API | `AIAnalysis` structure requires validation against the AI workflow |
| n8n | `Notifications`, `Escalations`, and automated-action logging require validation |
| MVC/CSHTML | No direct database change |
| Web API | No direct database change by itself |

The database is not modified during Step 2.

Any technology decision that affects the database will be carried forward to Database v2 validation.

---

# 12. Open Technology Decisions

| Item | Status |
|---|---|
| Exact MVC/API boundaries | Unresolved — Step 23 |
| Identity ↔ Users/Roles mapping | Unresolved — Step 21 + Database v2 |
| Authorization implementation | Unresolved — Step 22 |
| n8n ↔ ASP.NET Core communication | Unresolved — Phase D |
| n8n workflow ownership | Unresolved — Phase D |
| Background-processing mechanism | Unresolved — Step 19 |
| Gemini integration details | Unresolved — Phase C |
| AI failure/fallback behaviour | Unresolved — Phase C |
| Deployment environment | Unresolved — Step 29 |

---

# 13. Technology Stack Baseline

The current technology baseline is:

| Area | Technology | Classification |
|---|---|---|
| Programming Language | **C#** | Confirmed project direction |
| Application Framework | **ASP.NET Core** | Confirmed project direction |
| Web UI | **ASP.NET Core MVC + Razor/CSHTML** | Proposed design decision |
| API | **ASP.NET Core Web API** | Proposed design decision; exact use TBD |
| ORM / Data Access | **Entity Framework Core** | Proposed design decision |
| Database | **Microsoft SQL Server** | Proposed/selected technical direction |
| Authentication | **ASP.NET Core Identity** | Proposed; requires Database v1 reconciliation |
| Authorization | **Role-based authorization** | Proposed; detailed design in Step 22 |
| AI | **Gemini API** | Proposed technical decision |
| Automation | **n8n** | Faculty-provided technology direction |
| Background Processing | **TBD** | Unresolved |
| Version Control | **Git/GitHub** | Confirmed development direction |

---

# 14. Proposed Visual Studio Application Direction

At the technology-stack level, the proposed application direction is:


AI-ITSM Solution
│
└── ASP.NET Core Web Application
    │
    ├── MVC / Razor Views
    │      └── .cshtml
    │
    ├── Web API
    │      └── API endpoints where required
    │
    ├── Application Logic
    │
    ├── Entity Framework Core
    │
    └── SQL Server

**External Integration:**:
ASP.NET Core
     │
     ├──────────► Gemini API
     │
     └──────────► n8n

This represents the current technology direction only. It is not the final system architecture or module structure.

The architecture, layers, modules, dependencies, and integration boundaries will be defined in:

-Step 3 — Select Architecture
-Step 4 — Define Layers
-Step 5 — Define Modules

## 15. Step 2 Decision
SD-005 — Technology Stack Baseline

Status:
Confirmed baseline with proposed and unresolved elements

Decision:

The project will use C# and ASP.NET Core as its primary development direction. ASP.NET Core MVC with Razor/CSHTML is the proposed web UI approach, with ASP.NET Core Web API available where API-based communication is required. Entity Framework Core and SQL Server are proposed for application data access and relational persistence. ASP.NET Core Identity is proposed for authentication but requires reconciliation with Database Design v1. Gemini API is proposed for the AI capabilities. n8n is the faculty-provided technology direction for automation.

The exact API boundaries, background-processing mechanism, authentication/authorization implementation, AI integration architecture, n8n integration architecture, and deployment approach remain subject to later System Design decisions.

Database impact:

The technology stack does not directly modify Database Design v1 at this stage. However, ASP.NET Core Identity, EF Core, Gemini integration, and n8n automation have identified database implications that will be validated during the later System Design-to-Database comparison and Database v2 process.