using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VolunteerManagementSystem.Data;
using VolunteerManagementSystem.Models;

namespace VolunteerManagementSystem.Controllers
{
    /// <summary>
    /// Super Admin controller for managing volunteers and NGOs
    /// Only accessible by Superuser role
    /// </summary>
    [Authorize(Roles = "Superuser")]
    public class SuperAdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public SuperAdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: SuperAdmin/Volunteers - List all volunteers
        public async Task<IActionResult> Volunteers()
        {
            // Get all users with Volunteer role
            var volunteers = new List<ApplicationUser>();
            var allUsers = await _userManager.Users.ToListAsync();
            
            foreach (var user in allUsers)
            {
                if (await _userManager.IsInRoleAsync(user, "Volunteer"))
                {
                    volunteers.Add(user);
                }
            }

            // Get application counts for each volunteer
            var volunteerStats = new Dictionary<string, int>();
            foreach (var volunteer in volunteers)
            {
                var applicationCount = await _context.VolunteerApplications
                    .CountAsync(va => va.UserId == volunteer.Id);
                volunteerStats[volunteer.Id] = applicationCount;
            }

            ViewBag.VolunteerStats = volunteerStats;
            return View(volunteers);
        }

        // GET: SuperAdmin/VolunteerDetails/5 - View volunteer details
        public async Task<IActionResult> VolunteerDetails(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var volunteer = await _userManager.FindByIdAsync(id);
            if (volunteer == null || !await _userManager.IsInRoleAsync(volunteer, "Volunteer"))
            {
                return NotFound();
            }

            // Get volunteer's applications
            var applications = await _context.VolunteerApplications
                .Include(va => va.Event)
                .ThenInclude(e => e.NGO)
                .Where(va => va.UserId == id)
                .OrderByDescending(va => va.AppliedAt)
                .ToListAsync();

            ViewBag.Applications = applications;
            return View(volunteer);
        }

        // POST: SuperAdmin/DeleteVolunteer/5 - Delete a volunteer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteVolunteer(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var volunteer = await _userManager.FindByIdAsync(id);
            if (volunteer == null || !await _userManager.IsInRoleAsync(volunteer, "Volunteer"))
            {
                TempData["Error"] = "Volunteer not found.";
                return RedirectToAction(nameof(Volunteers));
            }

            // Delete all volunteer applications first
            var applications = await _context.VolunteerApplications
                .Where(va => va.UserId == id)
                .ToListAsync();
            
            _context.VolunteerApplications.RemoveRange(applications);
            await _context.SaveChangesAsync();

            // Delete the user
            var result = await _userManager.DeleteAsync(volunteer);
            if (result.Succeeded)
            {
                TempData["Success"] = "Volunteer deleted successfully!";
            }
            else
            {
                TempData["Error"] = "Failed to delete volunteer.";
            }

            return RedirectToAction(nameof(Volunteers));
        }

        // GET: SuperAdmin/NGOs - List all NGOs
        public async Task<IActionResult> NGOs()
        {
            var ngoProfiles = await _context.NGOProfiles
                .Include(n => n.User)
                .ToListAsync();

            // Get event counts for each NGO
            var ngoStats = new Dictionary<int, int>();
            foreach (var ngo in ngoProfiles)
            {
                var eventCount = await _context.Events
                    .CountAsync(e => e.NGOId == ngo.Id);
                ngoStats[ngo.Id] = eventCount;
            }

            ViewBag.NGOStats = ngoStats;
            return View(ngoProfiles);
        }

        // GET: SuperAdmin/NGODetails/5 - View NGO details
        public async Task<IActionResult> NGODetails(int id)
        {
            var ngoProfile = await _context.NGOProfiles
                .Include(n => n.User)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (ngoProfile == null)
            {
                return NotFound();
            }

            // Get NGO's events
            var events = await _context.Events
                .Where(e => e.NGOId == id)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();

            ViewBag.Events = events;
            return View(ngoProfile);
        }

        // POST: SuperAdmin/DeleteNGO/5 - Delete an NGO
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteNGO(int id)
        {
            var ngoProfile = await _context.NGOProfiles
                .Include(n => n.User)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (ngoProfile == null)
            {
                TempData["Error"] = "NGO not found.";
                return RedirectToAction(nameof(NGOs));
            }

            // Delete all events and their applications
            var events = await _context.Events
                .Where(e => e.NGOId == id)
                .ToListAsync();

            foreach (var eventItem in events)
            {
                var applications = await _context.VolunteerApplications
                    .Where(va => va.EventId == eventItem.Id)
                    .ToListAsync();
                
                _context.VolunteerApplications.RemoveRange(applications);
            }

            _context.Events.RemoveRange(events);
            
            // Delete NGO profile
            _context.NGOProfiles.Remove(ngoProfile);
            await _context.SaveChangesAsync();

            // Delete the user
            var result = await _userManager.DeleteAsync(ngoProfile.User);
            if (result.Succeeded)
            {
                TempData["Success"] = "NGO deleted successfully!";
            }
            else
            {
                TempData["Error"] = "Failed to delete NGO user account.";
            }

            return RedirectToAction(nameof(NGOs));
        }
    }
}
