using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VolunteerManagementSystem.Data;
using VolunteerManagementSystem.Models;

namespace VolunteerManagementSystem.Controllers
{
    /// <summary>
    /// Manage NGO organization profiles (CRUD). Approval lifecycle removed.
    /// NGOs can manage their own profile. Superuser can still soft-delete.
    /// </summary>
    [Authorize]
    public class NGOProfilesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        public NGOProfilesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }

        // GET: NGOProfiles - disabled
        // Listing disabled in UI; superuser can manage via dedicated views/actions
        [Authorize(Roles = "Superuser")]
        public async Task<IActionResult> Index()
        {
            return NotFound();
        }

        // GET: NGOProfiles/Details/5 - View a single NGO profile (owner or superuser)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ngoProfile = await _context.NGOProfiles
                .Include(n => n.User)
                .Include(n => n.Events)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ngoProfile == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);

            // Check if user owns this profile or is superuser
            if (ngoProfile.UserId != currentUser.Id && !User.IsInRole("Superuser"))
            {
                return Forbid();
            }

            return View(ngoProfile);
        }

        // GET: NGOProfiles/Create - Render create form (NGO/Superuser)
        [Authorize(Roles = "NGO,Superuser")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: NGOProfiles/Create - Persist new profile; logo upload supported
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "NGO,Superuser")]
        public async Task<IActionResult> Create([Bind("Name,Mission,ContactEmail,ContactPhone,Address,Description")] NGOProfile ngoProfile, IFormFile? logoFile)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            // Check if user already has an NGO profile
            var existingProfile = await _context.NGOProfiles
                .FirstOrDefaultAsync(n => n.UserId == currentUser.Id);

            if (existingProfile != null)
            {
                ModelState.AddModelError("", "You already have an NGO profile.");
                return View(ngoProfile);
            }

            if (ModelState.IsValid)
            {
                ngoProfile.UserId = currentUser.Id;
                ngoProfile.CreatedAt = DateTime.UtcNow;

                // Handle logo upload
                if (logoFile != null && logoFile.Length > 0)
                {
                    var uploadsDir = Path.Combine(_environment.WebRootPath, "uploads", "logos");
                    if (!Directory.Exists(uploadsDir))
                    {
                        Directory.CreateDirectory(uploadsDir);
                    }

                    var fileName = $"{currentUser.Id}_{DateTime.Now:yyyyMMddHHmmss}_{logoFile.FileName}";
                    var filePath = Path.Combine(uploadsDir, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await logoFile.CopyToAsync(stream);
                    }

                    ngoProfile.LogoPath = $"uploads/logos/{fileName}";
                }

                // New NGO profiles are pending approval (IsActive=false set during creation in AccountController)
                _context.Add(ngoProfile);
                await _context.SaveChangesAsync();

                TempData["Message"] = "NGO profile created successfully!";
                return RedirectToAction("Details", new { id = ngoProfile.Id });
            }

            return View(ngoProfile);
        }

        // GET: NGOProfiles/Edit/5 - Owners or superuser can edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ngoProfile = await _context.NGOProfiles.FindAsync(id);
            if (ngoProfile == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);

            // Check if user owns this profile or is superuser
            if (ngoProfile.UserId != currentUser.Id && !User.IsInRole("Superuser"))
            {
                return Forbid();
            }

            return View(ngoProfile);
        }

        // POST: NGOProfiles/Edit/5 - Save edits and optionally replace logo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Mission,ContactEmail,ContactPhone,Address,Description")] NGOProfile ngoProfile, IFormFile? logoFile)
        {
            if (id != ngoProfile.Id)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var existingProfile = await _context.NGOProfiles.FindAsync(id);

            // Check if user owns this profile or is superuser
            if (existingProfile.UserId != currentUser.Id && !User.IsInRole("Superuser"))
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    existingProfile.Name = ngoProfile.Name;
                    existingProfile.Mission = ngoProfile.Mission;
                    existingProfile.ContactEmail = ngoProfile.ContactEmail;
                    existingProfile.ContactPhone = ngoProfile.ContactPhone;
                    existingProfile.Address = ngoProfile.Address;
                    existingProfile.Description = ngoProfile.Description;
                    existingProfile.UpdatedAt = DateTime.UtcNow;

                    // Handle logo upload
                    if (logoFile != null && logoFile.Length > 0)
                    {
                        var uploadsDir = Path.Combine(_environment.WebRootPath, "uploads", "logos");
                        if (!Directory.Exists(uploadsDir))
                        {
                            Directory.CreateDirectory(uploadsDir);
                        }

                        var fileName = $"{currentUser.Id}_{DateTime.Now:yyyyMMddHHmmss}_{logoFile.FileName}";
                        var filePath = Path.Combine(uploadsDir, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await logoFile.CopyToAsync(stream);
                        }

                        // Delete old logo if exists
                        if (!string.IsNullOrEmpty(existingProfile.LogoPath))
                        {
                            var oldFilePath = Path.Combine(_environment.WebRootPath, existingProfile.LogoPath);
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }

                        existingProfile.LogoPath = $"uploads/logos/{fileName}";
                    }

                    _context.Update(existingProfile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NGOProfileExists(ngoProfile.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                TempData["Message"] = "NGO profile updated successfully!";
                return RedirectToAction("Details", new { id = ngoProfile.Id });
            }

            return View(ngoProfile);
        }

        // GET: NGOProfiles/Delete/5 - Confirm soft delete (Superuser)
        [Authorize(Roles = "Superuser")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ngoProfile = await _context.NGOProfiles
                .Include(n => n.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ngoProfile == null)
            {
                return NotFound();
            }

            return View(ngoProfile);
        }

        // POST: NGOProfiles/Delete/5 - Soft delete by disabling profile
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Superuser")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ngoProfile = await _context.NGOProfiles.FindAsync(id);
            
            if (ngoProfile != null)
            {
                ngoProfile.IsActive = false;
                ngoProfile.UpdatedAt = DateTime.UtcNow;
                _context.Update(ngoProfile);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // Approval/reject actions removed; NGOs are active on creation.

        private bool NGOProfileExists(int id)
        {
            return _context.NGOProfiles.Any(e => e.Id == id);
        }
    }
}
