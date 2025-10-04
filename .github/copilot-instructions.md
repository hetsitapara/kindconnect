<!--
Guidance for AI coding agents working on KindConnect (VolunteerManagementSystem)
Keep this file short, concrete, and codebase-specific. Reference files are given as examples.
--> 

# KindConnect — AI agent instructions

This file contains concise, actionable guidance for an AI coding assistant working on the KindConnect ASP.NET Core MVC application.

Key facts (read before editing):
- App entry: `Program.cs` — uses ASP.NET Core minimal host, configures EF Core (SQL Server) and Identity.
- DbContext: `Data/ApplicationDbContext.cs` — IdentityDbContext<ApplicationUser> plus DbSets: `NGOProfiles`, `Events`, `VolunteerApplications`.
- Seeding: `Data/SeedData.cs` — calls `context.Database.EnsureCreatedAsync()` and creates 3 roles and sample users (superadmin/ngo/volunteer). Changes that require migrations should not rely on EnsureCreated in production.
- Views: Razor MVC in `Views/` and controllers in `Controllers/` using conventional routes (default route: `{controller=Home}/{action=Index}/{id?}`).

What to do and what to avoid
- Preserve Identity and seeding behavior unless explicitly asked to change authentication or initial user accounts. See `SeedData.cs` for passwords and seeded emails.
- Prefer adding EF Core Migrations for schema changes (project already contains `Migrations/`). Avoid breaking `EnsureCreated` behavior used during local seeding.
- Global authorization: the app sets a fallback policy requiring authentication (Program.cs). Any public endpoints must be decorated with `[AllowAnonymous]`.
- Role checks: code expects roles named `Superuser`, `NGO`, and `Volunteer`. Use these exact names when adding role checks or creating seed roles.

Common patterns and examples (copy-paste friendly)
- Creating a controller action that requires an NGO owner check: follow `EventsController.Create/Edit/Delete` — fetch `NGOProfiles` by `User.Id` via `UserManager<ApplicationUser>` and validate ownership before mutating events.
- Soft-delete events: set `Event.IsActive = false` and update `UpdatedAt` (see `EventsController.DeleteConfirmed`). Do not physically delete event rows without considering related `VolunteerApplication` constraints.
- Prevent duplicate volunteer applications: the database enforces a unique index on `(EventId, UserId)` (see `ApplicationDbContext.OnModelCreating`). Handle DbUpdateException or check existence before insert.

Build, run, and dev workflows
- Local run (assumes SQL Server and `DefaultConnection` configured in `appsettings.json`): dotnet run from project root (the repo uses .NET 8, see `VolunteerManagementSystem.csproj` TargetFramework).
- Database migrations: use EF tools (project already references `Microsoft.EntityFrameworkCore.Tools`). Example commands (run in project folder):
  - dotnet ef migrations add <Name>
  - dotnet ef database update
- Seed data: Happens automatically on startup via `SeedData.Initialize(...)` in `Program.cs`. To re-seed during development, drop the local DB and restart.

Tests and linting
- There are no automated tests in the repo. If adding tests, follow folder pattern `Tests/` and prefer xUnit or MSTest. Keep database access mocked or use the EF InMemory provider for unit tests.

Integration points and notable dependencies
- Uses SQL Server via `Microsoft.EntityFrameworkCore.SqlServer` (see `VolunteerManagementSystem.csproj`).
- Identity UI is used; account pages live under `Views/Account` and the project uses `ApplicationUser` (see `Models/ApplicationUser.cs`).
- Uses CsvHelper and iTextSharp for exports (search `CsvHelper` or `iTextSharp` usages before editing related code).

When changing authentication, roles, or seeding
- If you change role names or required roles, update both `ApplicationDbContext.OnModelCreating` role seed and `SeedData` creation logic. Keep role NormalizedName values uppercase.
- If you convert EnsureCreated -> MigrateAsync in SeedData, ensure migrations exist and are applied in CI before deployment.

Files to inspect for context
- `Program.cs`, `Data/ApplicationDbContext.cs`, `Data/SeedData.cs`, `Controllers/EventsController.cs`, `Controllers/VolunteerApplicationsController.cs`, `Models/*`, `Views/*`, `Migrations/*`, `VolunteerManagementSystem.csproj`, `README.md`, `PROJECT_WORKFLOW.md`.

If uncertain about runtime secrets or database
- Do not attempt to read or modify secrets. Respect `appsettings.json` conventions and use IConfiguration for any new settings.

Ask the maintainer if:
- You need to change seeded passwords or initial admin emails.
- You want to switch production seeding from EnsureCreated to migrations.
- You need to add background services or change the authentication scheme.

-- End
