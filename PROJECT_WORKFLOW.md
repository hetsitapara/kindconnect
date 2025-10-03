## KindConnect / Volunteer Management System - Workflow Guide

This document explains the high-level flow of the application so you can comfortably present it in a viva.

### 1) Roles and Responsibilities
- **Superuser**: System administrator. Approves/rejects NGO profiles, views global stats, can manage everything.
- **NGO**: Organization user. Manages their NGO profile and creates/edits events. Can review and respond to volunteer applications for their events.
- **Volunteer**: End user. Views public events and applies to participate.

### 2) Application Startup
- `Program.cs` configures services:
  - SQL Server via `ApplicationDbContext`
  - ASP.NET Core Identity with roles (Superuser, NGO, Volunteer)
  - MVC controllers with views and a default authorization policy (auth required by default)
- Middleware pipeline: HTTPS redirection → Static files → Routing → Authentication → Authorization → MVC route mapping.
- On startup, database seeding runs (`SeedData.Initialize`): ensures roles and sample users exist (superuser, one NGO, one volunteer) and creates a sample NGO profile.

### 3) Data Model Overview
- `ApplicationUser`: Extends IdentityUser with profile fields (`FirstName`, `LastName`, `Phone`, timestamps, `IsActive`).
- `NGOProfile`: One-to-one with `ApplicationUser` (owner). Stores organization details and `IsActive` for approval status.
- `Event`: Created by an NGO; includes schedule, venue, capacity, visibility, contact fields. Computed helpers like `IsFull` and `IsUpcoming`.
- `VolunteerApplication`: Links a volunteer (`UserId`) to an event with a message, skills, and status (`Pending/Approved/Rejected/Cancelled`). Unique per `(EventId, UserId)`.

### 4) Authentication & Authorization
- Global policy requires authentication. Public pages or actions use `[AllowAnonymous]` explicitly.
- Role checks:
  - Superuser-only actions: approval/rejection of NGOs, global dashboards.
  - NGO actions: create/edit/delete own events; manage applications for their events.
  - Volunteer actions: apply/cancel their own applications.

### 5) Main User Journeys
1. Registration & Login (`AccountController`)
   - User registers as Volunteer or NGO.
   - If NGO, an `NGOProfile` is created in a pending state (`IsActive=false`). Superuser must approve it.
   - After login: NGOs go to dashboard/events; Volunteers go to events list.

2. NGO Profile Management (`NGOProfilesController`)
   - NGO creates/edits their profile and optionally uploads a logo.
   - Superuser can approve, reject (disable), or soft-delete profiles.

3. Event Lifecycle (`EventsController`)
   - NGOs (with approved profile) create events, setting schedule, capacity, visibility, and contact details.
   - Anyone authenticated can view events; Volunteers typically see only active & public events. Superuser can see all.
   - NGOs can edit or soft-delete their own events. Soft-delete sets `IsActive=false`.

4. Volunteer Applications (`VolunteerApplicationsController`)
   - Volunteers open an event and apply with a message and optional skills/availability.
   - The system prevents duplicate applications and applying to full events.
   - NGOs review applications for their events, then approve or reject with an optional response message.
   - Volunteers can cancel their own pending applications.

### 6) Dashboards (`DashboardController`)
- Superuser: Global stats (users, NGOs, events, applications) and recent activity.
- Volunteer: Personal stats (counts by status), list of their recent applications, and upcoming approved events.
- NGO: Redirected to events and event details with applications list for owned events.

### 7) Views Overview
- Razor views under `Views/` correspond to controller actions. Key pages:
  - `Home/Index`: Landing page with public stats.
  - `Account/Register`, `Account/Login`, `Account/Manage`: Auth & profile.
  - `Events/Index`, `Events/Details`, `Events/Create`, `Events/Edit`, `Events/Delete`.
  - `VolunteerApplications/Apply`, `VolunteerApplications/Index`, `VolunteerApplications/Manage`, `ManageAll`, `Details`.
  - `Dashboard/Superuser`, `Dashboard/Volunteer`.

### 8) Important Business Rules
- NGO profile must be approved to create/manage events (unless the user is Superuser).
- Events enforce time and capacity rules at creation/edit.
- Volunteer applications are unique per event/user and blocked if the event is full.
- Soft-deletes are used for events and NGO profiles via `IsActive` flags.

### 9) Configuration & Environment
- `appsettings.json` defines `DefaultConnection` for SQL Server.
- Development vs Production behavior: error handling and HSTS in production.

### 10) How to Demo (Step-by-step)
1. Log in as `superadmin@gmail.com` (password from seed). Show Superuser dashboard.
2. Log in as `ngo@example.com`. Show creating an event and then viewing it in the events list.
3. Log in as `volunteer@example.com`. Apply to the event.
4. Back as NGO: approve or reject the application.
5. As Volunteer: show application status and upcoming events.

### 11) File Map (Where to look)
- Startup & config: `Program.cs`
- Data layer: `Data/ApplicationDbContext.cs`, `Data/SeedData.cs`, `Migrations/`
- Domain models: `Models/*.cs`
- Controllers: `Controllers/*.cs`
- Views: `Views/**`


