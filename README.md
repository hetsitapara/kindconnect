# KindConnect - Volunteer Management System

This is a role-based ASP.NET Core MVC application to connect NGOs with volunteers.

## Quick Start
- Ensure SQL Server is available and update `DefaultConnection` in `appsettings.json`.
- Run the application. On first start, seed data creates:
  - Superuser: `superadmin@gmail.com`
  - NGO: `ngo@example.com`
  - Volunteer: `volunteer@example.com`

## Project Workflow
See `PROJECT_WORKFLOW.md` for a complete explanation of the app flow, roles, and demo steps.


-----------------------------------------------------------------(work distribution)-------------------------------------------------------------------------------------------

## Het_Sitapara(CE_147): Authentication & User Management Module**

### **Responsibility**
Handle all user authentication, authorization, account management, and user profile operations.

### **Files to Work On**

#### **Controllers**
- [Controllers/AccountController.cs](cci:7://file:///Users/hetsitapara/Desktop/KindConnect-main%20copy/Controllers/AccountController.cs:0:0-0:0) (280 lines)
  - User registration (Volunteer & NGO)
  - Login/Logout
  - Profile management
  - Access control

#### **Models**
- [Models/ApplicationUser.cs](cci:7://file:///Users/hetsitapara/Desktop/KindConnect-main%20copy/Models/ApplicationUser.cs:0:0-0:0) (35 lines)
  - User entity with Identity integration
  - User properties (FirstName, LastName, Phone, etc.)
- [Models/ViewModels/RegisterViewModel.cs](cci:7://file:///Users/hetsitapara/Desktop/KindConnect-main%20copy/Users/hetsitapara/Desktop/KindConnect-main%20copy/Models/ViewModels/RegisterViewModel.cs:0:0-0:0)
  - Registration form data

#### **Views**
- [Views/Account/Register.cshtml](cci:7://file:///Users/hetsitapara/Desktop/KindConnect-main%20copy/Users/hetsitapara/Desktop/KindConnect-main%20copy/Views/Account/Register.cshtml:0:0-0:0)
- [Views/Account/Login.cshtml](cci:7://file:///Users/hetsitapara/Desktop/KindConnect-main%20copy/Users/hetsitapara/Desktop/KindConnect-main%20copy/Views/Account/Login.cshtml:0:0-0:0)
- [Views/Account/Manage.cshtml](cci:7://file:///Users/hetsitapara/Desktop/KindConnect-main%20copy/Users/hetsitapara/Desktop/KindConnect-main%20copy/Views/Account/Manage.cshtml:0:0-0:0)
- [Views/Account/EditProfile.cshtml](cci:7://file:///Users/hetsitapara/Desktop/KindConnect-main%20copy/Users/hetsitapara/Desktop/KindConnect-main%20copy/Views/Account/EditProfile.cshtml:0:0-0:0)
- [Views/Account/AccessDenied.cshtml](cci:7://file:///Users/hetsitapara/Desktop/KindConnect-main%20copy/Users/hetsitapara/Desktop/KindConnect-main%20copy/Views/Account/AccessDenied.cshtml:0:0-0:0)
- [Views/Shared/_LoginPartial.cshtml](cci:7://file:///Users/hetsitapara/Desktop/KindConnect-main%20copy/Users/hetsitapara/Desktop/KindConnect-main%20copy/Views/Shared/_LoginPartial.cshtml:0:0-0:0)

#### **Data & Configuration**
- [Data/SeedData.cs](cci:7://file:///Users/hetsitapara/Desktop/KindConnect-main%20copy/Users/hetsitapara/Desktop/KindConnect-main%20copy/Data/SeedData.cs:0:0-0:0)
  - Initial user/role seeding
  - Demo data creation
- Part of [Program.cs](cci:7://file:///Users/hetsitapara/Desktop/KindConnect-main%20copy/Program.cs:0:0-0:0) (lines 22-44)
  - Identity configuration
  - Authentication/Authorization setup

#### **Key Responsibilities**
- ✅ User registration with role selection (Volunteer/NGO)
- ✅ Login/logout functionality
- ✅ Profile editing
- ✅ Password validation
- ✅ Role-based access control setup
- ✅ Session management

---

## Vanshit Gadoya(CE_080): NGO & Event Management Module**

### **Responsibility**
Handle NGO profiles, event creation/management, and NGO-specific dashboards.

### **Files to Work On**

#### **Controllers**
- [Controllers/EventsController.cs](cci:7://file:///Users/hetsitapara/Desktop/KindConnect-main%20copy/Controllers/EventsController.cs:0:0-0:0) (452 lines)
  - Event CRUD operations
  - Event listing with filters
  - Event browsing for volunteers
- [Controllers/NGOProfilesController.cs](cci:7://file:///Users/hetsitapara/Desktop/KindConnect-main%20copy/Controllers/NGOProfilesController.cs:0:0-0:0) (262 lines)
  - NGO profile CRUD
  - Logo upload handling
- [Controllers/DashboardController.cs](cci:7://file:///Users/hetsitapara/Desktop/KindConnect-main%20copy/Controllers/DashboardController.cs:0:0-0:0) (148 lines)
  - NGO dashboard
  - Event details dashboard

#### **Models**
- [Models/Event.cs](cci:7://file:///Users/hetsitapara/Desktop/KindConnect-main%20copy/Models/Event.cs:0:0-0:0) (82 lines)
  - Event entity
  - Event properties and computed fields
  - ApplicationStatus enum
- [Models/NGOProfile.cs](cci:7://file:///Users/hetsitapara/Desktop/KindConnect-main%20copy/Models/NGOProfile.cs:0:0-0:0) (56 lines)
  - NGO profile entity
- [Models/ViewModels/CreateEventViewModel.cs](cci:7://file:///Users/hetsitapara/Desktop/KindConnect-main%20copy/Users/hetsitapara/Desktop/KindConnect-main%20copy/Models/ViewModels/CreateEventViewModel.cs:0:0-0:0)
- [Models/ViewModels/EditEventViewModel.cs](cci:7://file:///Users/hetsitapara/Desktop/KindConnect-main%20copy/Users/hetsitapara/Desktop/KindConnect-main%20copy/Models/ViewModels/EditEventViewModel.cs:0:0-0:0)
- [Models/ViewModels/CreateNGOProfileViewModel.cs](cci:7://file:///Users/hetsitapara/Desktop/KindConnect-main%20copy/Users/hetsitapara/Desktop/KindConnect-main%20copy/Models/ViewModels/CreateNGOProfileViewModel.cs:0:0-0:0)
- [Models/ViewModels/EditNGOProfileViewModel.cs](cci:7://file:///Users/hetsitapara/Desktop/KindConnect-main%20copy/Users/hetsitapara/Desktop/KindConnect-main%20copy/Models/ViewModels/EditNGOProfileViewModel.cs:0:0-0:0)

#### **Views**
- [Views/Events/Index.cshtml](cci:7://file:///Users/hetsitapara/Desktop/KindConnect-main%20copy/Users/hetsitapara/Desktop/KindConnect-main%20copy/Views/Events/Index.cshtml:0:0-0:0)
- [Views/Events/Create.cshtml](cci:7://file:///Users/hetsitapara/Desktop/KindConnect-main%20copy/Users/hetsitapara/Desktop/KindConnect-main%20copy/Views/Events/Create.cshtml:0:0-0:0)
- [Views/Events/Edit.cshtml](cci:7://file:///Users/hetsitapara/Desktop/KindConnect-main%20copy/Users/hetsitapara/Desktop/KindConnect-main%20copy/Views/Events/Edit.cshtml:0:0-0:0)
- [Views/Events/Delete.cshtml](cci:7://file:///Users/hetsitapara/Desktop/KindConnect-main%20copy/Users/hetsitapara/Desktop/KindConnect-main%20copy/Views/Events/Delete.cshtml:0:0-0:0)
- [Views/Events/Details.cshtml](cci:7://file:///Users/hetsitapara/Desktop/KindConnect-main%20copy/Users/hetsitapara/Desktop/KindConnect-main%20copy/Views/Events/Details.cshtml:0:0-0:0)
- [Views/Dashboard/EventDetails.cshtml](cci:7://file:///Users/hetsitapara/Desktop/KindConnect-main%20copy/Users/hetsitapara/Desktop/KindConnect-main%20copy/Views/Dashboard/EventDetails.cshtml:0:0-0:0)
- `Views/NGOProfiles/*` (all NGO profile views)

#### **Key Responsibilities**
- ✅ Event creation, editing, deletion (NGO-owned)
- ✅ Event listing with search/filter by category and NGO
- ✅ Event capacity management
- ✅ NGO profile management
- ✅ Logo/image upload handling
- ✅ Event visibility controls (public/private)
- ✅ NGO dashboard with statistics

---

## Darsh Gadara(CE_078): Volunteer Application & Admin Module**

### **Responsibility**
Handle volunteer applications, application management, and superadmin functionalities.

### **Files to Work On**

#### **Controllers**
- [Controllers/VolunteerApplicationsController.cs](cci:7://file:///Users/hetsitapara/Desktop/KindConnect-main%20copy/Controllers/VolunteerApplicationsController.cs:0:0-0:0) (354 lines)
  - Volunteer application submission
  - Application approval/rejection
  - Application management for NGOs
  - Application cancellation
- [Controllers/SuperAdminController.cs](cci:7://file:///Users/hetsitapara/Desktop/KindConnect-main%20copy/Controllers/SuperAdminController.cs:0:0-0:0) (192 lines)
  - Superuser dashboard
  - Volunteer management
  - NGO management
  - System-wide statistics
- [Controllers/DashboardController.cs](cci:7://file:///Users/hetsitapara/Desktop/KindConnect-main%20copy/Controllers/DashboardController.cs:0:0-0:0) (partial - Volunteer & Superuser dashboards)
  - Volunteer dashboard
  - Superuser dashboard
- [Controllers/HomeController.cs](cci:7://file:///Users/hetsitapara/Desktop/KindConnect-main%20copy/Controllers/HomeController.cs:0:0-0:0) (1313 bytes)
  - Public home page

#### **Models**
- [Models/VolunteerApplication.cs](cci:7://file:///Users/hetsitapara/Desktop/KindConnect-main%20copy/Models/VolunteerApplication.cs:0:0-0:0) (50 lines)
  - Application entity
  - Application status tracking
- [Models/ViewModels/VolunteerApplicationViewModel.cs](cci:7://file:///Users/hetsitapara/Desktop/KindConnect-main%20copy/Users/hetsitapara/Desktop/KindConnect-main%20copy/Models/ViewModels/VolunteerApplicationViewModel.cs:0:0-0:0)

#### **Views**
- [Views/VolunteerApplications/Index.cshtml](cci:7://file:///Users/hetsitapara/Desktop/KindConnect-main%20copy/Users/hetsitapara/Desktop/KindConnect-main%20copy/Views/VolunteerApplications/Index.cshtml:0:0-0:0)
- [Views/VolunteerApplications/Apply.cshtml](cci:7://file:///Users/hetsitapara/Desktop/KindConnect-main%20copy/Users/hetsitapara/Desktop/KindConnect-main%20copy/Views/VolunteerApplications/Apply.cshtml:0:0-0:0)
- [Views/VolunteerApplications/Details.cshtml](cci:7://file:///Users/hetsitapara/Desktop/KindConnect-main%20copy/Users/hetsitapara/Desktop/KindConnect-main%20copy/Views/VolunteerApplications/Details.cshtml:0:0-0:0)
- [Views/VolunteerApplications/Manage.cshtml](cci:7://file:///Users/hetsitapara/Desktop/KindConnect-main%20copy/Users/hetsitapara/Desktop/KindConnect-main%20copy/Views/VolunteerApplications/Manage.cshtml:0:0-0:0)
- [Views/VolunteerApplications/ManageAll.cshtml](cci:7://file:///Users/hetsitapara/Desktop/KindConnect-main%20copy/Users/hetsitapara/Desktop/KindConnect-main%20copy/Views/VolunteerApplications/ManageAll.cshtml:0:0-0:0)
- `Views/SuperAdmin/Superuser.cshtml`
- [Views/SuperAdmin/Volunteers.cshtml](cci:7://file:///Users/hetsitapara/Desktop/KindConnect-main%20copy/Users/hetsitapara/Desktop/KindConnect-main%20copy/Views/SuperAdmin/Volunteers.cshtml:0:0-0:0)
- [Views/SuperAdmin/VolunteerDetails.cshtml](cci:7://file:///Users/hetsitapara/Desktop/KindConnect-main%20copy/Users/hetsitapara/Desktop/KindConnect-main%20copy/Views/SuperAdmin/VolunteerDetails.cshtml:0:0-0:0)
- [Views/SuperAdmin/NGOs.cshtml](cci:7://file:///Users/hetsitapara/Desktop/KindConnect-main%20copy/Users/hetsitapara/Desktop/KindConnect-main%20copy/Views/SuperAdmin/NGOs.cshtml:0:0-0:0)
- [Views/SuperAdmin/NGODetails.cshtml](cci:7://file:///Users/hetsitapara/Desktop/KindConnect-main%20copy/Users/hetsitapara/Desktop/KindConnect-main%20copy/Views/SuperAdmin/NGODetails.cshtml:0:0-0:0)
- [Views/Dashboard/Volunteer.cshtml](cci:7://file:///Users/hetsitapara/Desktop/KindConnect-main%20copy/Users/hetsitapara/Desktop/KindConnect-main%20copy/Views/Dashboard/Volunteer.cshtml:0:0-0:0)
- [Views/Dashboard/Superuser.cshtml](cci:7://file:///Users/hetsitapara/Desktop/KindConnect-main%20copy/Users/hetsitapara/Desktop/KindConnect-main%20copy/Views/Dashboard/Superuser.cshtml:0:0-0:0)
- `Views/Home/*` (all home views)

#### **Key Responsibilities**
- ✅ Volunteer application submission
- ✅ Application approval/rejection workflow
- ✅ Application status tracking (Pending, Approved, Rejected, Cancelled)
- ✅ Volunteer dashboard with application history
- ✅ Superadmin dashboard with system statistics
- ✅ User management (view/delete volunteers and NGOs)
- ✅ System-wide monitoring and reporting

---

## **Shared Components (All Developers)**

### **Common Files**
- [Program.cs](cci:7://file:///Users/hetsitapara/Desktop/KindConnect-main%20copy/Program.cs:0:0-0:0) - Application startup and configuration
- [Data/ApplicationDbContext.cs](cci:7://file:///Users/hetsitapara/Desktop/KindConnect-main%20copy/Users/hetsitapara/Desktop/KindConnect-main%20copy/Data/ApplicationDbContext.cs:0:0-0:0) - Database context
- `Migrations/*` - Database migrations
- [Views/Shared/_Layout.cshtml](cci:7://file:///Users/hetsitapara/Desktop/KindConnect-main%20copy/Users/hetsitapara/Desktop/KindConnect-main%20copy/Views/Shared/_Layout.cshtml:0:0-0:0) - Main layout
- [Views/Shared/_ValidationScriptsPartial.cshtml](cci:7://file:///Users/hetsitapara/Desktop/KindConnect-main%20copy/Users/hetsitapara/Desktop/KindConnect-main%20copy/Views/Shared/_ValidationScriptsPartial.cshtml:0:0-0:0)
- [Views/Shared/Error.cshtml](cci:7://file:///Users/hetsitapara/Desktop/KindConnect-main%20copy/Users/hetsitapara/Desktop/KindConnect-main%20copy/Views/Shared/Error.cshtml:0:0-0:0)
- [Views/_ViewImports.cshtml](cci:7://file:///Users/hetsitapara/Desktop/KindConnect-main%20copy/Users/hetsitapara/Desktop/KindConnect-main%20copy/Views/_ViewImports.cshtml:0:0-0:0)
- [Views/_ViewStart.cshtml](cci:7://file:///Users/hetsitapara/Desktop/KindConnect-main%20copy/Users/hetsitapara/Desktop/KindConnect-main%20copy/Views/_ViewStart.cshtml:0:0-0:0)
- `wwwroot/*` - Static assets (CSS, JS, images)
- [appsettings.json](cci:7://file:///Users/hetsitapara/Desktop/KindConnect-main%20copy/appsettings.json:0:0-0:0) - Configuration
- [VolunteerManagementSystem.csproj](cci:7://file:///Users/hetsitapara/Desktop/KindConnect-main%20copy/VolunteerManagementSystem.csproj:0:0-0:0) - Project file

---

## **Database Schema Overview**

### **Tables**
1. **AspNetUsers** (Identity) - Extended by ApplicationUser
2. **NGOProfiles** - NGO organization details
3. **Events** - Volunteer events
4. **VolunteerApplications** - Applications to events

### **Relationships**
- User (1) → NGOProfile (0..1)
- User (1) → VolunteerApplications (0..*)
- NGOProfile (1) → Events (0..*)
- Event (1) → VolunteerApplications (0..*)

---

## **Coordination Points**

### **Het_Sitapara ↔Vanshit Gadoya**
- User registration creates NGO profiles
- Authentication affects NGO dashboard access

### **Vanshit Gadoya ↔ Darsh Gadara**
- Events are created by NGOs (Dev 2) and applied to by volunteers (Dev 3)
- Event capacity affects application approval

### **Het_Sitapara ↔ Darsh Gadara**
- User roles determine application permissions
- Superadmin manages all users

---

## **Testing Responsibilities**

### **Het_Sitapara**
- Test registration for both Volunteer and NGO roles
- Test login/logout flows
- Test profile editing

### **Vanshit Gadoya**
- Test event CRUD operations
- Test event filtering and search
- Test NGO profile management
- Test file uploads

### **Darsh Gadara**
- Test application submission
- Test approval/rejection workflow
- Test volunteer dashboard
- Test superadmin operations

---

## **Development Workflow Recommendations**

1. **Phase 1**: Het_Sitapara completes authentication (foundation)
2. **Phase 2**: Vanshit Gadoya builds NGO/Event features (depends on auth)
3. **Phase 3**: Darsh Gadara builds application workflow (depends on events)
4. **Phase 4**: All developers integrate and test together

---

This division ensures:
- ✅ **Clear separation of concerns**
- ✅ **Minimal code conflicts**
- ✅ **Logical feature grouping**
- ✅ **Balanced workload** (~400-500 lines of controller code each)
- ✅ **Independent development** with clear integration points

## Summary

I've successfully divided the **KindConnect Volunteer Management System** codebase into three logical modules for three developers:

### **Module Breakdown**

1. **Het_Sitapara - Authentication & User Management** (~315 lines of controller code)
   - Account management, login/logout, user profiles, role-based access

2. **Vanshit Gadoya - NGO & Event Management** (~862 lines of controller code)
   - Event CRUD, NGO profiles, event browsing, dashboards

3. **Darsh Gadara- Volunteer Application & Admin** (~694 lines of controller code)
   - Application workflow, approval/rejection, superadmin features, system monitoring

Each module has:
- ✅ Clear file ownership
- ✅ Specific responsibilities
- ✅ Defined integration points
- ✅ Testing scope
- ✅ Balanced workload

The division follows the natural flow of the application: **Authentication → Event Creation → Application Management**, allowing for sequential or parallel development with minimal conflicts.
