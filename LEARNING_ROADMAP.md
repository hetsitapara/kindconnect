# KindConnect - 5-Step Learning Roadmap

## 📋 Overview
**KindConnect** is a Volunteer Management System built with ASP.NET Core MVC that connects NGOs with volunteers. This roadmap will guide you through understanding the entire project systematically.

---

## 🎯 STEP 1: Understand the Project Purpose & Architecture (30 mins)

### What This Project Does
- **Purpose**: Platform connecting NGOs with volunteers for community events
- **Key Features**:
  - NGOs create and manage volunteer events
  - Volunteers browse and apply to events
  - Superuser approves NGO registrations and monitors system
  - Application tracking (Pending/Approved/Rejected/Cancelled)

### Technology Stack
- **Framework**: ASP.NET Core 8.0 MVC
- **Authentication**: ASP.NET Core Identity
- **Database**: SQL Server with Entity Framework Core
- **Frontend**: Razor Views (server-side rendering)
- **Additional Libraries**: 
  - CsvHelper (data export)
  - iTextSharp (PDF generation)

### Architecture Pattern: MVC
```
Models/ ────────> Data entities (ApplicationUser, Event, NGOProfile, VolunteerApplication)
Views/ ─────────> Razor templates for UI
Controllers/ ───> Business logic and request handling
Data/ ──────────> Database context and seeding
```

### Three User Roles
1. **Superuser** - System admin (approves NGOs, views all data)
2. **NGO** - Organization (creates events, manages applications)
3. **Volunteer** - End user (applies to events)

### 📝 Action Items
- [ ] Read `README.md` and `PROJECT_WORKFLOW.md`
- [ ] Review `Program.cs` to understand app startup
- [ ] Check `VolunteerManagementSystem.csproj` for dependencies
- [ ] Understand the 3 roles and their permissions

---

## 🗄️ STEP 2: Study the Data Models (45 mins)

### Core Models Location: `/Models/`

#### 1. ApplicationUser.cs
```
Extends IdentityUser (built-in ASP.NET Identity)
├── FirstName, LastName
├── Phone
├── CreatedAt, UpdatedAt
├── IsActive (soft delete flag)
└── Navigation: NGOProfile (1-to-1)
```

#### 2. NGOProfile.cs
```
One NGO profile per NGO user
├── OrganizationName, Description
├── Website, Address, City, State, ZipCode
├── ContactEmail, ContactPhone
├── LogoPath (file upload)
├── IsActive (approval status by Superuser)
└── Navigation: Events (1-to-many)
```

#### 3. Event.cs
```
Events created by NGOs
├── Title, Description, Category
├── EventDate, StartTime, EndTime
├── Venue, City, State
├── MaxVolunteers (capacity)
├── IsPublic (visibility)
├── ContactEmail, ContactPhone
├── IsActive (soft delete)
├── Computed Properties:
│   ├── IsFull (checks current vs max volunteers)
│   └── IsUpcoming (checks if event is in future)
└── Navigation: VolunteerApplications (1-to-many)
```

#### 4. VolunteerApplication.cs
```
Links volunteers to events
├── EventId, UserId (composite unique key)
├── ApplicationMessage
├── Skills, Availability
├── Status (Pending/Approved/Rejected/Cancelled)
├── ResponseMessage (from NGO)
├── AppliedAt, UpdatedAt
└── Navigation: Event, User
```

### Database Relationships
```
ApplicationUser (NGO) ──1:1──> NGOProfile
NGOProfile ──1:many──> Event
Event ──1:many──> VolunteerApplication
ApplicationUser (Volunteer) ──1:many──> VolunteerApplication
```

### 📝 Action Items
- [ ] Read all 4 model files in `/Models/`
- [ ] Draw the entity relationship diagram on paper
- [ ] Check `/Data/ApplicationDbContext.cs` for DbContext configuration
- [ ] Review `/Data/SeedData.cs` to see sample data creation
- [ ] Look at `/Migrations/` to understand database schema

---

## 🎮 STEP 3: Explore Controllers & Business Logic (60 mins)

### Controllers Location: `/Controllers/`

#### 1. AccountController.cs (Authentication)
**Key Actions:**
- `Register` - User registration (choose NGO or Volunteer role)
- `Login` - Authentication
- `Logout` - Sign out
- `Manage` - User profile management
- **Flow**: Register → Creates user with role → If NGO, creates NGOProfile (pending approval)

#### 2. HomeController.cs (Public Pages)
**Key Actions:**
- `Index` - Landing page with public statistics
- `Privacy` - Privacy policy page
- **Note**: Uses `[AllowAnonymous]` for public access

#### 3. DashboardController.cs (Role-based Dashboards)
**Key Actions:**
- `Superuser` - Global stats (users, NGOs, events, applications)
- `Volunteer` - Personal stats and upcoming events
- **Authorization**: Role-specific views

#### 4. NGOProfilesController.cs (NGO Management)
**Key Actions:**
- `Create`, `Edit` - NGO profile management
- `Approve`, `Reject` - Superuser actions
- `Delete` - Soft delete (sets IsActive=false)
- **Business Rule**: NGO must be approved to create events

#### 5. EventsController.cs (Event Management)
**Key Actions:**
- `Index` - List events (filtered by role)
- `Details` - Event details with applications
- `Create`, `Edit`, `Delete` - Event CRUD (NGO only)
- `ExportEventsToPdf`, `ExportEventsToCSV` - Data export
- **Business Rules**: 
  - Only approved NGOs can create events
  - Validates dates and capacity

#### 6. VolunteerApplicationsController.cs (Applications)
**Key Actions:**
- `Apply` - Volunteer applies to event
- `Index` - Volunteer views their applications
- `Manage` - NGO views applications for their event
- `ManageAll` - NGO views all applications across events
- `Approve`, `Reject` - NGO responds to applications
- `Cancel` - Volunteer cancels their application
- **Business Rules**:
  - No duplicate applications
  - Can't apply to full events
  - Unique constraint on (EventId, UserId)

#### 7. SuperAdminController.cs (Admin Panel)
**Key Actions:**
- `Volunteers`, `VolunteerDetails` - View volunteer data
- `ExportVolunteersToPdf`, `ExportVolunteersToCSV` - Export
- **Authorization**: Superuser only

### Authorization Patterns
```csharp
[Authorize(Roles = "Superuser")]        // Superuser only
[Authorize(Roles = "NGO")]              // NGO only
[Authorize(Roles = "Volunteer")]        // Volunteer only
[Authorize]                             // Any authenticated user
[AllowAnonymous]                        // Public access
```

### 📝 Action Items
- [ ] Read each controller file top to bottom
- [ ] Trace one complete flow: Volunteer applies → NGO approves
- [ ] Identify where authorization checks happen
- [ ] Note how soft deletes work (IsActive flags)
- [ ] Check how file uploads work (NGO logos)

---

## 🎨 STEP 4: Understand the Views & User Interface (45 mins)

### Views Location: `/Views/`

#### Folder Structure
```
Views/
├── Account/          → Login, Register, Manage
├── Dashboard/        → Superuser, Volunteer dashboards
├── Events/           → Index, Details, Create, Edit, Delete
├── Home/             → Index (landing), Privacy
├── SuperAdmin/       → Volunteers, VolunteerDetails
├── VolunteerApplications/ → Apply, Index, Manage, ManageAll, Details
└── Shared/           → _Layout, _LoginPartial, Error
```

#### Key View Files to Study

**1. Shared/_Layout.cshtml**
- Main layout template (navbar, footer)
- Navigation changes based on user role
- Shows different menu items for Superuser/NGO/Volunteer

**2. Account/Register.cshtml**
- Registration form
- Role selection (NGO vs Volunteer)
- Additional fields for NGO users

**3. Events/Index.cshtml**
- Lists events
- Filters based on role:
  - Volunteers see public, active, upcoming events
  - NGOs see their own events
  - Superuser sees all events

**4. Events/Details.cshtml**
- Event information
- Apply button for volunteers
- Applications list for event owner (NGO)

**5. VolunteerApplications/Apply.cshtml**
- Application form (message, skills, availability)
- Validation to prevent duplicate/full event applications

**6. Dashboard/Superuser.cshtml**
- System-wide statistics
- Charts and graphs
- Recent activity

**7. Dashboard/Volunteer.cshtml**
- Personal application statistics
- Upcoming approved events
- Recent applications

#### View Components & Patterns
- **Razor Syntax**: `@model`, `@if`, `@foreach`, `@Html`, `@Url`
- **Tag Helpers**: `asp-controller`, `asp-action`, `asp-route-*`
- **Partial Views**: `_LoginPartial` for login/logout UI
- **ViewData/ViewBag**: Passing data from controller to view

### 📝 Action Items
- [ ] Open `Views/Shared/_Layout.cshtml` - understand the main template
- [ ] Review navigation logic in _Layout (how menus change by role)
- [ ] Study one complete CRUD flow: Events (Index → Details → Create → Edit)
- [ ] Check form validation in views
- [ ] Look at how ViewModels are used (in `/Models/ViewModels/`)

---

## 🚀 STEP 5: Run & Test the Application (60 mins)

### Prerequisites
- SQL Server installed and running
- .NET 8.0 SDK installed
- Visual Studio or VS Code

### Setup Steps

**1. Configure Database**
```json
// In appsettings.json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=KindConnectDB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

**2. Apply Migrations**
```bash
dotnet ef database update
```

**3. Run Application**
```bash
dotnet run
```

**4. Default Seed Accounts**
- **Superuser**: `superadmin@gmail.com` / Password from SeedData
- **NGO**: `ngo@example.com` / Password from SeedData
- **Volunteer**: `volunteer@example.com` / Password from SeedData

### Testing Workflow

#### Test Scenario 1: NGO Creates Event
1. Login as `ngo@example.com`
2. Navigate to Events → Create New Event
3. Fill form and submit
4. Verify event appears in Events list

#### Test Scenario 2: Volunteer Applies
1. Login as `volunteer@example.com`
2. Browse Events → Click event Details
3. Click "Apply" button
4. Fill application form and submit
5. Check "My Applications" to see status

#### Test Scenario 3: NGO Reviews Application
1. Login as `ngo@example.com`
2. Go to event Details
3. View applications list
4. Approve or Reject application with message

#### Test Scenario 4: Superuser Oversight
1. Login as `superadmin@gmail.com`
2. View dashboard statistics
3. Navigate to Volunteers section
4. Export data to PDF/CSV

#### Test Scenario 5: New NGO Registration
1. Logout
2. Register new account with NGO role
3. Login as new NGO → See "Pending Approval" message
4. Login as Superuser → Approve new NGO
5. Login as NGO again → Can now create events

### What to Observe
- **Authorization**: Try accessing pages you shouldn't (e.g., Volunteer accessing NGO pages)
- **Validation**: Try submitting invalid data
- **Business Rules**: Try applying twice to same event, applying to full event
- **Soft Deletes**: Delete an event, check database (IsActive=false)
- **File Uploads**: Upload NGO logo, verify it appears

### 📝 Action Items
- [ ] Set up database connection
- [ ] Run migrations
- [ ] Start application and login with all 3 roles
- [ ] Complete all 5 test scenarios
- [ ] Try breaking the rules (duplicate applications, etc.)
- [ ] Check database tables in SQL Server Management Studio
- [ ] Test export features (PDF/CSV)
- [ ] Review browser network tab to see requests

---

## 🎓 Bonus: Advanced Understanding

### Security Features
- Password hashing (ASP.NET Identity)
- CSRF protection (built into Razor forms)
- Authorization policies
- SQL injection prevention (EF Core parameterized queries)

### Performance Considerations
- Eager loading: `.Include()` for related data
- Lazy loading disabled (explicit includes required)
- Indexes on foreign keys

### Code Quality Patterns
- Repository pattern (via DbContext)
- ViewModel pattern (separating display from domain models)
- Dependency injection (services in constructors)
- Async/await for database operations

### Future Enhancements You Could Add
- Email notifications
- Event search and filtering
- Volunteer ratings/reviews
- Event categories with images
- Real-time notifications (SignalR)
- Mobile responsive design improvements

---

## 📚 Quick Reference

### File Navigation Cheat Sheet
```
Need to understand...          → Look at...
├── App startup                → Program.cs
├── Database schema            → Models/*.cs, Migrations/
├── User registration          → Controllers/AccountController.cs
├── Event creation             → Controllers/EventsController.cs
├── Application process        → Controllers/VolunteerApplicationsController.cs
├── Admin features             → Controllers/SuperAdminController.cs
├── Page layouts               → Views/Shared/_Layout.cshtml
├── Seed data                  → Data/SeedData.cs
└── Database queries           → Data/ApplicationDbContext.cs
```

### Common Commands
```bash
# Build project
dotnet build

# Run application
dotnet run

# Create migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update

# Restore packages
dotnet restore
```

---

## ✅ Completion Checklist

By the end of this roadmap, you should be able to:
- [ ] Explain the purpose and architecture of KindConnect
- [ ] Describe all 4 data models and their relationships
- [ ] Trace the flow from volunteer application to NGO approval
- [ ] Identify the 3 user roles and their permissions
- [ ] Navigate the codebase confidently
- [ ] Run and test the application
- [ ] Explain key features in a demo/presentation
- [ ] Modify and extend the codebase

---

**Good luck with your learning journey! 🚀**

*Estimated total time: 4 hours*
