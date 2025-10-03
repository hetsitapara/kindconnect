using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VolunteerManagementSystem.Data;
using VolunteerManagementSystem.Models;
using VolunteerManagementSystem.Models.ViewModels;

namespace VolunteerManagementSystem.Controllers
{
    /// <summary>
    /// Handles volunteer application lifecycle: apply, view, NGO manage, approve/reject/cancel.
    /// Volunteers manage their applications; NGOs manage applications for their events.
    /// </summary>
    [Authorize]
    public class VolunteerApplicationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public VolunteerApplicationsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: VolunteerApplications - List current user's applications
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var applications = _context.VolunteerApplications
                .Include(va => va.Event)
                .Include(va => va.User)
                .Where(va => va.UserId == currentUser.Id)
                .OrderByDescending(va => va.AppliedAt)
                .ToListAsync();

            return View(await applications);
        }

        // GET: VolunteerApplications/Apply/5 - Render apply form (Volunteer only)
        [Authorize(Roles = "Volunteer")]
        public async Task<IActionResult> Apply(int? eventId)
        {
            if (eventId == null)
            {
                return NotFound();
            }

            var eventItem = await _context.Events
                .Include(e => e.NGO)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (eventItem == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);

            // Check if user has already applied
            var existingApplication = await _context.VolunteerApplications
                .FirstOrDefaultAsync(va => va.EventId == eventId && va.UserId == currentUser.Id);

            if (existingApplication != null)
            {
                TempData["Message"] = "You have already applied for this event.";
                return RedirectToAction("Details", "Events", new { id = eventId });
            }

            // Check if event is full
            if (eventItem.IsFull)
            {
                TempData["Message"] = "This event is full.";
                return RedirectToAction("Details", "Events", new { id = eventId });
            }

            ViewBag.Event = eventItem;
            return View(new VolunteerApplicationViewModel());
        }

        // POST: VolunteerApplications/Apply - Submit application for an event
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Volunteer")]
        public async Task<IActionResult> Apply(int eventId, VolunteerApplicationViewModel model)
        {
            var eventItem = await _context.Events.FindAsync(eventId);
            if (eventItem == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);

            // Check if user has already applied
            var existingApplication = await _context.VolunteerApplications
                .FirstOrDefaultAsync(va => va.EventId == eventId && va.UserId == currentUser.Id);

            if (existingApplication != null)
            {
                TempData["Message"] = "You have already applied for this event.";
                return RedirectToAction("Details", "Events", new { id = eventId });
            }

            // Check if event is full
            if (eventItem.IsFull)
            {
                TempData["Message"] = "This event is full.";
                return RedirectToAction("Details", "Events", new { id = eventId });
            }

            if (ModelState.IsValid)
            {
                // Create VolunteerApplication entity from view model
                var application = new VolunteerApplication
                {
                    EventId = eventId,
                    UserId = currentUser.Id,
                    Message = model.Message,
                    Skills = model.Skills,
                    Availability = model.Availability,
                    AppliedAt = DateTime.UtcNow,
                    Status = ApplicationStatus.Pending
                };

                _context.Add(application);
                await _context.SaveChangesAsync();

                TempData["Message"] = "Your application has been submitted successfully!";
                return RedirectToAction("Details", "Events", new { id = eventId });
            }

            ViewBag.Event = eventItem;
            return View(model);
        }

        // GET: VolunteerApplications/Details/5 - View a single application (owner/NGO/Superuser)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var application = await _context.VolunteerApplications
                .Include(va => va.Event)
                    .ThenInclude(e => e.NGO)
                .Include(va => va.User)
                .Include(va => va.RespondedByUser)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (application == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            // Check if user owns this application or is NGO/Superuser
            if (application.UserId != currentUser.Id && !User.IsInRole("NGO") && !User.IsInRole("Superuser"))
            {
                return Forbid();
            }

            ViewBag.CurrentUserId = currentUser.Id;
            return View(application);
        }

        // GET: VolunteerApplications/Manage/5 - NGO manage applications for a single event
        [Authorize(Roles = "NGO,Superuser")]
        public async Task<IActionResult> Manage(int? eventId)
        {
            if (eventId == null)
            {
                return NotFound();
            }

            var eventItem = await _context.Events
                .Include(e => e.NGO)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (eventItem == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var ngoProfile = await _context.NGOProfiles.FirstOrDefaultAsync(n => n.UserId == currentUser.Id);

            // Check if user owns this event or is superuser
            if (ngoProfile == null || (eventItem.NGOId != ngoProfile.Id && !User.IsInRole("Superuser")))
            {
                return Forbid();
            }

            var applications = await _context.VolunteerApplications
                .Include(va => va.User)
                .Where(va => va.EventId == eventId)
                .OrderByDescending(va => va.AppliedAt)
                .ToListAsync();

            ViewBag.Event = eventItem;
            return View(applications);
        }

        // GET: VolunteerApplications/ManageAll - NGO overview of all their events' applications
        [Authorize(Roles = "NGO,Superuser")]
        public async Task<IActionResult> ManageAll()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            var ngoProfile = await _context.NGOProfiles.FirstOrDefaultAsync(n => n.UserId == currentUser.Id);
            if (ngoProfile == null && !User.IsInRole("Superuser"))
            {
                return Forbid();
            }

            var query = _context.VolunteerApplications
                .Include(va => va.Event)
                    .ThenInclude(e => e.NGO)
                .Include(va => va.User)
                .Include(va => va.RespondedByUser)
                .AsQueryable();

            // If not superuser, only show applications for their events
            if (!User.IsInRole("Superuser"))
            {
                query = query.Where(va => va.Event.NGOId == ngoProfile.Id);
            }

            var applications = await query
                .OrderByDescending(va => va.AppliedAt)
                .ToListAsync();

            // Get summary statistics
            var stats = new
            {
                TotalApplications = applications.Count,
                PendingApplications = applications.Count(a => a.Status == ApplicationStatus.Pending),
                ApprovedApplications = applications.Count(a => a.Status == ApplicationStatus.Approved),
                RejectedApplications = applications.Count(a => a.Status == ApplicationStatus.Rejected),
                CancelledApplications = applications.Count(a => a.Status == ApplicationStatus.Cancelled)
            };

            ViewBag.Stats = stats;
            return View(applications);
        }

        // POST: VolunteerApplications/Approve/5 - NGO approve an application
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "NGO,Superuser")]
        public async Task<IActionResult> Approve(int id, string? responseMessage)
        {
            var application = await _context.VolunteerApplications
                .Include(va => va.Event)
                .Include(va => va.User)
                .FirstOrDefaultAsync(va => va.Id == id);

            if (application == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var ngoProfile = await _context.NGOProfiles.FirstOrDefaultAsync(n => n.UserId == currentUser.Id);

            // Check if user owns this event or is superuser
            if (ngoProfile == null || (application.Event.NGOId != ngoProfile.Id && !User.IsInRole("Superuser")))
            {
                return Forbid();
            }

            // Check if event is full
            if (application.Event.IsFull && application.Status != ApplicationStatus.Approved)
            {
                TempData["Message"] = "Cannot approve application. Event is full.";
                return RedirectToAction("Manage", new { eventId = application.EventId });
            }

            application.Status = ApplicationStatus.Approved;
            application.RespondedAt = DateTime.UtcNow;
            application.ResponseMessage = responseMessage;
            application.RespondedByUserId = currentUser.Id;

            _context.Update(application);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Application approved successfully!";
            return RedirectToAction("Manage", new { eventId = application.EventId });
        }

        // POST: VolunteerApplications/Reject/5 - NGO reject an application
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "NGO,Superuser")]
        public async Task<IActionResult> Reject(int id, string? responseMessage)
        {
            var application = await _context.VolunteerApplications
                .Include(va => va.Event)
                .Include(va => va.User)
                .FirstOrDefaultAsync(va => va.Id == id);

            if (application == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var ngoProfile = await _context.NGOProfiles.FirstOrDefaultAsync(n => n.UserId == currentUser.Id);

            // Check if user owns this event or is superuser
            if (ngoProfile == null || (application.Event.NGOId != ngoProfile.Id && !User.IsInRole("Superuser")))
            {
                return Forbid();
            }

            application.Status = ApplicationStatus.Rejected;
            application.RespondedAt = DateTime.UtcNow;
            application.ResponseMessage = responseMessage;
            application.RespondedByUserId = currentUser.Id;

            _context.Update(application);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Application rejected.";
            return RedirectToAction("Manage", new { eventId = application.EventId });
        }

        // POST: VolunteerApplications/Cancel/5 - Volunteer cancels their pending application
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var application = await _context.VolunteerApplications
                .Include(va => va.Event)
                .Include(va => va.User)
                .FirstOrDefaultAsync(va => va.Id == id);

            if (application == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);

            // Check if user owns this application
            if (application.UserId != currentUser.Id)
            {
                return Forbid();
            }

            // Only allow cancellation if status is pending
            if (application.Status != ApplicationStatus.Pending)
            {
                TempData["Message"] = "Cannot cancel application. Status is not pending.";
                return RedirectToAction("Index");
            }

            application.Status = ApplicationStatus.Cancelled;
            application.RespondedAt = DateTime.UtcNow;

            _context.Update(application);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Application cancelled successfully.";
            return RedirectToAction("Index");
        }
    }
}
