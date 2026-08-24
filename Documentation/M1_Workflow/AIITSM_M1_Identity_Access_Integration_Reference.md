# AI-ITSM --- M1 Identity & Access Module: Integration Reference

**Module:** M1 --- Identity & Access\
**Status:** Implemented, tested, merged into `main`\
**Framework:** ASP.NET Core Identity / Entity Framework Core / ASP.NET
Core MVC\
**Purpose:** Reference for team members integrating other AI-ITSM
modules with authentication, authorization, users, and roles.

------------------------------------------------------------------------

## 1. Work Completed

The Identity & Access module has been implemented as the central
authentication and user-management layer for AI-ITSM.

Implemented functionality includes:

-   ASP.NET Core Identity integration.
-   SQL Server/LocalDB-backed Identity database.
-   Custom `ApplicationUser` and `ApplicationRole`.
-   User login and logout.
-   Authentication cookie configuration.
-   Role-Based Access Control (RBAC).
-   Four system roles:
    -   `Employee`
    -   `HelpDeskAgent`
    -   `ITAdministrator`
    -   `ITManager`
-   Initial role seeding.
-   Bootstrap administrator creation.
-   Administrator user-management functionality.
-   User creation.
-   User editing.
-   Role assignment and role changes.
-   User activation/deactivation.
-   Inactive-user handling.
-   Duplicate-email protection.
-   Administrator password reset.
-   Custom Access Denied handling.
-   Administrator dashboard.
-   Role-aware navigation.
-   Login UI.
-   Current-user service for integration with other modules.
-   Identity automated tests.
-   Merge/integration with the current M2/M3/M6 work on `main`.

------------------------------------------------------------------------

## 2. Identity Database

The Identity module currently uses its own EF Core context:

`ApplicationDbContext`

The development Identity database is configured through:

`DefaultConnection`

Example local configuration:

``` json
"DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=AITSM_IdentityDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
```

Identity migrations have been created and applied successfully. The
initial Identity migration was applied to the local development
database.

> Other application modules currently also use the main
> `AIITSMDbContext`/`AIITSMDatabase`. Database v2 still needs to
> finalize how Identity users are referenced across all module tables.

------------------------------------------------------------------------

## 3. Identity Entities

### ApplicationUser

The application uses a custom Identity user derived from ASP.NET Core
Identity.

Important fields currently used include:

-   `Id`
-   `UserName`
-   `Email`
-   `FullName`
-   `IsActive`
-   `EmailConfirmed`

The Identity `Id` is currently a **string**.

### ApplicationRole

Custom roles use `ApplicationRole`.

The standard roles seeded by the application are:

``` text
Employee
HelpDeskAgent
ITAdministrator
ITManager
```

Team members should use these exact role names when applying
authorization.

------------------------------------------------------------------------

## 4. Authentication

Authentication is configured in `AIITSM.Web/Program.cs`.

The application registers:

``` csharp
AddIdentity<ApplicationUser, ApplicationRole>()
```

with:

``` csharp
options.User.RequireUniqueEmail = true;
```

Cookie paths are configured as:

``` text
Login:        /Account/Login
AccessDenied: /Account/AccessDenied
```

The middleware order is:

``` csharp
app.UseAuthentication();
app.UseAuthorization();
```

Authentication must remain before authorization.

------------------------------------------------------------------------

## 5. Account Functionality

`AccountController` currently supports:

### Login

Users authenticate using their email and password.

Successful login redirects to the application home/dashboard flow.

### Logout

Authenticated users can log out using the logout POST action.

### Access Denied

Unauthorized users are redirected to:

``` text
/Account/AccessDenied
```

------------------------------------------------------------------------

## 6. Role-Based Access Control

ASP.NET Core role authorization is available throughout the Web project.

Example:

``` csharp
[Authorize(Roles = "ITAdministrator")]
public class UserManagementController : Controller
{
}
```

Multiple roles can also be allowed where required:

``` csharp
[Authorize(Roles = "ITAdministrator,ITManager")]
```

Use the standard role names defined in this document instead of creating
alternate names.

------------------------------------------------------------------------

## 7. Administrator User Management

The administrator workflow supports management of application users.

Implemented operations include:

-   View users.
-   Create users.
-   Edit users.
-   Assign/change roles.
-   Activate/deactivate accounts.
-   Reset user passwords.
-   Prevent duplicate email addresses.

The user-management interface also displays user information such as:

-   Full name.
-   Email.
-   Role.
-   Active/inactive status.

Administrative functionality should remain protected using the
`ITAdministrator` role.

------------------------------------------------------------------------

## 8. Duplicate Email Protection

Unique emails are enforced at the ASP.NET Core Identity configuration
level:

``` csharp
options.User.RequireUniqueEmail = true;
```

The application/controller layer can still perform its own duplicate
check to provide a clearer validation message.

Other modules should **not create Identity users directly in the
database**. User creation should go through Identity/UserManager so
Identity validation rules remain effective.

------------------------------------------------------------------------

## 9. Active/Inactive Users

`ApplicationUser` contains:

``` csharp
IsActive
```

This is used to represent whether an account is enabled.

Administration functionality allows an account to be activated or
deactivated.

Other modules should avoid inventing separate user-status systems when
the requirement is simply whether the AI-ITSM user account is active.

------------------------------------------------------------------------

## 10. Identity Seeder

`IdentitySeeder` is responsible for:

1.  Creating the four standard roles when they do not exist.
2.  Creating the initial administrator when configured and absent.
3.  Assigning the bootstrap administrator to `ITAdministrator`.

The administrator configuration is read from configuration rather than
being hard-coded directly in `IdentitySeeder.cs`.

Development configuration uses the `BootstrapAdmin` section.

Example structure:

``` json
"BootstrapAdmin": {
  "Email": "admin@aitsm.com",
  "Password": "<development-secret>",
  "FullName": "System Administrator"
}
```

Do not commit real production secrets.

For deployment, use an appropriate secret/environment configuration
mechanism.

------------------------------------------------------------------------

## 11. Current User Integration Service

To prevent every module from directly depending on `HttpContext`, M1
exposes:

``` text
ICurrentUserService
```

Location:

``` text
AITSM.Application
└── 01_M1_IdentityAccess
    └── Interfaces
        └── ICurrentUserService.cs
```

The contract exposes:

``` csharp
string? UserId { get; }
string? Email { get; }
bool IsAuthenticated { get; }
bool IsInRole(string role);
```

The Web implementation is:

``` text
AIITSM.Web
└── 01_M1_IdentityAccess
    └── Services
        └── CurrentUserService.cs
```

It uses `IHttpContextAccessor` to obtain the current authenticated
user's claims.

------------------------------------------------------------------------

## 12. How Other Modules Should Use the Current User

Inject the M1 interface into a controller/service that needs the
authenticated user.

Example:

``` csharp
private readonly ICurrentUserService _currentUserService;

public ExampleController(
    ICurrentUserService currentUserService)
{
    _currentUserService = currentUserService;
}
```

Then use:

``` csharp
_currentUserService.UserId
_currentUserService.Email
_currentUserService.IsAuthenticated
_currentUserService.IsInRole("HelpDeskAgent")
```

Typical uses include:

-   Identifying who created an incident.
-   Identifying the logged-in Help Desk Agent.
-   Recording who performed an administrative action.
-   Audit logging.
-   Restricting operations according to roles.
-   Associating records with authenticated users.

------------------------------------------------------------------------

## 13. Important Cross-Module Integration Issue

There is currently an important user-ID mismatch that Database v2 must
resolve.

### M1 Identity

ASP.NET Core Identity currently uses:

``` text
ApplicationUser.Id = string
```

### Incident Management / existing shared current-user flow

Parts of the existing Incident Management integration currently expect:

``` text
UserId = int
```

Because these types do not match, M2 currently retains its
temporary/demo current-user implementation rather than being directly
replaced by the M1 service.

**Do not independently convert IDs or create ad-hoc mappings inside
individual modules.**

This should be resolved centrally during Database v2 / shared-domain
integration so all modules use a consistent user-reference strategy.

------------------------------------------------------------------------

## 14. Program.cs Integration

The merged Web startup configuration now contains registrations for
multiple modules.

M1 contributes:

-   `ApplicationDbContext`
-   ASP.NET Core Identity
-   Identity cookie configuration
-   `IHttpContextAccessor`
-   M1 `ICurrentUserService`
-   Identity role/admin seeding
-   Authentication middleware

The merged application also contains registrations from other modules
such as Incident Management, Agent Workflow and AI.

When editing `Program.cs`, **do not replace the entire file with a
module-specific version**. Preserve registrations belonging to all
integrated modules.

------------------------------------------------------------------------

## 15. AI/Gemini Configuration Note

The integrated project contains the AI module and may print:

``` text
GEMINI_API_KEY not loaded
```

when the application starts without a local Gemini key.

The AI integration reads:

``` text
GEMINI_API_KEY
```

from environment/`.env` configuration.

`.env` is ignored by Git and API keys must not be committed to the
repository.

This message is related to the AI module, not a failure of M1 Identity
authentication.

------------------------------------------------------------------------

## 16. NuGet / Project Dependencies Added for Integration

Identity/EF Core dependencies include packages such as:

``` text
Microsoft.AspNetCore.Identity.EntityFrameworkCore
Microsoft.EntityFrameworkCore
Microsoft.EntityFrameworkCore.SqlServer
Microsoft.EntityFrameworkCore.Tools
```

During integration with the AI module, the Infrastructure project also
requires:

``` text
Google.GenAI
```

Do not remove another module's package references when resolving
`.csproj` merge conflicts.

------------------------------------------------------------------------

## 17. Automated Testing

A dedicated test project was added/configured for Identity testing.

At the final post-merge verification:

``` text
9 tests passed
0 tests failed
```

Tests cover Identity/user behavior including:

-   Identity model/property behavior.
-   User creation through `UserManager`.
-   Role creation/assignment.
-   Duplicate-email rejection.
-   Password reset behavior.
-   Active/inactive state persistence.
-   Current-user service behavior.

Tests use an EF Core in-memory Identity setup where appropriate,
avoiding modification of the developer's real Identity database.

------------------------------------------------------------------------

## 18. Integration/Merge Work Completed

The M1 feature was developed on:

``` text
feature/identity-access
```

It was pushed to GitHub and merged into:

``` text
main
```

During integration, conflicts were resolved while preserving work from
the other modules.

Important shared files that required merge handling included:

``` text
AIITSM.Infrastructure/AIITSM.Infrastructure.csproj
AIITSM.Web/Program.cs
AIITSM.Web/appsettings.json
```

The merged solution was rebuilt successfully and all 9 Identity tests
passed.

------------------------------------------------------------------------

## 19. Generated Files / Repository Cleanup

The repository has previously tracked Visual Studio-generated files
under locations such as:

``` text
.vs/
obj/
```

These caused a significant number of unnecessary Git merge conflicts.

The repository `.gitignore` already contains rules for these generated
files, but files committed before the ignore rules may remain tracked.

Team members should **not intentionally commit `.vs`, `bin`, or `obj`
output**.

A separate repository-cleanup change is recommended to stop tracking
existing generated files rather than mixing that cleanup with feature
changes.

------------------------------------------------------------------------

## 20. Guidance for Team Members

When integrating with M1:

1.  Use ASP.NET Core authentication instead of creating a separate login
    mechanism.
2.  Use the existing standard roles.
3.  Use `[Authorize]` / role authorization for protected Web actions.
4.  Use M1's current-user abstraction where its string Identity user ID
    is compatible.
5.  Do not directly insert/update ASP.NET Identity tables.
6.  Use `UserManager<ApplicationUser>` and
    `RoleManager<ApplicationRole>` for Identity operations.
7.  Do not commit passwords, Gemini keys, or other secrets.
8.  Preserve all module registrations when editing shared `Program.cs`.
9.  Coordinate user foreign-key/type changes through Database v2.
10. Run the complete build and test suite after changing shared
    Identity/database configuration.

------------------------------------------------------------------------

## 21. Current Integration Status

  Area                              Status
  --------------------------------- ---------------------
  Identity setup                    Complete
  Login/logout                      Complete
  RBAC                              Complete
  Standard roles                    Complete
  Admin user management             Complete
  Activation/deactivation           Complete
  Duplicate email protection        Complete
  Password reset                    Complete
  Access Denied                     Complete
  Admin dashboard/navigation        Complete
  Identity database/migrations      Complete
  Current-user abstraction          Complete
  Automated Identity tests          9 passing
  Merge into `main`                 Complete
  Solution rebuild after merge      Successful
  M1 ↔ M2 user-ID unification       Pending Database v2
  Production secret configuration   Deployment concern
  `.vs`/`obj` repository cleanup    Recommended

------------------------------------------------------------------------

## 22. Main Remaining Cross-Team Task

The most important remaining integration task is to establish a **single
user-reference strategy in Database v2**.

Before changing Incident/Agent/Audit tables, the team should agree on
whether those records reference:

``` text
ApplicationUser.Id (string)
```

directly or use another centrally defined mapping strategy.

Once that is decided, the temporary M2 current-user implementation can
be removed and Incident Management can consume the authenticated M1 user
consistently.

------------------------------------------------------------------------

**Reference status:** Prepared after M1 Identity & Access
implementation, testing, conflict resolution, and merge into `main`.
