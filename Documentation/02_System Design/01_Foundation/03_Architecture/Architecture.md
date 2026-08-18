# Phase A — Step 3: Select Architecture

## 1. Purpose

This step defines the overall architectural style of the AI-Powered IT Service Desk & Incident Management System.

The purpose is to determine how the application will be organized and how the main application will interact with the database and external services.

The architecture must remain suitable for a seven-member student development team while providing sufficient separation of responsibilities, maintainability, security, and support for the required AI and automation capabilities.

The architecture selected in this step will provide the foundation for:

- Step 4 — Define Layers
- Step 5 — Define Modules
- Phase B — System Behaviour
- Phase C — AI Design
- Phase D — Automation
- Phase E — Technical Design

---

# 2. Architecture Requirements and Constraints

The architecture must support the requirements and project constraints identified during Requirement Analysis and the technology-stack decision.

Important considerations include:

- The system is a web-based IT service desk and incident management application.
- The application will be developed using C# and ASP.NET Core.
- The system requires role-based access.
- The system contains incident management, assignment, communication, AI assistance, notification, escalation, reporting, and administration capabilities.
- AI capabilities are required for incident analysis and related functionality.
- Automated workflows are required for notifications, escalation, and other automation requirements.
- The faculty has specified n8n as the automation technology.
- The system should remain maintainable and support future AI, automation, and integration requirements.
- Failure of an individual automation workflow should not prevent core incident-management functionality.
- The project is being developed by a seven-member student team.
- The architecture should avoid unnecessary enterprise-level complexity.

The Requirement Analysis specifically identifies maintainability, modular separation, future AI/automation capability, and integration support as non-functional considerations. These requirements are relevant to the architectural decision.

---

# 3. Architecture Options Considered

The following architectural approaches were considered at a high level:

## 3.1 Simple Monolithic Application

A single application containing all functionality without strong internal modular separation.

### Advantages

- Simple to understand.
- Simple to develop and run locally.
- Simple deployment model.

### Limitations

- Can become difficult to maintain as functionality increases.
- Provides weaker separation between major functional areas.
- Makes future changes to individual functional areas less structured.

This approach is therefore not selected as the primary architectural approach.

---

## 3.2 Layered Modular Monolith

A single ASP.NET Core application organized into clearly separated layers and internal functional modules.

The application remains one main deployable application while maintaining separation between presentation, application logic, domain logic, and infrastructure concerns.

### Advantages

- Suitable for a seven-member development team.
- Easier local development and debugging than a distributed architecture.
- Provides separation of responsibilities.
- Supports modular development without introducing unnecessary distributed-system complexity.
- Works naturally with ASP.NET Core and Entity Framework Core.
- Allows the team to evolve modules without immediately requiring separate services.
- Reduces deployment and service-to-service communication complexity.

### Limitations

- The main ASP.NET Core application remains a shared deployment boundary.
- A failure in the main application can affect multiple internal modules.
- Stronger isolation between modules would require additional architectural mechanisms.

---

## 3.3 Microservices / Distributed Architecture

The system could be divided into multiple independently deployed services such as incident management, AI, notifications, reporting, and other functional services.

### Advantages

- Independent deployment of services.
- Stronger service-level isolation.
- Individual services can potentially scale independently.

### Limitations

- Introduces additional deployment and configuration complexity.
- Requires service-to-service communication.
- Requires additional handling for distributed failures.
- Makes local development and debugging more complicated.
- Adds infrastructure and operational overhead.
- Is unnecessary unless a documented requirement justifies the additional complexity.

The current requirements do not justify adopting a microservices architecture.

---

# 4. Selected Architecture

## 4.1 Decision

**Layered Modular Monolith**

## 4.2 Status

**Selected design decision**

## 4.3 Description

The AI-Powered IT Service Desk & Incident Management System will use a **Layered Modular Monolith** architecture.

The main application will be implemented as one ASP.NET Core application with clearly separated internal layers and functional modules.

The architecture will provide separation of responsibilities while avoiding the unnecessary complexity of independently deployed microservices.

The detailed layer definitions and module boundaries will be established in:

- **Step 4 — Define Layers**
- **Step 5 — Define Modules**

---

# 5. Main Application Boundary

The main application will consist of one ASP.NET Core application.

Conceptually:

AI-ITSM ASP.NET Core Application
│
├── Web UI
│   └── MVC / Razor / CSHTML
│
├── Web API
│   └── API endpoints where appropriate
│
├── Application Logic
│
├── Domain Logic
│
└── Infrastructure