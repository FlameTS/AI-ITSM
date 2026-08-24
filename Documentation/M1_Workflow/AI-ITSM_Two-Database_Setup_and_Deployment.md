# AI-ITSM Database Setup & Deployment Notes

## 1. Current Database Architecture

The AI-ITSM project currently uses **two separate SQL Server databases**.

```text
                         AI-ITSM
                            |
              +-------------+-------------+
              |                           |
             M1                       M2 / M3 / M6
              |                           |
    ApplicationDbContext            AIITSMDbContext
              |                           |
     AITSM_IdentityDb                  ITServiceDesk
              |                           |
      ASP.NET Identity          Incidents / Agent Workflow /
      Users / Roles / etc.             AI data
```

### M1 — Identity Database

- Context: `ApplicationDbContext`
- Connection string: `DefaultConnection`
- Local database name: `AITSM_IdentityDb`
- Purpose: ASP.NET Core Identity
- Migration:
  `20260819071438_InitialIdentity`

The M1 migration creates the ASP.NET Identity tables, including:

- `AspNetUsers`
- `AspNetRoles`
- `AspNetUserClaims`
- `AspNetUserLogins`
- `AspNetUserRoles`
- `AspNetUserTokens`
- `AspNetRoleClaims`

### M2 / M3 / M6 — Main Application Database

- Context: `AIITSMDbContext`
- Connection string: `AIITSMDatabase`
- Local database name: `ITServiceDesk`
- Purpose: Incident Management, Agent Workflow, AI and related application data.
- Existing `Database.sql` remains separate from the M1 Identity migration.

---

## 2. Why M1 Did Not Work Initially

The merged `Program.cs` registers the M1 database:

```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));
```

and ASP.NET Identity:

```csharp
.AddEntityFrameworkStores<ApplicationDbContext>()
```

The application also runs:

```csharp
await IdentitySeeder.SeedRolesAndAdminAsync(
    roleManager,
    userManager,
    builder.Configuration);
```

However, normal application startup does **not currently call**:

```csharp
Database.MigrateAsync()
```

Therefore, when the application first started:

```text
Application starts
    |
    v
IdentitySeeder
    |
    v
ApplicationDbContext tries to access AITSM_IdentityDb
    |
    v
AITSM_IdentityDb does not exist
    |
    v
SQL Server error 4060
    |
    v
Application stops
```

Visual Studio then displayed:

> Unable to connect to web server 'https'. The web server is no longer running.

The HTTPS message was a consequence of the application crashing during startup, not the root database problem.

---

## 3. How the M1 Database Was Created Locally

The database was created by explicitly applying the EF Core migration.

Command used:

```cmd
dotnet ef database update --project Source\AIITSM\AIITSM.Infrastructure --startup-project Source\AIITSM\AIITSM.Web --context ApplicationDbContext
```

EF Core automatically performed:

```text
CREATE DATABASE [AITSM_IdentityDb]
        |
        v
Create __EFMigrationsHistory
        |
        v
Apply 20260819071438_InitialIdentity
        |
        v
Create ASP.NET Identity tables
```

The migration was successfully recorded in:

```text
__EFMigrationsHistory
```

with:

```text
20260819071438_InitialIdentity
```

After this, the LocalDB instance contained:

```text
(localdb)\MSSQLLocalDB
|
+-- ITServiceDesk
|
+-- AITSM_IdentityDb
|
+-- other local databases
```

---

## 4. Important Rule: Do Not Change Database.sql for M1

For the current architecture, **do not add the M1 Identity tables to `Database.sql`**.

The databases are intentionally separate:

```text
Database.sql
    |
    +--> ITServiceDesk
          |
          +--> M2
          +--> M3
          +--> M6

EF Core Identity migration
    |
    +--> AITSM_IdentityDb
          |
          +--> M1
```

This keeps M1's Identity database independent from the main application database.

---

## 5. Development Workflow

When setting up a new development machine:

### Step 1 — Configure connection strings

Example local configuration:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=AITSM_IdentityDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True",
    "AIITSMDatabase": "Server=localhost;Database=ITServiceDesk;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Do not commit real secrets/API keys.

### Step 2 — Create/update the M1 database

Run:

```cmd
dotnet ef database update --project Source\AIITSM\AIITSM.Infrastructure --startup-project Source\AIITSM\AIITSM.Web --context ApplicationDbContext
```

This can create `AITSM_IdentityDb` if it does not exist and apply pending M1 migrations.

### Step 3 — Create/update the main application database

Use the project's existing `Database.sql` / approved M2-M3-M6 database setup for `ITServiceDesk`.

Do not mix this with the M1 Identity migration.

### Step 4 — Start the application

Only after the required databases/schema are initialized:

```text
Start application
      |
      v
ASP.NET Identity connects to AITSM_IdentityDb
      |
      v
IdentitySeeder runs
      |
      v
Application starts normally
```

---

## 6. Deployment Concept

The current application does **not** automatically initialize databases merely because `Program.cs` registers the DbContexts.

Therefore, deployment should have a database initialization/migration step before starting the application.

Conceptually:

```text
DEPLOYMENT

1. Deploy application
        |
        v
2. Configure production connection strings
        |
        +------------------------------+
        |                              |
        v                              v
3. Initialize M1                  4. Initialize M2/M3/M6
   ApplicationDbContext              AIITSMDbContext
        |                              |
        v                              v
   Identity database                Main ITSM database
        |                              |
        v                              v
   Apply EF migrations              Apply approved
                                     schema/migrations
        +--------------+---------------+
                       |
                       v
5. Start Web Application
                       |
                       v
6. IdentitySeeder runs
```

### Important

For deployment, the production connection strings will point to the production SQL Server databases rather than LocalDB.

For example:

```text
DefaultConnection
    -> Production Identity Database

AIITSMDatabase
    -> Production ITServiceDesk Database
```

The database names/server names shown above are examples of the intended separation, not hard-coded deployment requirements.

---

## 7. Migration vs. Application Startup

### Current approach

Database initialization is performed separately:

```text
EF migration/deployment step
        |
        v
Database ready
        |
        v
Application starts
```

This is different from:

```text
Application starts
        |
        v
Application automatically runs Database.MigrateAsync()
```

The project currently uses the first approach.

Do not add automatic startup migration without an explicit project decision.

---

## 8. Troubleshooting

### Error

```text
Cannot open database "AITSM_IdentityDb"
```

Check whether the database exists:

```cmd
sqlcmd -S "(localdb)\MSSQLLocalDB" -E -Q "SELECT name FROM sys.databases"
```

If `AITSM_IdentityDb` is missing, apply the M1 migration:

```cmd
dotnet ef database update --project Source\AIITSM\AIITSM.Infrastructure --startup-project Source\AIITSM\AIITSM.Web --context ApplicationDbContext
```

### Check M1 migrations

```cmd
dotnet ef migrations list --project Source\AIITSM\AIITSM.Infrastructure --startup-project Source\AIITSM\AIITSM.Web --context ApplicationDbContext
```

### Check database migration history

```sql
SELECT * FROM __EFMigrationsHistory;
```

Expected initial migration:

```text
20260819071438_InitialIdentity
```

---

## 9. Current Status

### Confirmed

- M1 uses a separate `ApplicationDbContext`.
- M2/M3/M6 use `AIITSMDbContext`.
- M1 uses `DefaultConnection`.
- M2/M3/M6 use `AIITSMDatabase`.
- `AITSM_IdentityDb` was not initially present on the local machine.
- The existing M1 EF migration successfully created `AITSM_IdentityDb`.
- `ITServiceDesk` remains a separate database.
- `Database.sql` has not been changed to include M1 Identity tables.
- The M1 migration successfully created the Identity schema.

### Current architectural decision

Keep the two databases separate:

```text
M1
  -> AITSM_IdentityDb

M2 + M3 + M6
  -> ITServiceDesk
```

Database initialization/migration should be treated as a deployment/setup step rather than assuming the normal application startup creates the databases.

---

## 10. Key Lesson

**Registering a DbContext does not automatically create its database.**

This:

```csharp
builder.Services.AddDbContext<ApplicationDbContext>(...);
```

only tells ASP.NET Core/EF Core:

> "When `ApplicationDbContext` is requested, use this configuration."

The database was actually created when EF Core was instructed to apply the migration:

```cmd
dotnet ef database update ...
```

Therefore, remember:

```text
Connection string
      !=
Database creation

DbContext registration
      !=
Database creation

EF migration/update
      =
Database/schema initialization
```
