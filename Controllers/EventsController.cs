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
    /// Public event listing and NGO event CRUD.
    /// Volunteers see public/active events; Superuser can see all events.
    /// NGOs can create/edit/delete their own events (no approval required).
    /// </summary>
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EventsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Events - List events for browsing
        public async Task<IActionResult> Index()
        {
            var query = _context.Events
                .Include(e => e.NGO)
                .Where(e => e.IsActive && e.IsPublic)
                .OrderBy(e => e.StartAt)
                .AsQueryable();

            // If Superuser, show everything including private/inactive
            if (User.Identity.IsAuthenticated && User.IsInRole("Superuser"))
            {
                query = _context.Events
                    .Include(e => e.NGO)
                    .OrderBy(e => e.StartAt)
                    .AsQueryable();
            }
            // If NGO, show only their own events
            else if (User.Identity.IsAuthenticated && User.IsInRole("NGO"))
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var ngoProfile = await _context.NGOProfiles
                    .FirstOrDefaultAsync(n => n.UserId == currentUser.Id);

                if (ngoProfile != null)
                {
                    query = _context.Events
                        .Include(e => e.NGO)
                        .Where(e => e.NGOId == ngoProfile.Id && e.IsActive)
                        .OrderBy(e => e.StartAt)
                        .AsQueryable();
                }
                else
                {
                    // If NGO profile not found, show empty list
                    query = _context.Events
                        .Include(e => e.NGO)
                        .Where(e => false) // Empty query
                        .AsQueryable();
                }
            }

            var events = await query.ToListAsync();

            // Check if current user has applied for each event (only for volunteers)
            if (User.Identity.IsAuthenticated && User.IsInRole("Volunteer"))
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser != null)
                {
                    var userApplications = await _context.VolunteerApplications
                        .Where(va => va.UserId == currentUser.Id)
                        .Select(va => va.EventId)
                        .ToListAsync();

                    ViewBag.UserApplications = userApplications;
                }
            }

            return View(events);
        }

        // GET: Events/Browse - Browse all public events (for volunteers)
        [Authorize(Roles = "Volunteer")]
        public async Task<IActionResult> Browse()
        {
            var events = await _context.Events
                .Include(e => e.NGO)
                .Where(e => e.IsActive && e.IsPublic)
                .OrderBy(e => e.StartAt)
                .ToListAsync();

            // Check if current user has applied for each event
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser != null)
            {
                var userApplications = await _context.VolunteerApplications
                    .Where(va => va.UserId == currentUser.Id)
                    .Select(va => va.EventId)
                    .ToListAsync();

                ViewBag.UserApplications = userApplications;
            }

            return View("Index", events);
        }

        // GET: Events/Details/5 - Show event details and application state for volunteer
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventItem = await _context.Events
                .Include(e => e.NGO)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (eventItem == null)
            {
                return NotFound();
            }

            // Check if current user has applied for this event
            if (User.Identity.IsAuthenticated && User.IsInRole("Volunteer"))
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser != null)
                {
                    var hasApplied = await _context.VolunteerApplications
                        .AnyAsync(va => va.EventId == id && va.UserId == currentUser.Id);

                    ViewBag.HasApplied = hasApplied;
                }
            }

            return View(eventItem);
        }

        // GET: Events/Create - Render create form (NGO/Superuser only)
        [Authorize(Roles = "NGO,Superuser")]
        public IActionResult Create()
        {
            return View(new CreateEventViewModel());
        }

        // POST: Events/Create - Create a new event owned by current NGO
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "NGO,Superuser")]
        public async Task<IActionResult> Create(CreateEventViewModel model)
        {
            try
            {
                // Get current user
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    TempData["Error"] = "User not found. Please log in again.";
                    return RedirectToAction("Login", "Account");
                }

                // Find NGO profile for current user
                var ngoProfile = await _context.NGOProfiles
                    .FirstOrDefaultAsync(n => n.UserId == currentUser.Id);

                if (ngoProfile == null)
                {
                    TempData["Error"] = "NGO profile not found. Please contact administrator.";
                    return View(model);
                }

                // Approval is not required anymore; NGOs can create events immediately.

                // Validate model
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                // Validate business rules
                if (model.StartAt <= DateTime.UtcNow)
                {
                    ModelState.AddModelError("StartAt", "Start date must be in the future.");
                    return View(model);
                }

                if (model.EndAt <= model.StartAt)
                {
                    ModelState.AddModelError("EndAt", "End date must be after start date.");
                    return View(model);
                }

                // Create Event entity from ViewModel
                var eventItem = new Event
                {
                    NGOId = ngoProfile.Id,
                    Title = model.Title,
                    Description = model.Description,
                    Category = model.Category,
                    StartAt = model.StartAt,
                    EndAt = model.EndAt,
                    Venue = model.Venue,
                    Capacity = model.Capacity,
                    // Force events to be public by default; create form no longer exposes this field
                    IsPublic = true,
                    ContactPerson = model.ContactPerson,
                    ContactPhone = model.ContactPhone,
                    ContactEmail = model.ContactEmail,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                // Save to database
                _context.Events.Add(eventItem);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Event created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while creating the event. Please try again.";
                return View(model);
            }
        }

        // GET: Events/Edit/5 - Render edit form (NGO must own event or Superuser)
        [Authorize(Roles = "NGO,Superuser")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventItem = await _context.Events.FindAsync(id);
            if (eventItem == null)
            {
                return NotFound();
            }

            // Check if user owns this event
            var currentUser = await _userManager.GetUserAsync(User);
            var ngoProfile = await _context.NGOProfiles
                .FirstOrDefaultAsync(n => n.UserId == currentUser.Id);

            if ((ngoProfile == null || eventItem.NGOId != ngoProfile.Id) && !User.IsInRole("Superuser"))
            {
                TempData["Error"] = "You don't have permission to edit this event.";
                return RedirectToAction(nameof(Index));
            }

            // Approval no longer required

            // Map Event entity to EditEventViewModel
            var editViewModel = new EditEventViewModel
            {
                Id = eventItem.Id,
                Title = eventItem.Title,
                Description = eventItem.Description,
                Category = eventItem.Category,
                StartAt = eventItem.StartAt,
                EndAt = eventItem.EndAt,
                Venue = eventItem.Venue,
                Capacity = eventItem.Capacity,
                IsPublic = eventItem.IsPublic,
                ContactPerson = eventItem.ContactPerson,
                ContactPhone = eventItem.ContactPhone,
                ContactEmail = eventItem.ContactEmail,
                CreatedAt = eventItem.CreatedAt,
                UpdatedAt = eventItem.UpdatedAt,
                IsActive = eventItem.IsActive,
                NGOId = eventItem.NGOId
            };

            return View(editViewModel);
        }

        // POST: Events/Edit/5 - Persist edits to existing event
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "NGO,Superuser")]
        public async Task<IActionResult> Edit(int id, EditEventViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            try
            {
                // Get the original event from database to check ownership
                var originalEvent = await _context.Events.FindAsync(id);
                if (originalEvent == null)
                {
                    return NotFound();
                }

                // Check if user owns this event
                var currentUser = await _userManager.GetUserAsync(User);
                var ngoProfile = await _context.NGOProfiles
                    .FirstOrDefaultAsync(n => n.UserId == currentUser.Id);

                if ((ngoProfile == null || originalEvent.NGOId != ngoProfile.Id) && !User.IsInRole("Superuser"))
                {
                    TempData["Error"] = "You don't have permission to edit this event.";
                    return RedirectToAction(nameof(Index));
                }

                // Approval no longer required

                // Validate model
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                // Validate business rules
                if (model.StartAt <= DateTime.UtcNow)
                {
                    ModelState.AddModelError("StartAt", "Start date must be in the future.");
                    return View(model);
                }

                if (model.EndAt <= model.StartAt)
                {
                    ModelState.AddModelError("EndAt", "End date must be after start date.");
                    return View(model);
                }

                // Update the original event with new values
                originalEvent.Title = model.Title;
                originalEvent.Description = model.Description;
                originalEvent.Category = model.Category;
                originalEvent.StartAt = model.StartAt;
                originalEvent.EndAt = model.EndAt;
                originalEvent.Venue = model.Venue;
                originalEvent.Capacity = model.Capacity;
                originalEvent.IsPublic = model.IsPublic;
                originalEvent.ContactPerson = model.ContactPerson;
                originalEvent.ContactPhone = model.ContactPhone;
                originalEvent.ContactEmail = model.ContactEmail;
                originalEvent.UpdatedAt = DateTime.UtcNow;

                _context.Update(originalEvent);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Event updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventExists(model.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while updating the event. Please try again.";
                return View(model);
            }
        }

        // GET: Events/Delete/5 - Confirm soft-delete (mark inactive)
        [Authorize(Roles = "NGO,Superuser")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventItem = await _context.Events
                .Include(e => e.NGO)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (eventItem == null)
            {
                return NotFound();
            }

            // Check if user owns this event
            var currentUser = await _userManager.GetUserAsync(User);
            var ngoProfile = await _context.NGOProfiles
                .FirstOrDefaultAsync(n => n.UserId == currentUser.Id);

            if ((ngoProfile == null || eventItem.NGOId != ngoProfile.Id) && !User.IsInRole("Superuser"))
            {
                TempData["Error"] = "You don't have permission to edit this event.";
                return RedirectToAction(nameof(Index));
            }

            // Approval no longer required

            return View(eventItem);
        }

        // POST: Events/Delete/5 - Perform soft-delete (IsActive=false)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "NGO,Superuser")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var eventItem = await _context.Events.FindAsync(id);
            if (eventItem != null)
            {
                // Soft delete - just mark as inactive
                eventItem.IsActive = false;
                eventItem.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Event deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.Id == id);
        }
    }
}