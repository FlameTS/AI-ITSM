# AI-ITSM — AI-Powered IT Service Management & Incident Resolution Platform

An intelligent IT service management platform that uses AI to classify, prioritize, analyze, and assist in resolving IT incidents — while automating support workflows and escalations.

🔗 **Live Demo:** [https://ai-itsm.onrender.com](https://ai-itsm.onrender.com/)
📦 **Releases:** [GitHub Releases](https://github.com/FlameTS/AI-ITSM/releases)
💻 **Repository:** [github.com/FlameTS/AI-ITSM](https://github.com/FlameTS/AI-ITSM)

> ⚠️ Hosted on Render's free tier — the app may take 30–60 seconds to wake up on first load.
Note:
1. Main Contains the local version.
2. feature/render-deploy contains the version deployed on render
3. all others are intermediary files done by teammates.

---

## ✨ Features

- **Incident Management** — create, track, assign, and resolve IT incidents and service requests end-to-end
- **AI Assistance (Gemini-powered)** — analyzes incident descriptions to suggest category, priority, and possible resolutions; includes an AI chat assistant for common IT queries
- **Automated Workflows (n8n)** — assignment, status-change, critical-incident, and escalation notifications triggered automatically
- **Role-Based Access** — separate experiences for Employees, Help Desk Agents, IT Administrators, and IT Managers
- **Reporting & Monitoring** — incident statistics, unresolved/escalated incident tracking, and support-team performance

## 👥 Roles & Responsibilities

| Role | Responsibilities |
|---|---|
| Employee | Create incidents/service requests, track status, communicate with support, give feedback |
| Help Desk Agent | View/manage assigned incidents, update status, assign/reassign, investigate, resolve & close |
| IT Administrator | Manage users, roles/permissions, categories, and system configuration |
| IT Manager | View statistics, monitor unresolved/escalated incidents, monitor performance, reporting |

## 🔐 Demo Login

Use any of the accounts below on the [live demo](https://ai-itsm.onrender.com/) to explore the platform. Sign in as **Admin** first if you want to see the full app, including user management and configuration.

| Role | Email | Password |
|---|---|---|
| IT Administrator | `admin@aitsm.com` | `Admin@123` |
| IT Administrator | `admin2@aitsm.com` | `Admin2@123` |
| Employee | `tarman@itsm.com` | `Tarman@123` |
| Employee | `ananya@itsm.com` | `Ananya@123` |
| Help Desk Agent | `hello@itsm.com` | `Hello@123` |
| IT Manager | `world@itsm.com` | `World@123` |

## 🛠️ Tech Stack

- **Backend:** ASP.NET Core (.NET 10), Entity Framework Core, ASP.NET Core Identity
- **Database:** PostgreSQL (hosted on Supabase)
- **AI:** Google Gemini API
- **Automation:** n8n (webhook-driven workflows for notifications & escalations)
- **Hosting:** Render (Docker)
- **Version Control:** Git / GitHub

## 🚀 Getting Started

```bash
git clone https://github.com/FlameTS/AI-ITSM.git
cd AI-ITSM/Source/AIITSM
cp .env.example .env   # fill in your Supabase connection strings & Gemini API key
dotnet run
```

## 📄 License

See repository for license details.
