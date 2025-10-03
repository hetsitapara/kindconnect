using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VolunteerManagementSystem.Data;
using VolunteerManagementSystem.Models;

namespace VolunteerManagementSystem.Controllers
{
    /// <summary>
    /// Role-based dashboards and stats:
    /// - Superuser: system overview, recent events/applications
    /// - Volunteer: personal applications and upcoming approved events
    /// - NGO: redirected to Events management; also event details view for owners
    /// </summary>
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Entry point: redirect user to appropriate dashboard by role
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            
            if (User.IsInRole("Superuser"))
            {
                return RedirectToAction("Superuser");
            }
            else if (User.IsInRole("NGO"))
            {
                return RedirectToAction("Index", "Events");
            }
            else if (User.IsInRole("Volunteer"))
            {
                return RedirectToAction("Browse", "Events");
            }

            return RedirectToAction("Index", "Home");
        }

        // Superuser dashboard with global stats and recents
        [Authorize(Roles = "Superuser")]
        public async Task<IActionResult> Superuser()
        {
            var stats = new
            {
                TotalUsers = await _context.Users.CountAsync(),
                TotalNGOs = await _context.NGOProfiles.CountAsync(),
                TotalEvents = await _context.Events.CountAsync(),
                TotalApplications = await _context.VolunteerApplications.CountAsync(),
                PendingApplications = await _context.VolunteerApplications.CountAsync(va => va.Status == ApplicationStatus.Pending),
                ActiveEvents = await _context.Events.CountAsync(e => e.IsActive && e.StartAt > DateTime.UtcNow)
            };

            var recentEvents = await _context.Events
                .Include(e => e.NGO)
                .Where(e => e.IsActive)
                .OrderByDescending(e => e.CreatedAt)
                .Take(10)
                .ToListAsync();

            var recentApplications = await _context.VolunteerApplications
                .Include(va => va.Event)
                .Include(va => va.User)
                .OrderByDescending(va => va.AppliedAt)
                .Take(10)
                .ToListAsync();

            ViewBag.Stats = stats;
            ViewBag.RecentEvents = recentEvents;
            ViewBag.RecentApplications = recentApplications;

            return View();
        }


        // Volunteer dashboard showing their application stats and upcoming events
        [Authorize(Roles = "Volunteer")]
        public async Task<IActionResult> Volunteer()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var stats = new
            {
                TotalApplications = await _context.VolunteerApplications.CountAsync(va => va.UserId == currentUser.Id),
                PendingApplications = await _context.VolunteerApplications.CountAsync(va => va.UserId == currentUser.Id && va.Status == ApplicationStatus.Pending),
                ApprovedApplications = await _context.VolunteerApplications.CountAsync(va => va.UserId == currentUser.Id && va.Status == ApplicationStatus.Approved),
            };

            var myApplications = await _context.VolunteerApplications
                .Include(va => va.Event)
                .Where(va => va.UserId == currentUser.Id)
                .OrderByDescending(va => va.AppliedAt)
                .Take(10)
                .ToListAsync();


            var upcomingEvents = await _context.VolunteerApplications
                .Include(va => va.Event)
                .Where(va => va.UserId == currentUser.Id && va.Status == ApplicationStatus.Approved && va.Event.StartAt > DateTime.UtcNow)
                .OrderBy(va => va.Event.StartAt)
                .Take(5)
                .ToListAsync();

            ViewBag.Stats = stats;
            ViewBag.MyApplications = myApplications;
            ViewBag.UpcomingEvents = upcomingEvents;

            return View();
        }

        // Event details view for NGO owners and superuser with applications listing
        [Authorize(Roles = "NGO,Superuser")]
        public async Task<IActionResult> EventDetails(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var ngoProfile = await _context.NGOProfiles.FirstOrDefaultAsync(n => n.UserId == currentUser.Id);

            var eventItem = await _context.Events
                .Include(e => e.NGO)
                .Include(e => e.VolunteerApplications)
                .ThenInclude(va => va.User)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (eventItem == null)
            {
                return NotFound();
            }

            // Check if user owns this event or is superuser
            if (ngoProfile == null || (eventItem.NGOId != ngoProfile.Id && !User.IsInRole("Superuser")))
            {
                return Forbid();
            }

            var applications = await _context.VolunteerApplications
                .Include(va => va.User)
                .Where(va => va.EventId == id)
                .OrderByDescending(va => va.AppliedAt)
                .ToListAsync();


            ViewBag.Event = eventItem;
            ViewBag.Applications = applications;

            return View();
        }
    }
}
